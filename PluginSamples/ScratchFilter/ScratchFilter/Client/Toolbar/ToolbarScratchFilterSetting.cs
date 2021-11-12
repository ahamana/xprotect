//
// (C) 2020 Canon Inc.
//
// This file contains trade secrets of Canon Inc. No part may be reproduced
// or transmitted in any form by any means or for any purpose without the
// express written permission of Canon Inc.
//

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
    private protected sealed override void LoadIcon()
    {
        Icon = ClientControl.Instance.Theme.ThemeType switch
        {
            ThemeType.Dark => Resources.Toolbar_ScratchFilterSetting_Icon_Dark,
            ThemeType.Light => Resources.Toolbar_ScratchFilterSetting_Icon_Light,
            _ => default
        };
    }

    /// <inheritdoc />
    public sealed override void Activate()
    {
        if (CameraId == Guid.Empty)
        {
            return;
        }

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
