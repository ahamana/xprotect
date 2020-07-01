using ImageProcessor;
using ScratchFilter.Client.Data;
using ScratchFilter.Properties;
using System;
using System.Drawing;
using System.Windows.Forms;
using VideoOS.Platform;
using VideoOS.Platform.Client;
using Point = System.Drawing.Point;

namespace ScratchFilter.Client.Toolbar
{
    /// <summary>
    /// ツールバーの傷フィルタ機能用インスタンスです。
    /// </summary>
    /// <seealso cref="ImageViewerToolbarPluginInstance" />
    [ToString]
    internal sealed class ToolbarScratchFilterInstance : ImageViewerToolbarPluginInstance
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
                    return Resources.Toolbar_ScratchFilter_Description_Active;
                }
                else
                {
                    return Resources.Toolbar_ScratchFilter_Description_Inactive;
                }
            }
        }

        #endregion

        #region Methods

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

            using Bitmap original = ImageViewerAddOn.GetCurrentDisplayedImageAsBitmap();

            if (original == null)
            {
                return;
            }

            ScratchFilterSetting setting = ScratchFilterSettingManager.Instance.GetSetting(ImageViewerAddOn.CameraFQID.ObjectId);

            using ImageFactory imageFactory = new ImageFactory();

            imageFactory.Load(original)
                        .Contrast(setting.ImageContrast)
                        .Brightness(setting.ImageBrightness)
                        .Saturation(setting.ImageSaturation)
                        .Gamma(setting.ImageGamma);

            using Bitmap overlay = new Bitmap(imageFactory.Image);

            ImageViewerAddOn.SetOverlay(overlay, default, true, true, true, 1, DockStyle.None, DockStyle.None, Point.Empty.X, Point.Empty.Y);
        }

        /// <summary>
        /// イメージビューワにライブ映像の画像が表示された時に発生します。
        /// </summary>
        /// <param name="sender">イベントハンドラがアタッチされるオブジェクト</param>
        /// <param name="e">イベントのデータ</param>
        protected override void OnImageViewerImageDisplayed(object sender, ImageDisplayedEventArgs e)
        {
            HandleDisplayedImage();
        }

        /// <summary>
        /// イメージビューワに録画映像の画像が表示された時に発生します。
        /// </summary>
        /// <param name="sender">イベントハンドラがアタッチされるオブジェクト</param>
        /// <param name="e">イベントのデータ</param>
        protected override void OnImageViewerRecordedImageReceived(object sender, RecordedImageReceivedEventArgs e)
        {
            HandleDisplayedImage();
        }

        /// <summary>
        /// アイコンを読み込みます。
        /// </summary>
        protected override void LoadIcon()
        {
            Icon = (ClientControl.Instance.Theme.ThemeType, isActive) switch
            {
                (ThemeType.Dark, true) => Resources.Toolbar_ScratchFilter_Icon_Dark_Active,
                (ThemeType.Dark, false) => Resources.Toolbar_ScratchFilter_Icon_Dark_Inactive,
                (ThemeType.Light, true) => Resources.Toolbar_ScratchFilter_Icon_Light_Active,
                (ThemeType.Light, false) => Resources.Toolbar_ScratchFilter_Icon_Light_Inactive,
                _ => default
            };
        }

        /// <summary>
        /// ツールバー用プラグインのメニューが押下された時に発生します。
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
    internal sealed class ToolbarScratchFilter : ImageViewerToolbarPlugin
    {
        #region Fields

        /// <summary>
        /// プラグインの ID です。
        /// </summary>
        private static readonly Guid PluginId = Guid.Parse("655154aa-af1d-47af-b23a-09ea79eb1b9d");

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
        /// <returns>
        /// ツールバーの傷フィルタ機能用インスタンス
        /// </returns>
        public override ViewItemToolbarPluginInstance GenerateViewItemToolbarPluginInstance()
        {
            return new ToolbarScratchFilterInstance();
        }

        #endregion
    }
}
