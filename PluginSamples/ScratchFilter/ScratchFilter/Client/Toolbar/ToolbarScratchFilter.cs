//
// (C) 2020 Canon Inc.
//
// This file contains trade secrets of Canon Inc. No part may be reproduced
// or transmitted in any form by any means or for any purpose without the
// express written permission of Canon Inc.
//

using System;
using System.Drawing;
using System.Windows.Forms;

using ImageProcessor;

using ScratchFilter.Client.Data;
using ScratchFilter.Properties;

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

        #endregion Fields

        #region Properties

        /// <summary>
        /// プラグインの説明です。
        /// </summary>
        /// <value>プラグインの説明</value>
        private protected sealed override string Description =>
            isActive switch
            {
                true => Resources.Toolbar_ScratchFilter_Description_Active,
                false => Resources.Toolbar_ScratchFilter_Description_Inactive
            };

        #endregion Properties

        #region Methods

        /// <summary>
        /// イメージビューワの表示画像を処理します。
        /// </summary>
        /// <param name="imageViewerAddOn">イメージビューワのアドオン</param>
        private void HandleDisplayedImage(ImageViewerAddOn imageViewerAddOn)
        {
            imageViewerAddOn.ClearOverlay(default);

            if (!isActive)
            {
                return;
            }

            using var original = imageViewerAddOn.GetCurrentDisplayedImageAsBitmap();

            if (original is null)
            {
                return;
            }

            var setting = ScratchFilterSettingManager.Instance.GetSetting(imageViewerAddOn.CameraFQID.ObjectId);

            using var imageFactory = new ImageFactory();

            imageFactory.Load(original)
                        .Contrast(setting.ImageContrast)
                        .Brightness(setting.ImageBrightness)
                        .Saturation(setting.ImageSaturation)
                        .Gamma(setting.ImageGamma);

            using var overlay = new Bitmap(imageFactory.Image);

            imageViewerAddOn.SetOverlay(overlay, default, true, true, true, 1, DockStyle.None, DockStyle.None, Point.Empty.X, Point.Empty.Y);
        }

        /// <summary>
        /// イメージビューワにライブ映像の画像が表示された時に発生します。
        /// </summary>
        /// <param name="imageViewerAddOn">イメージビューワのアドオン</param>
        /// <param name="e">イベントのデータ</param>
        private protected sealed override void OnImageViewerImageDisplayed(ImageViewerAddOn imageViewerAddOn, ImageDisplayedEventArgs e)
        {
            HandleDisplayedImage(imageViewerAddOn);
        }

        /// <summary>
        /// イメージビューワに録画映像の画像が表示された時に発生します。
        /// </summary>
        /// <param name="imageViewerAddOn">イメージビューワのアドオン</param>
        /// <param name="e">イベントのデータ</param>
        private protected sealed override void OnImageViewerRecordedImageReceived(ImageViewerAddOn imageViewerAddOn, RecordedImageReceivedEventArgs e)
        {
            HandleDisplayedImage(imageViewerAddOn);
        }

        /// <summary>
        /// アイコンを読み込みます。
        /// </summary>
        private protected sealed override void LoadIcon()
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
        public sealed override void Activate()
        {
            isActive = !isActive;

            Title = Description;
            Tooltip = Description;

            ReloadIcon();
        }

        #endregion Methods
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

        #endregion Fields

        #region Properties

        /// <summary>
        /// ID です。
        /// </summary>
        /// <value>ID</value>
        public sealed override Guid Id { get; } = PluginId;

        #endregion Properties

        #region Methods

        /// <summary>
        /// ツールバーの傷フィルタ機能用インスタンスを生成します。
        /// </summary>
        /// <returns>ツールバーの傷フィルタ機能用インスタンス</returns>
        public sealed override ViewItemToolbarPluginInstance GenerateViewItemToolbarPluginInstance()
        {
            return new ToolbarScratchFilterInstance();
        }

        #endregion Methods
    }
}
