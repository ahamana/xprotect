using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using VideoOS.Platform;
using VideoOS.Platform.Live;

namespace ScratchFilter.Live
{
    /// <summary>
    /// Bitmap 画像を収集するためのクラスです。
    /// </summary>
    /// <seealso cref="ImageCollector{BitmapLiveSource, LiveSourceBitmapContent}" />
    internal sealed class BitmapCollector : ImageCollector<BitmapLiveSource, LiveSourceBitmapContent>
    {
        #region Constructors

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        /// <param name="cameraFQID">カメラの完全修飾 ID</param>
        internal BitmapCollector(FQID cameraFQID) : base(cameraFQID)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// ライブ映像のソースを生成します。
        /// </summary>
        /// <param name="camera">カメラ</param>
        /// <returns>ライブ映像のソース</returns>
        protected override BitmapLiveSource GenerateVideoLiveSource(Item camera)
        {
            return new BitmapLiveSource(camera, BitmapFormat.BGR24);
        }

        /// <summary>
        /// 画像のストリームを生成します。
        /// </summary>
        /// <param name="liveContent">ライブ映像の内容</param>
        /// <returns>画像のストリーム</returns>
        protected override Stream GenerateImageStream(LiveSourceBitmapContent liveContent)
        {
            Stream stream = new MemoryStream();

            using (Image image = new Bitmap(liveContent.GetPlaneWidth(0), liveContent.GetPlaneHeight(0),
                                            liveContent.GetPlaneStride(0), PixelFormat.Format24bppRgb,
                                            liveContent.GetPlanePointer(0)))
            {
                image.Save(stream, ImageFormat.Bmp);
            }

            return stream;
        }

        #endregion
    }
}
