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

using ScratchFilter.Client.Views.Toolbar;
using ScratchFilter.Properties;

using VideoOS.Platform;
using VideoOS.Platform.Client;

namespace ScratchFilter.Client.Toolbar;

/// <summary>
/// ツールバーの傷フィルタ機能の設定用インスタンスです。
/// </summary>
/// <seealso cref="ImageViewerToolbarPluginInstance" />
[ToString]
internal sealed class ToolbarScratchFilterSettingInstance : ImageViewerToolbarPluginInstance
{
    #region Properties

    /// <inheritdoc />
    private protected sealed override string Description { get; } = Resources.Toolbar_ScratchFilterSetting_Description;

    #endregion Properties

    #region Methods

    /// <inheritdoc />
    private protected sealed override void LoadIcon() =>
        Icon = ClientControl.Instance.Theme.ThemeType switch
        {
            ThemeType.Dark => Resources.Toolbar_ScratchFilterSetting_Icon_Dark,
            ThemeType.Light => Resources.Toolbar_ScratchFilterSetting_Icon_Light,
            _ => default
        };

    /// <inheritdoc />
    public sealed override void Activate()
    {
        var window = new ScratchFilterSettingWindow(CameraId);

        window.ShowDialog();
    }

    #endregion Methods
}

/// <summary>
/// ツールバーの傷フィルタ機能の設定です。
/// </summary>
/// <seealso cref="ImageViewerToolbarPlugin" />
[ToString]
internal sealed class ToolbarScratchFilterSetting : ImageViewerToolbarPlugin
{
    #region Fields

    /// <summary>
    /// プラグインの ID です。
    /// </summary>
    private static readonly Guid PluginId = Guid.Parse("27D96417-E33C-4F71-B231-BD692F4AF61B");

    #endregion Fields

    #region Properties

    /// <inheritdoc />
    public sealed override Guid Id { get; } = PluginId;

    #endregion Properties

    #region Methods

    /// <inheritdoc />
    public sealed override ViewItemToolbarPluginInstance GenerateViewItemToolbarPluginInstance() =>
        new ToolbarScratchFilterSettingInstance();

    #endregion Methods
}
