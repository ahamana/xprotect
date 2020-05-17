using System.IO;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace ImageStore.Client.Data
{
    /// <summary>
    /// 設定です。
    /// </summary>
    public class Settings
    {
        #region Fields

        /// <summary>
        /// 設定ファイルのパスです。
        /// </summary>
        private string filePath;

        #endregion

        #region Properties

        /// <summary>
        /// JPEG ファイルを出力するディレクトリです。
        /// </summary>
        /// <value>
        /// JPEG ファイルを出力するディレクトリ
        /// </value>
        public string OutputDir
        {
            get;
            set;
        }

        /// <summary>
        /// 出力する JPEG ファイルの幅です。
        /// </summary>
        /// <value>
        /// 出力する JPEG ファイルの幅
        /// </value>
        public ushort Width
        {
            get;
            set;
        }

        /// <summary>
        /// 出力する JPEG ファイルの高さです。
        /// </summary>
        /// <value>
        /// 出力する JPEG ファイルの高さ
        /// </value>
        public ushort Height
        {
            get;
            set;
        }

        /// <summary>
        /// 出力する JPEG ファイルの圧縮です。
        /// </summary>
        /// <value>
        /// 出力する JPEG ファイルの圧縮
        /// </value>
        public ushort Compression
        {
            get;
            set;
        }

        #endregion

        #region Methods

        /// <summary>
        /// 設定を読み込みます。
        /// </summary>
        /// <returns>設定</returns>
        public static Settings Load()
        {
            string directoryPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            string filePath = Path.Combine(directoryPath, "settings.xml");

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Settings));

            using (StreamReader reader = new StreamReader(filePath, new UTF8Encoding(true)))
            {
                Settings settings = (Settings)xmlSerializer.Deserialize(reader);
                settings.filePath = filePath;

                return settings;
            }
        }

        /// <summary>
        /// 設定を保存します。
        /// </summary>
        public void Save()
        {
            // デフォルトの名前空間の出力制御
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Settings));

            using (StreamWriter writer = new StreamWriter(filePath, false, new UTF8Encoding(true)))
            {
                xmlSerializer.Serialize(writer, this, namespaces);
            }
        }

        #endregion
    }
}
