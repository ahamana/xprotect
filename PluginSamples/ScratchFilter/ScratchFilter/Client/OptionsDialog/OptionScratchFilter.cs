//
// (C) 2020 Canon Inc.
//
// This file contains trade secrets of Canon Inc. No part may be reproduced
// or transmitted in any form by any means or for any purpose without the
// express written permission of Canon Inc.
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
