using System;
using System.IO;
using VideoOS.Platform;
using VideoOS.Platform.Live;

namespace ScratchFilter.Live
{
    /// <summary>
    /// JPEG 画像を収集するためのクラスです。
    /// </summary>
    /// <seealso cref="ImageCollector{JPEGLiveSource, LiveSourceContent}" />
    [ToString]
    internal sealed class JPEGCollector : ImageCollector<JPEGLiveSource, LiveSourceContent>
    {
        #region Constructors

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        /// <param name="cameraFQID">カメラの完全修飾 ID</param>
        internal JPEGCollector(FQID cameraFQID) : base(cameraFQID)
        {
        }

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        /// <param name="cameraId">カメラの ID</param>
        internal JPEGCollector(Guid cameraId) : base(cameraId)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// ライブ映像のソースを生成します。
        /// </summary>
        /// <param name="camera">カメラ</param>
        /// <returns>
        /// ライブ映像のソース
        /// </returns>
        protected override JPEGLiveSource GenerateVideoLiveSource(Item camera)
        {
            return new JPEGLiveSource(camera);
        }

        /// <summary>
        /// 画像のストリームを生成します。
        /// </summary>
        /// <param name="liveContent">ライブ映像の内容</param>
        /// <returns>
        /// 画像のストリーム
        /// </returns>
        protected override Stream GenerateImageStream(LiveSourceContent liveContent)
        {
            return new MemoryStream(liveContent.Content);
        }

        #endregion
    }
}
