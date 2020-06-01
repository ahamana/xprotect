using System.Windows;

namespace ScratchFilter.Client.View
{
    /// <summary>
    /// ScratchFilterSettingsWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ScratchFilterSettingWindow : Window
    {
        #region Constructors

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        public ScratchFilterSettingWindow()
        {
            InitializeComponent();
        }

        #endregion

        #region Methods

        /// <summary>
        /// OK ボタンが押下された時に呼び出されます。
        /// </summary>
        /// <param name="sender">通知元</param>
        /// <param name="e">イベント引数</param>
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// キャンセルボタンが押下された時に呼び出されます。
        /// </summary>
        /// <param name="sender">通知元</param>
        /// <param name="e">イベント引数</param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #endregion
    }
}
