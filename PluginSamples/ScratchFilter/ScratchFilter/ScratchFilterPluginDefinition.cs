//
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
using ScratchFilter.Client.OptionsDialog;
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
    /// <seealso cref="IDisposable" />
    [ToString]
    public sealed class ScratchFilterPluginDefinition : PluginDefinition, IDisposable
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

        /// <summary>
        /// アンマネージリソースが解放されたかどうかです。
        /// </summary>
        private bool isDisposed;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// 静的コンストラクタです。
        /// </summary>
        static ScratchFilterPluginDefinition()
        {
            var productName = FileVersionInfo.ProductName;
            var environmentType = Enum.GetName(typeof(EnvironmentType), EnvironmentManager.Instance.EnvironmentType);

            LogManager.Configuration.Variables.Add(nameof(productName), productName);
            LogManager.Configuration.Variables.Add(nameof(environmentType), environmentType);
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// 著作権です。
        /// </summary>
        /// <value>著作権</value>
        internal string Copyright { get; } = FileVersionInfo.LegalCopyright;

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
        /// Management Client でのトップレベルで使用するアイコンです。
        /// </summary>
        /// <value>Management Client でのトップレベルで使用するアイコン</value>
        public sealed override Image? Icon { get; }

        /// <summary>
        /// Smart Client のツールバー用プラグインの一覧です。
        /// </summary>
        /// <value>Smart Client のツールバー用プラグインの一覧</value>
        public sealed override List<ViewItemToolbarPlugin> ViewItemToolbarPlugins { get; } = new();

        /// <summary>
        /// Smart Client の設定オプション用プラグインの一覧です。
        /// </summary>
        /// <value>Smart Client の設定オプション用プラグインの一覧</value>
        public sealed override List<OptionsDialogPlugin> OptionsDialogPlugins { get; } = new();

        #endregion Properties

        #region Methods

        /// <summary>
        /// アンマネージリソースを解放し、必要に応じてマネージリソースも解放します。
        /// </summary>
        /// <param name="disposing">マネージリソースとアンマネージリソースの両方を解放する場合は <c>true</c>。アンマネージリソースだけを解放する場合は <c>false</c>。</param>
        private void Dispose(bool disposing)
        {
            if (isDisposed)
            {
                return;
            }

            if (disposing)
            {
                Icon?.Dispose();
            }

            isDisposed = true;
        }

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

                OptionsDialogPlugins.Add(new OptionScratchFilter(this));

                ScratchFilterSettingManager.Instance.Load();
            }
        }

        /// <summary>
        /// 終了処理を行います。
        /// </summary>
        public sealed override void Close()
        {
            ViewItemToolbarPlugins.Clear();
            OptionsDialogPlugins.Clear();

            Dispose();

            Logger.Info()
                  .Message("End plug-in on {0}", EnvironmentManager.Instance.EnvironmentProduct)
                  .Write();
        }

        /// <summary>
        /// アンマネージリソースの解放またはリセットに関連付けられているアプリケーション定義のタスクを実行します。
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion Methods
    }
}
