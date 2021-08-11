//
// (C) 2020 Canon Inc.
//
// This file contains trade secrets of Canon Inc. No part may be reproduced
// or transmitted in any form by any means or for any purpose without the
// express written permission of Canon Inc.
//

using ScratchFilter.Properties;

using VideoOS.Platform;
using VideoOS.Platform.Client;

namespace ScratchFilter.Client.Views.OptionsDialog
{
    /// <summary>
    /// 傷フィルタの設定オプション用のユーザーコントロールです。
    /// </summary>
    /// <seealso cref="OptionsDialogUserControl" />
    internal sealed partial class ScratchFilterUserControl : OptionsDialogUserControl
    {
        #region Fields

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
        internal ScratchFilterUserControl(ScratchFilterPluginDefinition pluginDefinition)
        {
            InitializeComponent();

            this.pluginDefinition = pluginDefinition;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// 初期化処理を行います。
        /// </summary>
        public sealed override void Init()
        {
            iconPictureBox.Image = ClientControl.Instance.Theme.ThemeType switch
            {
                ThemeType.Dark => Resources.Application_Icon_Dark,
                ThemeType.Light => Resources.Application_Icon_Light,
                _ => default
            };

            aboutLabel.Text = string.Format(Resources.Help_ScratchFilter_About_Details,
                                            pluginDefinition.Name,
                                            pluginDefinition.VersionString,
                                            pluginDefinition.Copyright);
        }

        /// <summary>
        /// 終了処理を行います。
        /// </summary>
        public sealed override void Close()
        {
            iconPictureBox.Image?.Dispose();

            Dispose();
        }

        #endregion Methods
    }
}
