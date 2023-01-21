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

using ScratchFilter.Client.Views.OptionsDialog;
using ScratchFilter.Properties;

using VideoOS.Platform.Client;

namespace ScratchFilter.Client.OptionsDialog;

/// <summary>
/// 傷フィルタの設定オプションです。
/// </summary>
/// <seealso cref="OptionsDialogPlugin" />
[ToString]
internal sealed class OptionScratchFilter : OptionsDialogPlugin
{
    #region Fields

    /// <summary>
    /// プラグインの ID です。
    /// </summary>
    private static readonly Guid PluginId = Guid.Parse("D3627413-98D3-4804-831F-271C4C81F082");

    /// <summary>
    /// プラグインの定義です。
    /// </summary>
    private readonly ScratchFilterPluginDefinition pluginDefinition;

    #endregion Fields

    #region Constructors

    /// <summary>
    /// コンストラクタです。
    /// </summary>
    /// <param name="pluginDefinition">プラグインの定義</param>
    internal OptionScratchFilter(ScratchFilterPluginDefinition pluginDefinition)
    {
        this.pluginDefinition = pluginDefinition;
    }

    #endregion Constructors

    #region Properties

    /// <inheritdoc />
    public sealed override Guid Id { get; } = PluginId;

    /// <inheritdoc />
    public sealed override string Name =>
        string.Format(Resources.Help_ScratchFilter_About, pluginDefinition.Name);

    #endregion Properties

    #region Methods

    /// <inheritdoc />
    public sealed override void Init() { }

    /// <inheritdoc />
    public sealed override void Close() { }

    /// <inheritdoc />
    public sealed override OptionsDialogUserControl GenerateUserControl() =>
        new ScratchFilterUserControl(pluginDefinition);

    /// <returns><c>true</c></returns>
    /// <inheritdoc />
    public sealed override bool SaveChanges() =>
        SaveProperties(true);

    #endregion Methods
}
