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
    /// 録画映像の画像を収集するための抽象クラスです。
    /// </summary>
    /// <typeparam name="TVideoSource"><see cref="VideoSource" /> のサブタイプ</typeparam>
    /// <typeparam name="TImageData">画像データの型</typeparam>
    /// <seealso cref="IVideoImageCollector&lt;TImageData&gt;" />
    [ToString]
    internal abstract class VideoImageCollector<TVideoSource, TImageData> : IVideoImageCollector<TImageData>
        where TVideoSource : VideoSource
        where TImageData : class
    {
        #region Fields

        /// <summary>
        /// アンマネージリソースが解放されたかどうかです。
        /// </summary>
        private bool disposed;

        /// <summary>
        /// 録画映像のソースです。
        /// </summary>
        private protected readonly TVideoSource videoSource;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        /// <param name="cameraFQID">カメラの完全修飾 ID</param>
        private protected VideoImageCollector(FQID cameraFQID) : this(cameraFQID.ObjectId) { }

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        /// <param name="cameraFQID">カメラの完全修飾 ID</param>
        /// <param name="imageSize">画像のサイズ</param>
        private protected VideoImageCollector(FQID cameraFQID, Size imageSize) : this(cameraFQID.ObjectId, imageSize) { }

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        /// <param name="cameraId">カメラの ID</param>
        /// <param name="imageSize">画像のサイズ</param>
        /// <exception cref="ArgumentException"><paramref name="cameraId" /> が <see cref="Guid.Empty" /> の場合にスローされます。</exception>
        private protected VideoImageCollector(Guid cameraId) : this(cameraId, Size.Empty) { }

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        /// <param name="cameraId">カメラの ID</param>
        /// <param name="imageSize">画像のサイズ</param>
        /// <exception cref="ArgumentException"><paramref name="cameraId" /> が <see cref="Guid.Empty" /> の場合にスローされます。</exception>
        private protected VideoImageCollector(Guid cameraId, Size imageSize)
        {
            if (cameraId == Guid.Empty)
            {
                throw new ArgumentException(nameof(cameraId));
            }

            var camera = Configuration.Instance.GetItem(cameraId, Kind.Camera);

            videoSource = GenerateVideoSource(camera);

            // 幅と高さに 0 を指定した場合は、実際の解像度の画像が取得される。
            videoSource.Init(imageSize.Width, imageSize.Height);
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// アンマネージリソースを解放し、必要に応じてマネージリソースも解放します。
        /// </summary>
        /// <param name="disposing">
        /// マネージリソースとアンマネージリソースの両方を解放する場合は <c>true</c>。アンマネージリソースだけを解放する場合は <c>false</c>。
        /// </param>
        private void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                videoSource?.Close();
            }

            disposed = true;
        }

        /// <summary>
        /// 録画映像のソースを生成します。
        /// </summary>
        /// <param name="camera">カメラ</param>
        /// <returns>録画映像のソース</returns>
        private protected abstract TVideoSource GenerateVideoSource(Item camera);

        /// <summary>
        /// 収集する画像のサイズを設定します。
        /// </summary>
        /// <param name="imageSize">画像のサイズ</param>
        /// <remarks>
        /// 幅と高さに 0 を指定した場合は、実際の解像度の画像が取得されます。
        /// </remarks>
        public abstract void SetImageSize(Size imageSize);

        /// <summary>
        /// 指定された日時の画像を取得します。
        /// </summary>
        /// <param name="dateTime">日時</param>
        /// <returns>画像データ</returns>
        public TImageData? GetImage(DateTime dateTime)
        {
            return (TImageData?)videoSource.Get(dateTime.ToUniversalTime());
        }

        /// <summary>
        /// 指定された日時、もしくは指定された日時以前の最も近い日時の画像を取得します。
        /// </summary>
        /// <param name="dateTime">日時</param>
        /// <returns>画像データ</returns>
        public TImageData? GetImageAtOrBefore(DateTime dateTime)
        {
            return (TImageData?)videoSource.GetAtOrBefore(dateTime.ToUniversalTime());
        }

        /// <summary>
        /// 指定された日時に最も近い日時の画像を取得します。
        /// </summary>
        /// <param name="dateTime">日時</param>
        /// <returns>画像データ</returns>
        public TImageData? GetImageNearest(DateTime dateTime)
        {
            return (TImageData?)videoSource.GetNearest(dateTime.ToUniversalTime());
        }

        /// <summary>
        /// アンマネージリソースの解放またはリセットに関連付けられているアプリケーション定義のタスクを実行します。
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion Methods
    }
}
