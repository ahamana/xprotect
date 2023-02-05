/*
 * MIT License
 *
 * (c) 2020 Akihiro Hamana
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System;
using System.Drawing;
using System.Windows.Forms;

using ImageProcessor;

using ScratchFilter.Client.Data;
using ScratchFilter.Properties;

using VideoOS.Platform;
using VideoOS.Platform.Client;

using Point = System.Drawing.Point;

namespace ScratchFilter.Client.Toolbar;

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

    /// <inheritdoc />
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

    /// <inheritdoc />
    private protected sealed override void OnImageViewerImageDisplayed(ImageViewerAddOn imageViewerAddOn, ImageDisplayedEventArgs e) =>
        HandleDisplayedImage(imageViewerAddOn);

    /// <inheritdoc />
    private protected sealed override void OnImageViewerRecordedImageReceived(ImageViewerAddOn imageViewerAddOn, RecordedImageReceivedEventArgs e) =>
        HandleDisplayedImage(imageViewerAddOn);

    /// <inheritdoc />
    private protected sealed override void LoadIcon() =>
        Icon = (ClientControl.Instance.Theme.ThemeType, isActive) switch
        {
            (ThemeType.Dark, true) => Resources.Toolbar_ScratchFilter_Icon_Dark_Active,
            (ThemeType.Dark, false) => Resources.Toolbar_ScratchFilter_Icon_Dark_Inactive,
            (ThemeType.Light, true) => Resources.Toolbar_ScratchFilter_Icon_Light_Active,
            (ThemeType.Light, false) => Resources.Toolbar_ScratchFilter_Icon_Light_Inactive,
            _ => default
        };

    /// <inheritdoc />
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
    private static readonly Guid PluginId = Guid.Parse("655154AA-AF1D-47AF-B23A-09EA79EB1B9D");

    #endregion Fields

    #region Properties

    /// <inheritdoc />
    public sealed override Guid Id { get; } = PluginId;

    #endregion Properties

    #region Methods

    /// <inheritdoc />
    public sealed override ViewItemToolbarPluginInstance GenerateViewItemToolbarPluginInstance() =>
        new ToolbarScratchFilterInstance();

    #endregion Methods
}
