//
// (C) 2020 Canon Inc.
//
// This file contains trade secrets of Canon Inc. No part may be reproduced
// or transmitted in any form by any means or for any purpose without the
// express written permission of Canon Inc.
//

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;

using Mapster;

namespace ScratchFilter.Client.Data
{
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
        private readonly IDictionary<Guid, ScratchFilterSetting> settings = new Dictionary<Guid, ScratchFilterSetting>();

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
        internal static ScratchFilterSettingManager Instance { get; } = new ScratchFilterSettingManager();

        #endregion Properties

        #region Methods

        /// <summary>
        /// 傷フィルタの設定を保存します。
        /// </summary>
        private void Save()
        {
            var dir = Path.GetDirectoryName(SettingFile);

            Directory.CreateDirectory(dir);

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

            JsonSerializer.Deserialize<List<ScratchFilterSetting>>(json).ForEach(setting =>
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
                setting = new ScratchFilterSetting
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
        /// <exception cref="ArgumentNullException"><paramref name="settings" /> が <c>null</c> の場合にスローされます。</exception>
        /// <exception cref="ArgumentException"><paramref name="settings" /> が空の場合にスローされます。</exception>
        internal void Save(params ScratchFilterSetting[] settings)
        {
            Save(Enumerable.AsEnumerable(settings));
        }

        /// <summary>
        /// 傷フィルタの設定を保存します。
        /// </summary>
        /// <param name="settings">更新対象の傷フィルタの設定の一覧</param>
        /// <exception cref="ArgumentNullException"><paramref name="settings" /> が <c>null</c> の場合にスローされます。</exception>
        /// <exception cref="ArgumentException"><paramref name="settings" /> が空の場合にスローされます。</exception>
        internal void Save(IEnumerable<ScratchFilterSetting> settings)
        {
            if (settings is null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            if (!settings.Any())
            {
                throw new ArgumentException(nameof(settings));
            }

            settings.ToList().ForEach(setting =>
            {
                this.settings.Remove(setting.CameraId);
                this.settings.Add(setting.CameraId, setting);
            });

            Save();
        }

        #endregion Methods
    }
}
