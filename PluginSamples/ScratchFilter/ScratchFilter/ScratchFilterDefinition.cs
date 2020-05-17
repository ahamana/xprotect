using NLog;
using ScratchFilter.Client.Toolbar;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        /// Gets the unique id identifying this plugin component
        /// </summary>
        public override Guid Id
        {
            get;
        } = PluginId;

        /// <summary>
        /// Define name of top level Tree node - e.g. A product name
        /// </summary>
        public override string Name
        {
            get;
        } = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductName;

        /// <summary>
        /// Your company name
        /// </summary>
        public override string Manufacturer
        {
            get;
        } = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).CompanyName;

        /// <summary>
        /// Version of this plugin.
        /// </summary>
        public override string VersionString
        {
            get;
        } = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;

        /// <summary>
        /// Icon to be used on top level - e.g. a product or company logo
        /// </summary>
        public override System.Drawing.Image Icon
        {
            get;
        }

        /// <summary>
        /// An extension plug-in to add to the view item toolbar in the Smart Client.
        /// </summary>
        public override List<ViewItemToolbarPlugin> ViewItemToolbarPlugins
        {
            get;
        } = new List<ViewItemToolbarPlugin>();

        #endregion

        #region Methods

        /// <summary>
        /// This method is called when the environment is up and running.
        /// Registration of Messages via RegisterReceiver can be done at this point.
        /// </summary>
        public override void Init()
        {
            Logger.Info("Start plug-in");

            ViewItemToolbarPlugins.Add(new ToolbarSeparator());
            ViewItemToolbarPlugins.Add(new ScratchFilterToolbarPlugin());
        }

        /// <summary>
        /// The main application is about to be in an undetermined state, either logging off or exiting.
        /// You can release resources at this point, it should match what you acquired during Init, so additional call to Init() will work.
        /// </summary>
        public override void Close()
        {
            ViewItemToolbarPlugins.Clear();

            Logger.Info("End plug-in");
        }

        #endregion
    }
}
