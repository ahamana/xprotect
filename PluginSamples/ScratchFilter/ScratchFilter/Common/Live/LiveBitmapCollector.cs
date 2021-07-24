//
// (C) 2020 Canon Inc.
//
// This file contains trade secrets of Canon Inc. No part may be reproduced
// or transmitted in any form by any means or for any purpose without the
// express written permission of Canon Inc.
//

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

using VideoOS.Platform;
using VideoOS.Platform.Live;

namespace ScratchFilter.Common.Live
{
    /// <summary>
    /// ライブ映像の Bitmap 画像を収集するためのクラスです。
    /// </summary>
    /// <seealso cref="LiveImageCollector&lt;BitmapLiveSource, LiveSourceBitmapContent&gt;" />
    [ToString]
    internal sealed class LiveBitmapCollector : LiveImageCollector<BitmapLiveSource, LiveSourceBitmapContent>
    {
        #region Constructors

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        /// <param name="cameraFQID">カメラの完全修飾 ID</param>
        internal LiveBitmapCollector(in FQID cameraFQID) : base(cameraFQID) { }

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        /// <param name="cameraFQID">カメラの完全修飾 ID</param>
        /// <param name="imageSize">画像のサイズ</param>
        internal LiveBitmapCollector(in FQID cameraFQID, in Size imageSize) : base(cameraFQID, imageSize) { }

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        /// <param name="cameraId">カメラの ID</param>
        internal LiveBitmapCollector(in Guid cameraId) : base(cameraId) { }

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        /// <param name="cameraId">カメラの ID</param>
        /// <param name="imageSize">画像のサイズ</param>
        internal LiveBitmapCollector(in Guid cameraId, in Size imageSize) : base(cameraId, imageSize) { }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// ライブ映像のソースを生成します。
        /// </summary>
        /// <param name="camera">カメラ</param>
        /// <returns>ライブ映像のソース</returns>
        private protected sealed override BitmapLiveSource GenerateVideoLiveSource(in Item camera)
        {
            return new(camera, BitmapFormat.BGR24);
        }

        /// <summary>
        /// 画像のストリームを生成します。
        /// </summary>
        /// <param name="liveContent">ライブ映像の内容</param>
        /// <returns>画像のストリーム</returns>
        private protected sealed override Stream GenerateImageStream(in LiveSourceBitmapContent liveContent)
        {
            var stream = new MemoryStream();

            using var image = new Bitmap(liveContent.GetPlaneWidth(0), liveContent.GetPlaneHeight(0),
                                         liveContent.GetPlaneStride(0), PixelFormat.Format24bppRgb,
                                         liveContent.GetPlanePointer(0));

            image.Save(stream, ImageFormat.Bmp);

            return stream;
        }

        #endregion Methods
    }
}
