//
// (C) 2020 Canon Inc.
//
// This file contains trade secrets of Canon Inc. No part may be reproduced
// or transmitted in any form by any means or for any purpose without the
// express written permission of Canon Inc.
//

using System;

using Prism.Mvvm;

using VideoOS.Platform;

namespace ScratchFilter.Client.Data
{
    /// <summary>
    /// 傷フィルタの設定です。
    /// </summary>
    /// <seealso cref="BindableBase" />
    [ToString]
    internal sealed class ScratchFilterSetting : BindableBase
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

        /// <summary>
        /// 画像のコントラストです。
        /// </summary>
        private int imageContrast = DefaultImageContrast;

        /// <summary>
        /// 画像の明るさです。
        /// </summary>
        private int imageBrightness = DefaultImageBrightness;

        /// <summary>
        /// 画像の彩度です。
        /// </summary>
        private int imageSaturation = DefaultImageSaturation;

        /// <summary>
        /// 画像のガンマです。
        /// </summary>
        private float imageGamma = DefaultImageGamma;

        #endregion Fields

        #region Properties

        /// <summary>
        /// カメラの ID です。
        /// </summary>
        /// <value>カメラの ID</value>
        public Guid CameraId { get; set; }

        /// <summary>
        /// カメラ名です。
        /// </summary>
        /// <value>カメラ名</value>
        public string CameraName
        {
            get
            {
                var camera = Configuration.Instance.GetItem(CameraId, Kind.Camera);

                return camera?.Name;
            }
        }

        /// <summary>
        /// 画像のコントラストです。
        /// </summary>
        /// <value>画像のコントラスト</value>
        public int ImageContrast
        {
            get => imageContrast;
            set => SetProperty(ref imageContrast, value);
        }

        /// <summary>
        /// 画像の明るさです。
        /// </summary>
        /// <value>画像の明るさ</value>
        public int ImageBrightness
        {
            get => imageBrightness;
            set => SetProperty(ref imageBrightness, value);
        }

        /// <summary>
        /// 画像の彩度です。
        /// </summary>
        /// <value>画像の彩度</value>
        public int ImageSaturation
        {
            get => imageSaturation;
            set => SetProperty(ref imageSaturation, value);
        }

        /// <summary>
        /// 画像のガンマです。
        /// </summary>
        /// <value>画像のガンマ</value>
        public float ImageGamma
        {
            get => imageGamma;
            set => SetProperty(ref imageGamma, value);
        }

        #endregion Properties
    }
}
