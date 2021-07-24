//
// (C) 2020 Canon Inc.
//
// This file contains trade secrets of Canon Inc. No part may be reproduced
// or transmitted in any form by any means or for any purpose without the
// express written permission of Canon Inc.
//

using System;
using System.Drawing;
using System.IO;

using VideoOS.Platform;
using VideoOS.Platform.Live;

namespace ScratchFilter.Common.Live
{
    /// <summary>
    /// ライブ映像の JPEG 画像を収集するためのクラスです。
    /// </summary>
    /// <seealso cref="LiveImageCollector&lt;JPEGLiveSource, LiveSourceContent&gt;" />
    [ToString]
    internal sealed class LiveJPEGCollector : LiveImageCollector<JPEGLiveSource, LiveSourceContent>
    {
        #region Constructors

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        /// <param name="cameraFQID">カメラの完全修飾 ID</param>
        internal LiveJPEGCollector(FQID cameraFQID) : base(cameraFQID) { }

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        /// <param name="cameraFQID">カメラの完全修飾 ID</param>
        /// <param name="imageSize">画像のサイズ</param>
        internal LiveJPEGCollector(FQID cameraFQID, Size imageSize) : base(cameraFQID, imageSize) { }

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        /// <param name="cameraId">カメラの ID</param>
        internal LiveJPEGCollector(Guid cameraId) : base(cameraId) { }

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        /// <param name="cameraId">カメラの ID</param>
        /// <param name="imageSize">画像のサイズ</param>
        internal LiveJPEGCollector(Guid cameraId, Size imageSize) : base(cameraId, imageSize) { }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// ライブ映像のソースを生成します。
        /// </summary>
        /// <param name="camera">カメラ</param>
        /// <returns>ライブ映像のソース</returns>
        private protected sealed override JPEGLiveSource GenerateVideoLiveSource(Item camera)
        {
            return new(camera)
            {
                // 無圧縮の画像を取得
                Compression = 100
            };
        }

        /// <summary>
        /// 画像のストリームを生成します。
        /// </summary>
        /// <param name="liveContent">ライブ映像の内容</param>
        /// <returns>画像のストリーム</returns>
        private protected sealed override Stream GenerateImageStream(LiveSourceContent liveContent)
        {
            return new MemoryStream(liveContent.Content);
        }

        #endregion Methods
    }
}
