//
// (C) 2020 Canon Inc.
//
// This file contains trade secrets of Canon Inc. No part may be reproduced
// or transmitted in any form by any means or for any purpose without the
// express written permission of Canon Inc.
//

namespace ImageStore.Client.Data
{
    /// <summary>
    /// 設定です。
    /// </summary>
    public sealed class ImageStoreSettings
    {
        #region Properties

        /// <summary>
        /// JPEG ファイルを出力するディレクトリです。
        /// </summary>
        /// <value>JPEG ファイルを出力するディレクトリ</value>
        public string OutputDir { get; set; } = string.Empty;

        /// <summary>
        /// 出力する JPEG ファイルの圧縮です。
        /// </summary>
        /// <value>出力する JPEG ファイルの圧縮</value>
        public ushort Compression { get; set; }

        #endregion Properties
    }
}
