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

namespace ScratchFilter.Client.OptionsDialog
{
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

            Name = string.Format(Resources.Help_ScratchFilter_About, pluginDefinition.Name);
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// ID です。
        /// </summary>
        /// <value>ID</value>
        public sealed override Guid Id { get; } = PluginId;

        /// <summary>
        /// 名前です。
        /// </summary>
        /// <value>名前</value>
        public sealed override string Name { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// 初期化処理を行います。
        /// </summary>
        public sealed override void Init() { }

        /// <summary>
        /// 終了処理を行います。
        /// </summary>
        public sealed override void Close() { }

        /// <summary>
        /// ユーザーコントロールを生成します。
        /// </summary>
        /// <returns>ユーザーコントロール</returns>
        public sealed override OptionsDialogUserControl GenerateUserControl() =>
            new ScratchFilterUserControl(pluginDefinition);

        /// <summary>
        /// 変更内容を保存します。
        /// </summary>
        /// <returns><c>true</c></returns>
        public sealed override bool SaveChanges() =>
            SaveProperties(true);

        #endregion Methods
    }
}
