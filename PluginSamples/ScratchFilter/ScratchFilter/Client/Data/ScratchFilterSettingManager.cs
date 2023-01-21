//
// MIT License
//
// (c) 2020 Akihiro Hamana
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;

using Mapster;

namespace ScratchFilter.Client.Data;

/// <summary>
/// 傷フィルタの設定のマネージャです。
/// </summary>
[ToString]
internal sealed class ScratchFilterSettingManager
{
    #region Fields

    /// <summary>
    /// バージョン情報です。
    /// </summary>
    private static readonly FileVersionInfo FileVersionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);

    /// <summary>
    /// 傷フィルタの設定ファイルです。
    /// </summary>
    private static readonly string SettingFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                                                              @"Canon\WebView",
                                                              FileVersionInfo.ProductName,
                                                              $"{nameof(ScratchFilterSetting)}.json");

    /// <summary>
    /// 傷フィルタの設定の一覧です。
    /// </summary>
    private readonly IDictionary<Guid, ScratchFilterSetting> settings = new SortedDictionary<Guid, ScratchFilterSetting>();

    #endregion Fields

    #region Constructors

    /// <summary>
    /// コンストラクタです。
    /// </summary>
    private ScratchFilterSettingManager() { }

    #endregion Constructors

    #region Properties

    /// <summary>
    /// インスタンスです。
    /// </summary>
    /// <value>インスタンス</value>
    internal static ScratchFilterSettingManager Instance { get; } = new();

    #endregion Properties

    #region Methods

    /// <summary>
    /// 傷フィルタの設定を保存します。
    /// </summary>
    private void Save()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(SettingFile));

        using var stream = File.Create(SettingFile);

        var value = settings.Values;

        var options = new JsonSerializerOptions
        {
            IgnoreReadOnlyProperties = true,
            WriteIndented = true
        };

        JsonSerializer.SerializeAsync(stream, value, value.GetType(), options).Wait();
    }

    /// <summary>
    /// 傷フィルタの設定を読み込みます。
    /// </summary>
    internal void Load()
    {
        if (!File.Exists(SettingFile))
        {
            return;
        }

        var json = File.ReadAllText(SettingFile);

        JsonSerializer.Deserialize<List<ScratchFilterSetting>>(json)?.ForEach(setting =>
        {
            settings.Add(setting.CameraId, setting);
        });
    }

    /// <summary>
    /// 傷フィルタの設定を取得します。
    /// </summary>
    /// <param name="cameraId">カメラの ID</param>
    /// <returns>傷フィルタの設定</returns>
    /// <exception cref="ArgumentException"><paramref name="cameraId" /> が <see cref="Guid.Empty" /> の場合にスローされます。</exception>
    internal ScratchFilterSetting GetSetting(Guid cameraId)
    {
        if (cameraId == Guid.Empty)
        {
            throw new ArgumentException(nameof(cameraId));
        }

        if (!settings.TryGetValue(cameraId, out var setting))
        {
            setting = new()
            {
                CameraId = cameraId
            };

            settings.Add(cameraId, setting);
        }

        return setting.Adapt<ScratchFilterSetting>();
    }

    /// <summary>
    /// 傷フィルタの設定を保存します。
    /// </summary>
    /// <param name="settings">更新対象の傷フィルタの設定</param>
    /// <exception cref="ArgumentException"><paramref name="settings" /> が空の場合にスローされます。</exception>
    internal void Save(params ScratchFilterSetting[] settings) =>
        Save(Enumerable.AsEnumerable(settings));

    /// <summary>
    /// 傷フィルタの設定を保存します。
    /// </summary>
    /// <param name="settings">更新対象の傷フィルタの設定の一覧</param>
    /// <exception cref="ArgumentException"><paramref name="settings" /> が空の場合にスローされます。</exception>
    internal void Save(IEnumerable<ScratchFilterSetting> settings)
    {
        if (!settings.Any())
        {
            throw new ArgumentException(nameof(settings));
        }

        foreach (var setting in settings)
        {
            this.settings.Remove(setting.CameraId);
            this.settings.Add(setting.CameraId, setting);
        }

        Save();
    }

    #endregion Methods
}
