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
    /// 画像を収集するための抽象クラスです。
    /// </summary>
    /// <typeparam name="TVideoLiveSource"><see cref="VideoLiveSource" /> のサブタイプ</typeparam>
    /// <typeparam name="TLiveSourceContent"><see cref="LiveSourceContent" /> のサブタイプ</typeparam>
    /// <seealso cref="IImageCollector" />
    [ToString]
    internal abstract class ImageCollector<TVideoLiveSource, TLiveSourceContent> : IImageCollector
        where TVideoLiveSource : VideoLiveSource
        where TLiveSourceContent : LiveSourceContent
    {
        #region Fields

        /// <summary>
        /// 映像を取得する際のタイムアウト期間です。
        /// </summary>
        private static readonly TimeSpan VideoSourceTimeout = TimeSpan.FromSeconds(5);

        /// <summary>
        /// アンマネージリソースが解放されたかどうかです。
        /// </summary>
        private bool disposed;

        /// <summary>
        /// ライブ映像のソースです。
        /// </summary>
        private TVideoLiveSource liveSource;

        /// <summary>
        /// 画像のストリームです。
        /// </summary>
        private Stream imageStream;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        /// <param name="cameraFQID">カメラの完全修飾 ID</param>
        /// <exception cref="ArgumentNullException"><paramref name="cameraFQID" /> が <c>null</c> の場合にスローされます。</exception>
        protected ImageCollector(FQID cameraFQID)
        {
            if (cameraFQID == null)
            {
                throw new ArgumentNullException(nameof(cameraFQID));
            }

            Init(Configuration.Instance.GetItem(cameraFQID));
        }

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        /// <param name="cameraId">カメラの ID</param>
        /// <exception cref="ArgumentException"><paramref name="cameraId" /> が <see cref="Guid.Empty" /> の場合にスローされます。</exception>
        protected ImageCollector(Guid cameraId)
        {
            if (cameraId == Guid.Empty)
            {
                throw new ArgumentException(nameof(cameraId));
            }

            Init(Configuration.Instance.GetItem(cameraId, Kind.Camera));
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
            LiveContentEventArgs args = e as LiveContentEventArgs;

            if (args == null || args.LiveContent == null)
            {
                return;
            }

            lock (this)
            {
                using TLiveSourceContent liveContent = (TLiveSourceContent)args.LiveContent;

                imageStream?.Dispose();
                imageStream = GenerateImageStream(liveContent);
            }
        }

        /// <summary>
        /// 初期化処理を行います。
        /// </summary>
        /// <param name="camera">カメラ</param>
        /// <exception cref="ArgumentNullException"><paramref name="camera" /> が <c>null</c> の場合にスローされます。</exception>
        private void Init(Item camera)
        {
            if (camera == null)
            {
                throw new ArgumentNullException(nameof(camera));
            }

            liveSource = GenerateVideoLiveSource(camera);

            liveSource.LiveModeStart = true;
            // 幅と高さに 0 を指定して、実際の解像度の画像を取得
            liveSource.Width = 0;
            liveSource.Height = 0;

            liveSource.LiveContentEvent += OnLiveSourceLiveContentEvent;

            liveSource.Init();
        }

        /// <summary>
        /// アンマネージリソースを解放し、必要に応じてマネージリソースも解放します。
        /// </summary>
        /// <param name="disposing">マネージリソースとアンマネージリソースの両方を解放する場合は <c>true</c>。アンマネージリソースだけを解放する場合は <c>false</c>。</param>
        private void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                liveSource?.Close();
                imageStream?.Dispose();
            }

            disposed = true;
        }

        /// <summary>
        /// ライブ映像のソースを生成します。
        /// </summary>
        /// <param name="camera">カメラ</param>
        /// <returns>ライブ映像のソース</returns>
        protected abstract TVideoLiveSource GenerateVideoLiveSource(Item camera);

        /// <summary>
        /// 画像のストリームを生成します。
        /// </summary>
        /// <param name="liveContent">ライブ映像の内容</param>
        /// <returns>画像のストリーム</returns>
        protected abstract Stream GenerateImageStream(TLiveSourceContent liveContent);

        /// <summary>
        /// 画像を取得します。
        /// </summary>
        /// <returns>画像</returns>
        public Bitmap GetImage()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            while (imageStream == null && stopwatch.Elapsed < VideoSourceTimeout)
            {
                Task.Delay(TimeSpan.FromTicks(TimeSpan.TicksPerMillisecond)).Wait();
            }

            lock (this)
            {
                if (imageStream == null)
                {
                    return null;
                }

                using Image image = Image.FromStream(imageStream);

                return new Bitmap(image);
            }
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
