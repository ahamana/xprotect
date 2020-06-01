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
    public class ScratchFilterSettingManager
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
        private readonly ICollection<ScratchFilterSetting> settings = new List<ScratchFilterSetting>();

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
                serializer.Populate(jsonReader, settings);
            }
        }

        /// <summary>
        /// 傷フィルタの設定を取得します。
        /// </summary>
        /// <param name="cameraId">カメラの ID</param>
        /// <returns>傷フィルタの設定</returns>
        internal ScratchFilterSetting GetSetting(Guid cameraId)
        {
            if (cameraId == Guid.Empty)
            {
                throw new ArgumentException(nameof(cameraId));
            }

            ScratchFilterSetting setting = settings.FirstOrDefault(setting => cameraId == setting.CameraId);

            if (setting == null)
            {
                setting = new ScratchFilterSetting()
                {
                    CameraId = cameraId
                };

                settings.Add(setting);
            }

            return setting;
        }

        /// <summary>
        /// 傷フィルタの設定を保存します。
        /// </summary>
        internal void Save()
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
                serializer.Serialize(jsonWriter, settings);
            }
        }

        #endregion
    }
}
