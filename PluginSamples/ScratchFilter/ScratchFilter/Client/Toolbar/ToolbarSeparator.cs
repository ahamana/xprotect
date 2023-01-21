//
// MIT License
//
// (c) 2020 Akihiro Hamana
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//

using System;
using System.Drawing;

using VideoOS.Platform;
using VideoOS.Platform.Client;
using VideoOS.Platform.UI;

namespace ScratchFilter.Client.Toolbar;

/// <summary>
/// ツールバーのセパレータ用インスタンスです。
/// </summary>
/// <seealso cref="ToolbarPluginInstance" />
[ToString]
internal sealed class ToolbarSeparatorInstance : ToolbarPluginInstance
{
    #region Fields

    /// <summary>
    /// セパレータのサイズです。
    /// </summary>
    private static readonly Size SeparatorSize = new(1, Util.ImageList.ImageSize.Height);

    #endregion Fields

    #region Methods

    /// <summary>
    /// セパレータ用のアイコンを生成します。
    /// </summary>
    /// <returns>セパレータ用のアイコン</returns>
    private Image CreateSeparatorIcon()
    {
        var icon = new Bitmap(SeparatorSize.Width, SeparatorSize.Height);

        using var graphics = Graphics.FromImage(icon);
        using var brush = new SolidBrush(ClientControl.Instance.Theme.SeparatorLinesColor);

        var rectangle = Rectangle.Empty with
        {
            Size = icon.Size
        };

        graphics.FillRectangle(brush, rectangle);

        return icon;
    }

    /// <inheritdoc />
    private protected sealed override void LoadIcon() =>
        Icon = CreateSeparatorIcon();

    /// <inheritdoc />
    public sealed override void Activate() { }

    #endregion Methods
}

/// <summary>
/// ツールバーのセパレータです。
/// </summary>
/// <seealso cref="ToolbarPlugin" />
[ToString]
internal sealed class ToolbarSeparator : ToolbarPlugin
{
    #region Properties

    /// <inheritdoc />
    public sealed override Guid Id { get; }

    /// <inheritdoc />
    public sealed override ToolbarPluginOverflowMode ToolbarPluginOverflowMode { get; } = ToolbarPluginOverflowMode.NeverInOverflow;

    #endregion Properties

    #region Methods

    /// <inheritdoc />
    public sealed override ViewItemToolbarPluginInstance GenerateViewItemToolbarPluginInstance() =>
        new ToolbarSeparatorInstance();

    #endregion Methods
}
