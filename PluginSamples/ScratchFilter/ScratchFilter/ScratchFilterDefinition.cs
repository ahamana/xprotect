using NLog;
using ScratchFilter.Client.Toolbar;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
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
    public class ScratchFilterDefinition : PluginDefinition
    {
        #region Fields

        /// <summary>
        /// ロガーです。
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// バージョン情報です。
        /// </summary>
        private static readonly FileVersionInfo FileVersionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);

        /// <summary>
        /// プラグインの ID です。
        /// </summary>
        internal static readonly Guid PluginId = new Guid("8c9a02d0-dcb0-4460-8779-23ad282677a0");

        /// <summary>
        /// Smart Client のツールバー用プラグインの ID です。
        /// </summary>
        internal static readonly Guid ToolbarPluginId = new Guid("655154aa-af1d-47af-b23a-09ea79eb1b9d");

        #endregion

        #region Properties

        /// <summary>
        /// ID です。
        /// </summary>
        /// <value>
        /// ID
        /// </value>
        public override Guid Id
        {
            get;
        } = PluginId;

        /// <summary>
        /// 製品名です。
        /// </summary>
        /// <value>
        /// 製品名
        /// </value>
        public override string Name
        {
            get;
        } = FileVersionInfo.ProductName;

        /// <summary>
        /// 会社名です。
        /// </summary>
        /// <value>
        /// 会社名
        /// </value>
        public override string Manufacturer
        {
            get;
        } = FileVersionInfo.CompanyName;

        /// <summary>
        /// プラグインのバージョンです。
        /// </summary>
        /// <value>
        /// プラグインのバージョン
        /// </value>
        public override string VersionString
        {
            get;
        } = FileVersionInfo.ProductVersion;

        /// <summary>
        /// トップレベルで使用するアイコンです。
        /// </summary>
        /// <value>
        /// トップレベルで使用するアイコン
        /// </value>
        public override Image Icon
        {
            get;
        }

        /// <summary>
        /// Smart Client のツールバー用プラグインの一覧です。
        /// </summary>
        /// <value>
        /// Smart Client のツールバー用プラグインの一覧
        /// </value>
        public override List<ViewItemToolbarPlugin> ViewItemToolbarPlugins
        {
            get;
        } = new List<ViewItemToolbarPlugin>();

        #endregion

        #region Methods

        /// <summary>
        /// 初期化処理を行います。
        /// </summary>
        public override void Init()
        {
            Logger.Info("Start plug-in");

            ViewItemToolbarPlugins.Add(new ToolbarSeparator());
            ViewItemToolbarPlugins.Add(new ScratchFilterToolbarPlugin());
            ViewItemToolbarPlugins.Add(new ToolbarSeparator());
        }

        /// <summary>
        /// 終了処理を行います。
        /// </summary>
        public override void Close()
        {
            ViewItemToolbarPlugins.Clear();

            Logger.Info("End plug-in");
        }

        #endregion
    }
}
