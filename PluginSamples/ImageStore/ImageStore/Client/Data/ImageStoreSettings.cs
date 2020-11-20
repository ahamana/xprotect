//
// (C) 2020 Canon Inc.
//
// This file contains trade secrets of Canon Inc. No part may be reproduced
// or transmitted in any form by any means or for any purpose without the
// express written permission of Canon Inc.
//

using System.IO;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace ImageStore.Client.Data
{
    /// <summary>
    /// 設定です。
    /// </summary>
    internal sealed class ImageStoreSettings
    {
        #region Fields

        /// <summary>
        /// 設定ファイルのパスです。
        /// </summary>
        private string filePath;

        #endregion Fields

        #region Properties

        /// <summary>
        /// JPEG ファイルを出力するディレクトリです。
        /// </summary>
        /// <value>JPEG ファイルを出力するディレクトリ</value>
        public string OutputDir { get; set; }

        /// <summary>
        /// 出力する JPEG ファイルの圧縮です。
        /// </summary>
        /// <value>出力する JPEG ファイルの圧縮</value>
        public ushort Compression { get; set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// 設定を読み込みます。
        /// </summary>
        /// <returns>設定</returns>
        internal static ImageStoreSettings Load()
        {
            var directoryPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var filePath = Path.Combine(directoryPath, $"{nameof(ImageStoreSettings)}.xml");

            var xmlSerializer = new XmlSerializer(typeof(ImageStoreSettings));

            using var reader = new StreamReader(filePath, Encoding.UTF8);

            var settings = (ImageStoreSettings)xmlSerializer.Deserialize(reader);
            settings.filePath = filePath;

            return settings;
        }

        /// <summary>
        /// 設定を保存します。
        /// </summary>
        internal void Save()
        {
            // デフォルトの名前空間の出力制御
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            var xmlSerializer = new XmlSerializer(typeof(ImageStoreSettings));

            using var writer = new StreamWriter(filePath, false, Encoding.UTF8);

            xmlSerializer.Serialize(writer, this, namespaces);
        }

        #endregion Methods
    }
}
