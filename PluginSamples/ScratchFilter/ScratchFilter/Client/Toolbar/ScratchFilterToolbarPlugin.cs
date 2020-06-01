using OpenCvSharp;
using OpenCvSharp.Extensions;
using ScratchFilter.Client.Data;
using ScratchFilter.Properties;
using System;
using System.Drawing;
using System.Windows.Forms;
using VideoOS.Platform.Client;

namespace ScratchFilter.Client.Toolbar
{
    /// <summary>
    /// ツールバーの傷フィルタ機能用インスタンスです。
    /// </summary>
    /// <seealso cref="ImageViewerToolbarPluginInstance" />
    [ToString]
    internal class ScratchFilterToolbarPluginInstance : ImageViewerToolbarPluginInstance
    {
        #region Fields

        /// <summary>
        /// 作動中であるかどうかです。
        /// </summary>
        private bool isActive;

        #endregion

        #region Properties

        /// <summary>
        /// プラグインの説明です。
        /// </summary>
        /// <value>
        /// プラグインの説明
        /// </value>
        protected override string Description
        {
            get
            {
                if (isActive)
                {
                    return Resources.ToolbarPlugin_ScratchFilter_Description_Active;
                }
                else
                {
                    return Resources.ToolbarPlugin_ScratchFilter_Description_Inactive;
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// 画像のコントラストを変更します。
        /// </summary>
        /// <param name="mat">画像</param>
        /// <param name="contrast">コントラストの変更に使用する値</param>
        private void ChangeContrast(Mat mat, float contrast)
        {
            if (mat == null)
            {
                throw new ArgumentNullException(nameof(mat));
            }

            using (Mat dst = mat * contrast)
            {
                dst.CopyTo(mat);
            }
        }

        /// <summary>
        /// 画像の明るさを変更します。
        /// </summary>
        /// <param name="mat">画像</param>
        /// <param name="brightness">明るさの変更に使用する値</param>
        private void ChangeBrightness(Mat mat, float brightness)
        {
            if (mat == null)
            {
                throw new ArgumentNullException(nameof(mat));
            }

            using (Mat dst = mat + brightness)
            {
                dst.CopyTo(mat);
            }
        }

        /// <summary>
        /// 画像の彩度を変更します。
        /// </summary>
        /// <param name="mat">画像</param>
        /// <param name="brightness">彩度の変更に使用する値</param>
        private void ChangeSaturation(Mat mat, float saturation)
        {
            if (mat == null)
            {
                throw new ArgumentNullException(nameof(mat));
            }

            Cv2.CvtColor(mat, mat, ColorConversionCodes.BGR2HSV);

            Cv2.Split(mat, out Mat[] channels);

            channels[2] = channels[2] * saturation;

            Cv2.Merge(channels, mat);

            Cv2.CvtColor(mat, mat, ColorConversionCodes.HSV2BGR);
        }

        /// <summary>
        /// 画像のガンマを変更します。
        /// </summary>
        /// <param name="mat">画像</param>
        /// <param name="brightness">ガンマの変更に使用する値</param>
        private void ChangeGamma(Mat mat, float gamma)
        {
            if (mat == null)
            {
                throw new ArgumentNullException(nameof(mat));
            }

            byte[] lut = new byte[256];

            for (int i = 0; i < lut.Length; i++)
            {
                lut[i] = (byte)(Math.Pow(i / 255.0, 1.0 / gamma) * 255.0);
            }

            Cv2.LUT(mat, lut, mat);
        }

        /// <summary>
        /// イメージビューワの表示画像を処理します。
        /// </summary>
        private void HandleDisplayedImage()
        {
            ImageViewerAddOn.ClearOverlay(default);

            if (!isActive)
            {
                return;
            }

            using (Bitmap original = ImageViewerAddOn.GetCurrentDisplayedImageAsBitmap())
            {
                if (original == null)
                {
                    return;
                }

                ScratchFilterSetting setting = ScratchFilterSettingManager.Instance.GetSetting(ImageViewerAddOn.CameraFQID.ObjectId);

                using (Mat mat = original.ToMat())
                {
                    ChangeContrast(mat, setting.ImageContrast);
                    ChangeBrightness(mat, setting.ImageBrightness);
                    ChangeSaturation(mat, setting.ImageSaturation);
                    ChangeGamma(mat, setting.ImageGamma);

                    using (Bitmap overlay = mat.ToBitmap())
                    {
                        ImageViewerAddOn.SetOverlay(overlay, default, true, true, true, 1, DockStyle.None, DockStyle.None, 0, 0);
                    }
                }
            }
        }

        /// <summary>
        /// イメージビューワにライブ映像の画像が表示された時に呼び出されます。
        /// </summary>
        /// <param name="sender">通知元</param>
        /// <param name="e">イベント引数</param>
        protected override void ImageViewerImageDisplayedEventHandler(object sender, ImageDisplayedEventArgs e)
        {
            HandleDisplayedImage();
        }

        /// <summary>
        /// イメージビューワに録画映像の画像が表示された時に呼び出されます。
        /// </summary>
        /// <param name="sender">通知元</param>
        /// <param name="e">イベント引数</param>
        protected override void ImageViewerRecordedImageReceivedEventHandler(object sender, RecordedImageReceivedEventArgs e)
        {
            HandleDisplayedImage();
        }

        /// <summary>
        /// アイコンを読み込みます。
        /// </summary>
        protected override void LoadIcon()
        {
        }

        /// <summary>
        /// ツールバー用プラグインのメニューが押下された時に呼び出されます。
        /// </summary>
        public override void Activate()
        {
            isActive = !isActive;

            Title = Description;
            Tooltip = Description;

            ReloadIcon();
        }

        #endregion
    }

    /// <summary>
    /// ツールバーの傷フィルタ機能です。
    /// </summary>
    /// <seealso cref="ImageViewerToolbarPlugin" />
    [ToString]
    internal class ScratchFilterToolbarPlugin : ImageViewerToolbarPlugin
    {
        #region Fields

        /// <summary>
        /// プラグインの ID です。
        /// </summary>
        private static readonly Guid PluginId = new Guid("655154aa-af1d-47af-b23a-09ea79eb1b9d");

        #endregion

        #region Properties

        /// <summary>
        /// ID です。
        /// </summary>
        /// <value>
        /// ID
        /// </value>
        public override Guid Id
        {
            get;
        } = PluginId;

        #endregion

        #region Methods

        /// <summary>
        /// ツールバーの傷フィルタ機能用インスタンスを生成します。
        /// </summary>
        /// <returns>ツールバーの傷フィルタ機能用インスタンス</returns>
        public override ViewItemToolbarPluginInstance GenerateViewItemToolbarPluginInstance()
        {
            return new ScratchFilterToolbarPluginInstance();
        }

        #endregion
    }
}
