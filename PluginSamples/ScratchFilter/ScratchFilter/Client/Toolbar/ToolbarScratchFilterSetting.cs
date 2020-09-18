using ScratchFilter.Client.Views;
using ScratchFilter.Properties;
using System;
using System.Windows;
using VideoOS.Platform;
using VideoOS.Platform.Client;

namespace ScratchFilter.Client.Toolbar
{
    /// <summary>
    /// ツールバーの傷フィルタ機能の設定用インスタンスです。
    /// </summary>
    /// <seealso cref="ImageViewerToolbarPluginInstance" />
    [ToString]
    internal sealed class ToolbarScratchFilterSettingInstance : ImageViewerToolbarPluginInstance
    {
        #region Properties

        /// <summary>
        /// プラグインの説明です。
        /// </summary>
        /// <value>
        /// プラグインの説明
        /// </value>
        protected override string Description { get; } = Resources.Toolbar_ScratchFilterSetting_Description;

        #endregion Properties

        #region Methods

        /// <summary>
        /// アイコンを読み込みます。
        /// </summary>
        protected override void LoadIcon()
        {
            Icon = ClientControl.Instance.Theme.ThemeType switch
            {
                ThemeType.Dark => Resources.Toolbar_ScratchFilterSetting_Icon_Dark,
                ThemeType.Light => Resources.Toolbar_ScratchFilterSetting_Icon_Light,
                _ => default
            };
        }

        /// <summary>
        /// ツールバー用プラグインのメニューが押下された時に発生します。
        /// </summary>
        public override void Activate()
        {
            Window window = new ScratchFilterSettingWindow(ImageViewerAddOn);

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
        private static readonly Guid PluginId = Guid.Parse("27d96417-e33c-4f71-b231-bd692f4af61b");

        #endregion Fields

        #region Properties

        /// <summary>
        /// ID です。
        /// </summary>
        /// <value>
        /// ID
        /// </value>
        public override Guid Id { get; } = PluginId;

        #endregion Properties

        #region Methods

        /// <summary>
        /// ツールバーの傷フィルタ機能の設定用インスタンスを生成します。
        /// </summary>
        /// <returns>
        /// ツールバーの傷フィルタ機能の設定用インスタンス
        /// </returns>
        public override ViewItemToolbarPluginInstance GenerateViewItemToolbarPluginInstance()
        {
            return new ToolbarScratchFilterSettingInstance();
        }

        #endregion Methods
    }
}
