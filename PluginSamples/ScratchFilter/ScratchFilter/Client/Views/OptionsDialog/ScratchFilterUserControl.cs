//
// (C) 2020 Canon Inc.
//
// This file contains trade secrets of Canon Inc. No part may be reproduced
// or transmitted in any form by any means or for any purpose without the
// express written permission of Canon Inc.
//

using ScratchFilter.Properties;

using VideoOS.Platform.Client;

namespace ScratchFilter.Client.Views.OptionsDialog
{
    /// <summary>
    /// 傷フィルタの設定オプション用のユーザーコントロールです。
    /// </summary>
    /// <seealso cref="OptionsDialogUserControl" />
    internal sealed partial class ScratchFilterUserControl : OptionsDialogUserControl
    {
        #region Constructors

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        /// <param name="pluginDefinition">プラグインの定義</param>
        internal ScratchFilterUserControl(ScratchFilterPluginDefinition pluginDefinition)
        {
            InitializeComponent();

            aboutLabel.Text = string.Format(Resources.Help_ScratchFilter_About_Details,
                                            pluginDefinition.Name,
                                            pluginDefinition.VersionString,
                                            pluginDefinition.Copyright);
        }

        #endregion Constructors
    }
}
