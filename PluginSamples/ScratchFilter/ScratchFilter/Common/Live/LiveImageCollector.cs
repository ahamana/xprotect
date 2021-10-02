//
// (C) 2020 Canon Inc.
//
// This file contains trade secrets of Canon Inc. No part may be reproduced
// or transmitted in any form by any means or for any purpose without the
// express written permission of Canon Inc.
//

using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

using VideoOS.Platform;
using VideoOS.Platform.Live;

namespace ScratchFilter.Common.Live
{
    /// <summary>
    /// ライブ映像の画像を収集するための抽象クラスです。
    /// </summary>
    /// <typeparam name="TVideoLiveSource"><see cref="VideoLiveSource" /> のサブタイプ</typeparam>
    /// <typeparam name="TLiveSourceContent"><see cref="LiveSourceContent" /> のサブタイプ</typeparam>
    /// <seealso cref="ILiveImageCollector" />
    [ToString]
    internal abstract class LiveImageCollector<TVideoLiveSource, TLiveSourceContent> : ILiveImageCollector
        where TVideoLiveSource : VideoLiveSource
        where TLiveSourceContent : LiveSourceContent
    {
        #region Fields

        /// <summary>
        /// 映像を取得する際のタイムアウト期間です。
        /// </summary>
        private static readonly TimeSpan VideoSourceTimeout = TimeSpan.FromSeconds(5);

        /// <summary>
        /// ライブ映像のソースです。
        /// </summary>
        private readonly TVideoLiveSource videoLiveSource;

        /// <summary>
        /// アンマネージリソースが解放されたかどうかです。
        /// </summary>
        private bool isDisposed;

        /// <summary>
        /// 画像のストリームです。
        /// </summary>
        private Stream? imageStream;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        /// <param name="cameraFQID">カメラの完全修飾 ID</param>
        private protected LiveImageCollector(FQID cameraFQID) : this(cameraFQID.ObjectId) { }

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        /// <param name="cameraFQID">カメラの完全修飾 ID</param>
        /// <param name="imageSize">画像のサイズ</param>
        /// <remarks>
        /// 画像のサイズの幅と高さに 0 を指定した場合は、実際の解像度の画像が収集されます。
        /// </remarks>
        private protected LiveImageCollector(FQID cameraFQID, Size imageSize) : this(cameraFQID.ObjectId, imageSize) { }

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        /// <param name="cameraId">カメラの ID</param>
        /// <exception cref="ArgumentException"><paramref name="cameraId" /> が <see cref="Guid.Empty" /> の場合にスローされます。</exception>
        private protected LiveImageCollector(Guid cameraId) : this(cameraId, Size.Empty) { }

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        /// <param name="cameraId">カメラの ID</param>
        /// <param name="imageSize">画像のサイズ</param>
        /// <exception cref="ArgumentException"><paramref name="cameraId" /> が <see cref="Guid.Empty" /> の場合にスローされます。</exception>
        /// <remarks>
        /// 画像のサイズの幅と高さに 0 を指定した場合は、実際の解像度の画像が収集されます。
        /// </remarks>
        private protected LiveImageCollector(Guid cameraId, Size imageSize)
        {
            if (cameraId == Guid.Empty)
            {
                throw new ArgumentException(nameof(cameraId));
            }

            var camera = Configuration.Instance.GetItem(cameraId, Kind.Camera);

            videoLiveSource = GenerateVideoLiveSource(camera);

            videoLiveSource.LiveModeStart = true;
            // 幅と高さに 0 を指定した場合は、実際の解像度の画像が取得される。
            videoLiveSource.Width = imageSize.Width;
            videoLiveSource.Height = imageSize.Height;

            videoLiveSource.LiveContentEvent += OnLiveSourceLiveContentEvent;

            videoLiveSource.Init();
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// ライブ映像の新しいフレームを取得した時に発生します。
        /// </summary>
        /// <param name="sender">イベントハンドラがアタッチされるオブジェクト</param>
        /// <param name="e">イベントのデータ</param>
        private void OnLiveSourceLiveContentEvent(object sender, EventArgs e)
        {
            if (e is not LiveContentEventArgs { LiveContent: not null } args)
            {
                return;
            }

            lock (this)
            {
                using var liveContent = (TLiveSourceContent)args.LiveContent;

                imageStream?.Dispose();
                imageStream = GenerateImageStream(liveContent);
            }
        }

        /// <summary>
        /// アンマネージリソースを解放し、必要に応じてマネージリソースも解放します。
        /// </summary>
        /// <param name="disposing">マネージリソースとアンマネージリソースの両方を解放する場合は <c>true</c>。アンマネージリソースだけを解放する場合は <c>false</c>。</param>
        private void Dispose(bool disposing)
        {
            if (isDisposed)
            {
                return;
            }

            if (disposing)
            {
                videoLiveSource.Close();
                imageStream?.Dispose();
            }

            isDisposed = true;
        }

        /// <summary>
        /// ライブ映像のソースを生成します。
        /// </summary>
        /// <param name="camera">カメラ</param>
        /// <returns>ライブ映像のソース</returns>
        private protected abstract TVideoLiveSource GenerateVideoLiveSource(Item camera);

        /// <summary>
        /// 画像のストリームを生成します。
        /// </summary>
        /// <param name="liveContent">ライブ映像の内容</param>
        /// <returns>画像のストリーム</returns>
        private protected abstract Stream GenerateImageStream(TLiveSourceContent liveContent);

        /// <inheritdoc cref="ILiveImageCollector.GetImage" />
        public Bitmap? GetImage()
        {
            var stopwatch = Stopwatch.StartNew();

            while (imageStream is null && stopwatch.Elapsed < VideoSourceTimeout)
            {
                Task.Delay(TimeSpan.FromTicks(TimeSpan.TicksPerMillisecond)).Wait();
            }

            lock (this)
            {
                if (imageStream is null)
                {
                    return null;
                }

                using var image = Image.FromStream(imageStream);

                return new(image);
            }
        }

        /// <inheritdoc cref="IDisposable.Dispose" />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion Methods
    }
}
