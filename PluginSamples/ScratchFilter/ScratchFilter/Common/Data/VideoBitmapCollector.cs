//
// (C) 2020 Canon Inc.
//
// This file contains trade secrets of Canon Inc. No part may be reproduced
// or transmitted in any form by any means or for any purpose without the
// express written permission of Canon Inc.
//

using System;
using System.Drawing;

using VideoOS.Platform;
using VideoOS.Platform.Data;

namespace ScratchFilter.Common.Data
{
    /// <summary>
    /// Bitmap 画像を収集するためのクラスです。
    /// </summary>
    /// <seealso cref="VideoImageCollector&lt;BitmapVideoSource, BitmapData&gt;" />
    [ToString]
    internal sealed class VideoBitmapCollector : VideoImageCollector<BitmapVideoSource, BitmapData>
    {
        #region Constructors

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        /// <param name="cameraFQID">カメラの完全修飾 ID</param>
        internal VideoBitmapCollector(FQID cameraFQID) : base(cameraFQID) { }

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        /// <param name="cameraFQID">カメラの完全修飾 ID</param>
        /// <param name="imageSize">画像のサイズ</param>
        internal VideoBitmapCollector(FQID cameraFQID, Size imageSize) : base(cameraFQID, imageSize) { }

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        /// <param name="cameraId">カメラの ID</param>
        internal VideoBitmapCollector(Guid cameraId) : base(cameraId) { }

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        /// <param name="cameraId">カメラの ID</param>
        /// <param name="imageSize">画像のサイズ</param>
        internal VideoBitmapCollector(Guid cameraId, Size imageSize) : base(cameraId, imageSize) { }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// 録画映像のソースを生成します。
        /// </summary>
        /// <param name="camera">カメラ</param>
        /// <returns>録画映像のソース</returns>
        private protected sealed override BitmapVideoSource GenerateVideoSource(Item camera)
        {
            return new(camera);
        }

        /// <summary>
        /// 収集する画像のサイズを設定します。
        /// </summary>
        /// <param name="imageSize">画像のサイズ</param>
        /// <remarks>
        /// 幅と高さに 0 を指定した場合は、実際の解像度の画像が取得されます。
        /// </remarks>
        public sealed override void SetImageSize(Size imageSize)
        {
            videoSource.Width = imageSize.Width;
            videoSource.Height = imageSize.Height;

            videoSource.SetWidthHeight();
        }

        #endregion Methods
    }
}
