﻿//
// (C) 2020 Canon Inc.
//
// This file contains trade secrets of Canon Inc. No part may be reproduced
// or transmitted in any form by any means or for any purpose without the
// express written permission of Canon Inc.
//

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;

using NLog;
using NLog.Fluent;

using ScratchFilter.Client.Data;
using ScratchFilter.Client.Toolbar;

using VideoOS.Platform;
using VideoOS.Platform.Client;

namespace ScratchFilter
{
    /// <summary>
    /// The PluginDefinition is the 'entry' point to any plugin.
    /// This is the starting point for any plugin development and the class MUST be available for a plugin to be loaded.
    /// Several PluginDefinitions are allowed to be available within one DLL.
    /// Here the references to all other plugin known objects and classes are defined.
    /// The class is an abstract class where all implemented methods and properties need to be declared with override.
    /// The class is constructed when the environment is loading the DLL.
    /// </summary>
    /// <seealso cref="PluginDefinition" />
    [ToString]
    public sealed class ScratchFilterPluginDefinition : PluginDefinition
    {
        #region Fields

        /// <summary>
        /// ロガーです。
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// アセンブリです。
        /// </summary>
        private static readonly Assembly Assembly = Assembly.GetExecutingAssembly();

        /// <summary>
        /// バージョン情報です。
        /// </summary>
        private static readonly FileVersionInfo FileVersionInfo = FileVersionInfo.GetVersionInfo(Assembly.Location);

        /// <summary>
        /// プラグインの ID です。
        /// </summary>
        private static readonly Guid PluginId = Guid.Parse(Assembly.GetCustomAttribute<GuidAttribute>().Value);

        #endregion Fields

        #region Constructors

        /// <summary>
        /// 静的コンストラクタです。
        /// </summary>
        static ScratchFilterPluginDefinition()
        {
            var productName = FileVersionInfo.ProductName;

            LogManager.Configuration.Variables.Add(nameof(productName), productName);
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// ID です。
        /// </summary>
        /// <value>ID</value>
        public sealed override Guid Id { get; } = PluginId;

        /// <summary>
        /// 製品名です。
        /// </summary>
        /// <value>製品名</value>
        public sealed override string Name { get; } = FileVersionInfo.ProductName;

        /// <summary>
        /// 会社名です。
        /// </summary>
        /// <value>会社名</value>
        public sealed override string Manufacturer { get; } = FileVersionInfo.CompanyName;

        /// <summary>
        /// プラグインのバージョンです。
        /// </summary>
        /// <value>プラグインのバージョン</value>
        public sealed override string VersionString { get; } = FileVersionInfo.ProductVersion;

        /// <summary>
        /// トップレベルで使用するアイコンです。
        /// </summary>
        /// <value>トップレベルで使用するアイコン</value>
        public sealed override Image? Icon { get; }

        /// <summary>
        /// Smart Client のツールバー用プラグインの一覧です。
        /// </summary>
        /// <value>Smart Client のツールバー用プラグインの一覧</value>
        public sealed override List<ViewItemToolbarPlugin> ViewItemToolbarPlugins { get; } = new();

        #endregion Properties

        #region Methods

        /// <summary>
        /// 初期化処理を行います。
        /// </summary>
        public sealed override void Init()
        {
            Logger.Info()
                  .Message("Start plug-in on {0}", EnvironmentManager.Instance.EnvironmentProduct)
                  .Write();

            if (EnvironmentManager.Instance.EnvironmentType == EnvironmentType.SmartClient)
            {
                ViewItemToolbarPlugins.Add(new ToolbarSeparator());
                ViewItemToolbarPlugins.Add(new ToolbarScratchFilter());
                ViewItemToolbarPlugins.Add(new ToolbarScratchFilterSetting());
                ViewItemToolbarPlugins.Add(new ToolbarSeparator());

                ScratchFilterSettingManager.Instance.Load();
            }
        }

        /// <summary>
        /// 終了処理を行います。
        /// </summary>
        public sealed override void Close()
        {
            ViewItemToolbarPlugins.Clear();

            Logger.Info()
                  .Message("End plug-in on {0}", EnvironmentManager.Instance.EnvironmentProduct)
                  .Write();
        }

        #endregion Methods
    }
}
