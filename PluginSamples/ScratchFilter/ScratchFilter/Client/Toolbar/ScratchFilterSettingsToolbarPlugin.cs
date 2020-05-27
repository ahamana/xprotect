using ScratchFilter.Client.View;
using ScratchFilter.Properties;
using System.Windows;
using VideoOS.Platform;
using VideoOS.Platform.Client;

namespace ScratchFilter.Client.Toolbar
{
    /// <summary>
    /// ツールバーの傷フィルタ機能の設定用インスタンスです。
    /// </summary>
    /// <seealso cref="ToolbarPluginInstance" />
    [ToString]
    internal class ScratchFilterSettingsToolbarPluginInstance : ToolbarPluginInstance
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
        } = Resources.ToolbarPlugin_ScratchFilterSettings_Description;

        #endregion

        #region Methods

        /// <summary>
        /// アイコンを読み込みます。
        /// </summary>
        protected override void LoadIcon()
        {
            Icon = ClientControl.Instance.Theme.ThemeType switch
            {
                ThemeType.Dark => Resources.ToolbarPlugin_ScratchFilterSettings_Icon_Dark,
                ThemeType.Light => Resources.ToolbarPlugin_ScratchFilterSettings_Icon_Light,
                _ => default
            };
        }

        /// <summary>
        /// ツールバーのインスタンスが UI に追加されたときに呼ばれます。
        /// </summary>
        /// <param name="viewItemInstance">ツールバーのアイテムに関連したビューアイテムのインスタンス</param>
        /// <param name="window">ツールバーの表示先のウィンドウ</param>
        public override void Init(Item viewItemInstance, Item window)
        {
            // ※「再生タブ → エクスポート → カメラ選択」を行うと、window が null の状態で呼ばれる。
            if (window == null)
            {
                return;
            }

            base.Init(viewItemInstance, window);

            Enabled = true;
        }

        /// <summary>
        /// ツールバー用プラグインのメニューが押下された時に呼び出されます。
        /// </summary>
        public override void Activate()
        {
            Window window = new ScratchFilterSettingsWindow();

            window.ShowDialog();
        }

        #endregion
    }

    /// <summary>
    /// ツールバーの傷フィルタ機能の設定です。
    /// </summary>
    /// <seealso cref="ToolbarPlugin" />
    [ToString]
    internal class ScratchFilterSettingsToolbarPlugin : ToolbarPlugin
    {
        #region Methods

        /// <summary>
        /// ツールバーの傷フィルタ機能の設定用インスタンスを生成します。
        /// </summary>
        /// <returns>ツールバーの傷フィルタ機能の設定用インスタンス</returns>
        public override ViewItemToolbarPluginInstance GenerateViewItemToolbarPluginInstance()
        {
            return new ScratchFilterSettingsToolbarPluginInstance();
        }

        #endregion
    }
}
