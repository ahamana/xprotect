using System;
using VideoOS.Platform;

namespace ScratchFilter.Client.Data
{
    /// <summary>
    /// 傷フィルタの設定です。
    /// </summary>
    [ToString]
    internal sealed class ScratchFilterSetting
    {
        #region Fields

        /// <summary>
        /// デフォルトの画像のコントラストです。
        /// </summary>
        private static readonly int DefaultImageContrast = 0;

        /// <summary>
        /// デフォルトの画像の明るさです。
        /// </summary>
        private static readonly int DefaultImageBrightness = 0;

        /// <summary>
        /// デフォルトの画像の彩度です。
        /// </summary>
        private static readonly int DefaultImageSaturation = 0;

        /// <summary>
        /// デフォルトの画像のガンマです。
        /// </summary>
        private static readonly float DefaultImageGamma = 1;

        #endregion

        #region Properties

        /// <summary>
        /// カメラの ID です。
        /// </summary>
        /// <value>
        /// カメラの ID
        /// </value>
        public Guid CameraId { get; set; }

        /// <summary>
        /// カメラ名です。
        /// </summary>
        /// <value>
        /// カメラ名
        /// </value>
        public string CameraName
        {
            get
            {
                Item camera = Configuration.Instance.GetItem(CameraId, Kind.Camera);

                return camera?.Name;
            }
        }

        /// <summary>
        /// 画像のコントラストです。
        /// </summary>
        /// <value>
        /// 画像のコントラスト
        /// </value>
        public int ImageContrast { get; set; } = DefaultImageContrast;

        /// <summary>
        /// 画像の明るさです。
        /// </summary>
        /// <value>
        /// 画像の明るさ
        /// </value>
        public int ImageBrightness { get; set; } = DefaultImageBrightness;

        /// <summary>
        /// 画像の彩度です。
        /// </summary>
        /// <value>
        /// 画像の彩度
        /// </value>
        public int ImageSaturation { get; set; } = DefaultImageSaturation;

        /// <summary>
        /// 画像のガンマです。
        /// </summary>
        /// <value>
        /// 画像のガンマ
        /// </value>
        public float ImageGamma { get; set; } = DefaultImageGamma;

        #endregion
    }
}
