using Mapster;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;

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

        #endregion

        #region Properties

        /// <summary>
        /// インスタンスです。
        /// </summary>
        /// <value>
        /// インスタンス
        /// </value>
        internal static ScratchFilterSettingManager Instance { get; } = new ScratchFilterSettingManager();

        #endregion

        #region Constructors

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        private ScratchFilterSettingManager() { }

        #endregion

        #region Methods

        /// <summary>
        /// 傷フィルタの設定を保存します。
        /// </summary>
        private void Save()
        {
            string dir = Path.GetDirectoryName(SettingFile);

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            using Stream stream = File.Create(SettingFile);

            object value = settings.Values;

            JsonSerializerOptions options = new JsonSerializerOptions()
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

            string json = File.ReadAllText(SettingFile);

            JsonSerializer.Deserialize<List<ScratchFilterSetting>>(json).ForEach(setting =>
            {
                settings.Add(setting.CameraId, setting);
            });
        }

        /// <summary>
        /// 傷フィルタの設定を取得します。
        /// </summary>
        /// <param name="cameraId">カメラの ID</param>
        /// <returns>
        /// 傷フィルタの設定
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="cameraId" /> が <see cref="Guid.Empty" /> の場合にスローされます。
        /// </exception>
        internal ScratchFilterSetting GetSetting(Guid cameraId)
        {
            if (cameraId == Guid.Empty)
            {
                throw new ArgumentException(nameof(cameraId));
            }

            if (!settings.TryGetValue(cameraId, out ScratchFilterSetting setting))
            {
                setting = new ScratchFilterSetting()
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
        /// <param name="setting">更新対象の傷フィルタの設定</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="setting" /> が <c>null</c> の場合にスローされます。
        /// </exception>
        internal void Save(ScratchFilterSetting setting)
        {
            if (setting == null)
            {
                throw new ArgumentNullException(nameof(setting));
            }

            Save(new[] { setting });
        }

        /// <summary>
        /// 傷フィルタの設定を保存します。
        /// </summary>
        /// <param name="settings">更新対象の傷フィルタの設定の一覧</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="settings" /> が <c>null</c> の場合にスローされます。
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="settings" /> が空の場合にスローされます。
        /// </exception>
        internal void Save(IEnumerable<ScratchFilterSetting> settings)
        {
            if (settings == null)
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

        #endregion
    }
}
