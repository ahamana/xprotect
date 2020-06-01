using ScratchFilter.Client.View;
using ScratchFilter.Properties;
using System;
using VideoOS.Platform;
using VideoOS.Platform.Client;

namespace ScratchFilter.Client.Toolbar
{
    /// <summary>
    /// ツールバーの傷フィルタ機能の設定用インスタンスです。
    /// </summary>
    /// <seealso cref="ImageViewerToolbarPluginInstance" />
    [ToString]
    internal class ScratchFilterSettingToolbarPluginInstance : ImageViewerToolbarPluginInstance
    {
        #region Properties

        /// <summary>
        /// プラグインの説明です。
        /// </summary>
        /// <value>
        /// プラグインの説明
        /// </value>
        protected override string Description
        {
            get;
        } = Resources.ToolbarPlugin_ScratchFilterSetting_Description;

        #endregion

        #region Methods

        /// <summary>
        /// アイコンを読み込みます。
        /// </summary>
        protected override void LoadIcon()
        {
            Icon = ClientControl.Instance.Theme.ThemeType switch
            {
                ThemeType.Dark => Resources.ToolbarPlugin_ScratchFilterSetting_Icon_Dark,
                ThemeType.Light => Resources.ToolbarPlugin_ScratchFilterSetting_Icon_Light,
                _ => default
            };
        }

        /// <summary>
        /// ツールバー用プラグインのメニューが押下された時に呼び出されます。
        /// </summary>
        public override void Activate()
        {
            using (ScratchFilterSettingWindow window = new ScratchFilterSettingWindow(ImageViewerAddOn))
            {
                window.ShowDialog();
            }
        }

        #endregion
    }

    /// <summary>
    /// ツールバーの傷フィルタ機能の設定です。
    /// </summary>
    /// <seealso cref="ImageViewerToolbarPlugin" />
    [ToString]
    internal class ScratchFilterSettingToolbarPlugin : ImageViewerToolbarPlugin
    {
        #region Fields

        /// <summary>
        /// プラグインの ID です。
        /// </summary>
        private static readonly Guid PluginId = new Guid("27d96417-e33c-4f71-b231-bd692f4af61b");

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

        #endregion

        #region Methods

        /// <summary>
        /// ツールバーの傷フィルタ機能の設定用インスタンスを生成します。
        /// </summary>
        /// <returns>ツールバーの傷フィルタ機能の設定用インスタンス</returns>
        public override ViewItemToolbarPluginInstance GenerateViewItemToolbarPluginInstance()
        {
            return new ScratchFilterSettingToolbarPluginInstance();
        }

        #endregion
    }
}
