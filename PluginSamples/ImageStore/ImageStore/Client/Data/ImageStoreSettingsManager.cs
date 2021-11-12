//
// (C) 2020 Canon Inc.
//
// This file contains trade secrets of Canon Inc. No part may be reproduced
// or transmitted in any form by any means or for any purpose without the
// express written permission of Canon Inc.
//

using System.IO;
using System.Reflection;
using System.Xml.Serialization;

namespace ImageStore.Client.Data;

/// <summary>
/// 設定のマネージャです。
/// </summary>
internal sealed class ImageStoreSettingsManager
{
    #region Fields

    /// <summary>
    /// 傷フィルタの設定ファイルです。
    /// </summary>
    private static readonly string SettingsFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                                                               $"{nameof(ImageStoreSettings)}.xml");

    #endregion Fields

    #region Constructors

    /// <summary>
    /// コンストラクタです。
    /// </summary>
    private ImageStoreSettingsManager() { }

    #endregion Constructors

    #region Properties

    /// <summary>
    /// インスタンスです。
    /// </summary>
    /// <value>インスタンス</value>
    internal static ImageStoreSettingsManager Instance { get; } = new();

    #endregion Properties

    #region Methods

    /// <summary>
    /// 設定を読み込みます。
    /// </summary>
    /// <returns>設定</returns>
    internal ImageStoreSettings Load()
    {
        var xmlSerializer = new XmlSerializer(typeof(ImageStoreSettings));

        using var reader = File.OpenText(SettingsFile);

        return (ImageStoreSettings)xmlSerializer.Deserialize(reader);
    }

    /// <summary>
    /// 設定を保存します。
    /// </summary>
    /// <param name="settings">設定</param>
    internal void Save(ImageStoreSettings settings)
    {
        // デフォルトの名前空間の出力制御
        var namespaces = new XmlSerializerNamespaces();
        namespaces.Add(string.Empty, string.Empty);

        var xmlSerializer = new XmlSerializer(typeof(ImageStoreSettings));

        using var writer = File.CreateText(SettingsFile);

        xmlSerializer.Serialize(writer, settings, namespaces);
    }

    #endregion Methods
}
