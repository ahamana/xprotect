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

using VideoOS.Platform;
using VideoOS.Platform.Client;
using VideoOS.Platform.Messaging;

namespace ScratchFilter.Client.Toolbar;

/// <summary>
/// ツールバー用プラグインのインスタンスのベースとなる抽象クラスです。
/// </summary>
/// <seealso cref="ViewItemToolbarPluginInstance" />
/// <seealso cref="IDisposable" />
[ToString]
internal abstract class ToolbarPluginInstance : ViewItemToolbarPluginInstance, IDisposable
{
    #region Fields

    /// <summary>
    /// メッセージの受信オブジェクトです。
    /// </summary>
    private readonly List<object> messageReceivers = new();

    /// <summary>
    /// アンマネージリソースが解放されたかどうかです。
    /// </summary>
    private bool isDisposed;

    #endregion Fields

    #region Constructors

    /// <summary>
    /// コンストラクタです。
    /// </summary>
    private protected ToolbarPluginInstance()
    {
        Enabled = false;
    }

    #endregion Constructors

    #region Properties

    /// <summary>
    /// プラグインの説明です。
    /// </summary>
    /// <value>プラグインの説明</value>
    private protected virtual string? Description { get; }

    #endregion Properties

    #region Methods

    /// <summary>
    /// テーマが変更された時に発生します。
    /// </summary>
    /// <param name="message">テーマ変更の内容</param>
    /// <param name="destination">通知先</param>
    /// <param name="sender">通知元</param>
    /// <returns><c>null</c></returns>
    private object? ThemeChangedIndicationReceiver(Message message, FQID destination, FQID sender)
    {
        ReloadIcon();

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
        }

        isDisposed = true;
    }

    /// <summary>
    /// アイコンを読み込みます。
    /// </summary>
    private protected abstract void LoadIcon();

    /// <summary>
    /// アイコンを再読み込みします。
    /// </summary>
    private protected void ReloadIcon()
    {
        // Dispose メソッドの実行のみとして、null は設定しない。
        // ※ null を設定すると、デフォルトアイコンが一瞬表示されてしまう。
        Icon?.Dispose();

        LoadIcon();
    }

    /// <inheritdoc />
    public override void Init(Item viewItemInstance, Item window)
    {
        Title = Description;
        Tooltip = Description;

        LoadIcon();

        messageReceivers.Add(EnvironmentManager.Instance.RegisterReceiver(ThemeChangedIndicationReceiver,
                                                                          new MessageIdFilter(MessageId.SmartClient.ThemeChangedIndication)));
    }

    /// <inheritdoc />
    public override void Close()
    {
        messageReceivers.ForEach(EnvironmentManager.Instance.UnRegisterReceiver);

        Dispose();
    }

    /// <inheritdoc cref="IDisposable.Dispose" />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion Methods
}

/// <summary>
/// ツールバー用プラグインのベースとなる抽象クラスです。
/// </summary>
/// <seealso cref="ViewItemToolbarPlugin" />
[ToString]
internal abstract class ToolbarPlugin : ViewItemToolbarPlugin
{
    #region Properties

    /// <remarks>
    /// Smart Client の UI のどこにも表示されず、使用されることはありません。
    /// </remarks>
    /// <inheritdoc />
    public sealed override string? Name { get; }

    #endregion Properties

    #region Methods

    /// <inheritdoc />
    public override void Init()
    {
        ViewItemToolbarPlaceDefinition.ViewItemIds = new()
        {
            ViewAndLayoutItem.CameraBuiltinId
        };

        ViewItemToolbarPlaceDefinition.WorkSpaceIds = new()
        {
            ClientControl.LiveBuildInWorkSpaceId,
            ClientControl.PlaybackBuildInWorkSpaceId
        };

        ViewItemToolbarPlaceDefinition.WorkSpaceStates = new()
        {
            WorkSpaceState.Normal
        };
    }

    /// <inheritdoc />
    public override void Close() { }

    #endregion Methods
}
