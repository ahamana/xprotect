using Mapster;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

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
        /// 傷フィルタの設定ファイルのパスです。
        /// </summary>
        private static readonly string SettingFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
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
        internal static ScratchFilterSettingManager Instance
        {
            get;
        } = new ScratchFilterSettingManager();

        #endregion

        #region Constructors

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        private ScratchFilterSettingManager()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// 傷フィルタの設定を保存します。
        /// </summary>
        private void Save()
        {
            string dirPath = Path.GetDirectoryName(SettingFilePath);

            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            JsonSerializer serializer = new JsonSerializer()
            {
                Formatting = Formatting.Indented
            };

            using (TextWriter textWriter = new StreamWriter(SettingFilePath, false, Encoding.UTF8))
            using (JsonWriter jsonWriter = new JsonTextWriter(textWriter))
            {
                serializer.Serialize(jsonWriter, settings.Values);
            }
        }

        /// <summary>
        /// 傷フィルタの設定を読み込みます。
        /// </summary>
        internal void Load()
        {
            if (!File.Exists(SettingFilePath))
            {
                return;
            }

            JsonSerializer serializer = new JsonSerializer();

            using (TextReader textReader = new StreamReader(SettingFilePath, Encoding.UTF8))
            using (JsonReader jsonReader = new JsonTextReader(textReader))
            {
                serializer.Deserialize<List<ScratchFilterSetting>>(jsonReader).ForEach(setting =>
                {
                    settings.Add(setting.CameraId, setting);
                });
            }
        }

        /// <summary>
        /// 傷フィルタの設定を取得します。
        /// </summary>
        /// <param name="cameraId">カメラの ID</param>
        /// <returns>傷フィルタの設定</returns>
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
