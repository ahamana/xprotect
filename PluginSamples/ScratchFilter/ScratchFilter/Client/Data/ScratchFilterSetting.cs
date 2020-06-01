using System;

namespace ScratchFilter.Client.Data
{
    /// <summary>
    /// 傷フィルタの設定です。
    /// </summary>
    [ToString]
    public class ScratchFilterSetting
    {
        /// <summary>
        /// カメラの ID です。
        /// </summary>
        /// <value>
        /// カメラの ID
        /// </value>
        public Guid CameraId
        {
            get;
            set;
        }

        /// <summary>
        /// 画像のコントラストです。
        /// </summary>
        /// <value>
        /// 画像のコントラスト
        /// </value>
        public float ImageContrast
        {
            get;
            set;
        } = 1;

        /// <summary>
        /// 画像の明るさです。
        /// </summary>
        /// <value>
        /// 画像の明るさ
        /// </value>
        public float ImageBrightness
        {
            get;
            set;
        } = 1;

        /// <summary>
        /// 画像の色相です。
        /// </summary>
        /// <value>
        /// 画像の色相
        /// </value>
        public float ImageHue
        {
            get;
            set;
        }

        /// <summary>
        /// 画像の彩度です。
        /// </summary>
        /// <value>
        /// 画像の彩度
        /// </value>
        public float ImageSaturation
        {
            get;
            set;
        } = 1;

        /// <summary>
        /// 画像のガンマです。
        /// </summary>
        /// <value>
        /// 画像のガンマ
        /// </value>
        public float ImageGamma
        {
            get;
            set;
        } = 1;
    }
}
