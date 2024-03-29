﻿/*
 * MIT License
 *
 * (c) 2020 Akihiro Hamana
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

using ImageStore.Client.Data;

using NLog;

using VideoOS.Platform;
using VideoOS.Platform.Data;
using VideoOS.Platform.Live;
using VideoOS.Platform.Messaging;

namespace ImageStore;

/// <summary>
/// The PluginDefinition is the 'entry' point to any plugin.
/// This is the starting point for any plugin development and the class MUST be available for a plugin to be loaded.
/// Several PluginDefinitions are allowed to be available within one DLL.
/// Here the references to all other plugin known objects and classes are defined.
/// The class is an abstract class where all implemented methods and properties need to be declared with override.
/// The class is constructed when the environment is loading the DLL.
/// </summary>
/// <seealso cref="PluginDefinition" />
/// <seealso cref="IDisposable" />
public class ImageStorePluginDefinition : PluginDefinition, IDisposable
{
    #region Fields

    /// <summary>
    /// ロガーです。
    /// </summary>
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// アセンブリです。
    /// </summary>
    private static readonly Assembly Assembly = Assembly.GetExecutingAssembly();

    /// <summary>
    /// バージョン情報です。
    /// </summary>
    private static readonly FileVersionInfo FileVersionInfo = FileVersionInfo.GetVersionInfo(Assembly.Location);

    /// <summary>
    /// プラグインの ID です。
    /// </summary>
    private static readonly Guid PluginId = Guid.Parse(Assembly.GetCustomAttribute<GuidAttribute>().Value);

    /// <summary>
    /// メッセージを受信するためのフィルタです。
    /// </summary>
    private readonly List<object> messageCommunicationFilters = new();

    /// <summary>
    /// ライブ映像のソースです。
    /// </summary>
    private readonly IDictionary<FQID, LiveSource> liveSources = new Dictionary<FQID, LiveSource>();

    /// <summary>
    /// 設定です。
    /// </summary>
    private readonly ImageStoreSettings settings;

    /// <summary>
    /// アンマネージリソースが解放されたかどうかです。
    /// </summary>
    private bool isDisposed;

    #endregion Fields

    #region Constructors

    /// <summary>
    /// 静的コンストラクタです。
    /// </summary>
    static ImageStorePluginDefinition()
    {
        var productName = FileVersionInfo.ProductName;
        var environmentType = Enum.GetName(typeof(EnvironmentType), EnvironmentManager.Instance.EnvironmentType);

        LogManager.Configuration.Variables.Add(nameof(productName), productName);
        LogManager.Configuration.Variables.Add(nameof(environmentType), environmentType);
    }

    /// <summary>
    /// コンストラクタです。
    /// </summary>
    public ImageStorePluginDefinition()
    {
        settings = ImageStoreSettingsManager.Instance.Load();
    }

    #endregion Constructors

    #region Properties

    /// <inheritdoc />
    public sealed override Guid Id { get; } = PluginId;

    /// <inheritdoc />
    public sealed override string Name { get; } = FileVersionInfo.ProductName;

    /// <inheritdoc />
    public sealed override string Manufacturer { get; } = FileVersionInfo.CompanyName;

    /// <inheritdoc />
    public sealed override string VersionString { get; } = FileVersionInfo.ProductVersion;

    /// <inheritdoc />
    public sealed override Image? Icon { get; }

    #endregion Properties

    #region Methods

    /// <summary>
    /// 画像を保存するファイルのパスを生成します。
    /// </summary>
    /// <param name="cameraName">カメラ名</param>
    /// <returns>画像を保存するファイルのパス</returns>
    private string GenerateImageFilePath(in string cameraName)
    {
        var now = DateTime.Now;

        var filePath = Path.Combine(settings.OutputDir,
                                    $"{now:yyyy-MM-dd}",
                                    cameraName,
                                    $"{now:yyyy-MM-dd_HH-mm-ss-fff}.jpg");

        return filePath;
    }

    /// <summary>
    /// ライブ映像の新しいフレームを取得した時に発生します。
    /// </summary>
    /// <param name="sender">イベントハンドラがアタッチされるオブジェクト</param>
    /// <param name="e">イベントのデータ</param>
    private void OnLiveSourceLiveContentEvent(object sender, EventArgs e)
    {
        if (sender is not JPEGLiveSource liveSource)
        {
            return;
        }

        if (e is not LiveContentEventArgs { LiveContent: not null } args)
        {
            return;
        }

        var filePath = GenerateImageFilePath(liveSource.Item.Name);

        Directory.CreateDirectory(Path.GetDirectoryName(filePath));

        try
        {
            using var liveContent = args.LiveContent;

            File.WriteAllBytes(filePath, liveContent.Content);
        }
        catch
        {
            // Do nothing.
        }
    }

    /// <summary>
    /// 新規イベントが発生した場合に呼び出されます。
    /// </summary>
    /// <param name="message">新規イベントの内容</param>
    /// <param name="destination">通知先</param>
    /// <param name="sender">通知元</param>
    /// <returns><c>null</c></returns>
    private object? NewEventIndicationReceiver(Message message, FQID destination, FQID sender)
    {
        if (message.Data is not BaseEvent eventData)
        {
            return null;
        }

        var eventSource = eventData.EventHeader.Source;

        switch (eventData.EventHeader.Message)
        {
            case "Recording Started":
                if (!liveSources.ContainsKey(eventSource.FQID))
                {
                    var camera = Configuration.Instance.GetItem(eventSource.FQID);

                    var liveSource = new JPEGLiveSource(camera)
                    {
                        LiveModeStart = true,

                        // 幅と高さに 0 を指定して、実際の解像度の画像を取得
                        Width = 0,
                        Height = 0
                    };

                    if (Enumerable.Range(1, 100).Contains(settings.Compression))
                    {
                        liveSource.Compression = settings.Compression;
                    }

                    liveSource.LiveContentEvent += OnLiveSourceLiveContentEvent;

                    liveSource.Init();

                    liveSources.Add(eventSource.FQID, liveSource);
                }

                break;

            case "Recording Stopped":
                if (liveSources.ContainsKey(eventSource.FQID))
                {
                    var liveSource = liveSources[eventSource.FQID];

                    liveSource.Close();

                    liveSources.Remove(eventSource.FQID);
                }

                break;
        }

        return null;
    }

    /// <summary>
    /// アンマネージリソースを解放し、必要に応じてマネージリソースも解放します。
    /// </summary>
    /// <param name="disposing">マネージリソースとアンマネージリソースの両方を解放する場合は <c>true</c>。アンマネージリソースだけを解放する場合は <c>false</c>。</param>
    private void Dispose(bool disposing)
    {
        if (isDisposed)
        {
            return;
        }

        if (disposing)
        {
            Icon?.Dispose();

            foreach (var liveSource in liveSources.Values)
            {
                liveSource.Close();
            }
        }

        isDisposed = true;
    }

    /// <inheritdoc />
    public sealed override void Init()
    {
        Logger.ForInfoEvent()
              .Message("Start plug-in on {0}", EnvironmentManager.Instance.EnvironmentProduct)
              .Log();

        MessageCommunicationManager.Start(EnvironmentManager.Instance.MasterSite.ServerId);

        var messageCommunication = MessageCommunicationManager.Get(EnvironmentManager.Instance.MasterSite.ServerId);

        messageCommunicationFilters.Add(messageCommunication.RegisterCommunicationFilter(NewEventIndicationReceiver,
                                                                                         new(MessageId.Server.NewEventIndication)));
    }

    /// <inheritdoc />
    public sealed override void Close()
    {
        var messageCommunication = MessageCommunicationManager.Get(EnvironmentManager.Instance.MasterSite.ServerId);

        if (messageCommunication is not null)
        {
            messageCommunicationFilters.ForEach(messageCommunication.UnRegisterCommunicationFilter);
        }

        MessageCommunicationManager.Stop(EnvironmentManager.Instance.MasterSite.ServerId);

        Dispose();

        Logger.ForInfoEvent()
              .Message("End plug-in on {0}", EnvironmentManager.Instance.EnvironmentProduct)
              .Log();
    }

    /// <inheritdoc cref="IDisposable.Dispose" />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion Methods
}
