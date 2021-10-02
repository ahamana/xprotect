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
    /// <seealso cref="LiveImageCollector{BitmapLiveSource, LiveSourceBitmapContent}" />
    [ToString]
    internal sealed class LiveBitmapCollector : LiveImageCollector<BitmapLiveSource, LiveSourceBitmapContent>
    {
        #region Constructors

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        /// <param name="cameraFQID">カメラの完全修飾 ID</param>
        internal LiveBitmapCollector(FQID cameraFQID) : base(cameraFQID) { }

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        /// <param name="cameraFQID">カメラの完全修飾 ID</param>
        /// <param name="imageSize">画像のサイズ</param>
        /// <remarks>
        /// 画像のサイズの幅と高さに 0 を指定した場合は、実際の解像度の画像が収集されます。
        /// </remarks>
        internal LiveBitmapCollector(FQID cameraFQID, Size imageSize) : base(cameraFQID, imageSize) { }

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        /// <param name="cameraId">カメラの ID</param>
        internal LiveBitmapCollector(Guid cameraId) : base(cameraId) { }

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        /// <param name="cameraId">カメラの ID</param>
        /// <param name="imageSize">画像のサイズ</param>
        /// <remarks>
        /// 画像のサイズの幅と高さに 0 を指定した場合は、実際の解像度の画像が収集されます。
        /// </remarks>
        internal LiveBitmapCollector(Guid cameraId, Size imageSize) : base(cameraId, imageSize) { }

        #endregion Constructors

        #region Methods

        /// <inheritdoc />
        private protected sealed override BitmapLiveSource GenerateVideoLiveSource(Item camera) =>
            new(camera, BitmapFormat.BGR24);

        /// <inheritdoc />
        private protected sealed override Stream GenerateImageStream(LiveSourceBitmapContent liveContent)
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
