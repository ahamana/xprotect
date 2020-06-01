using ScratchFilter.Client.Data;
using System;
using System.Drawing;
using System.Windows;
using VideoOS.Platform.Client;

namespace ScratchFilter.Client.View
{
    /// <summary>
    /// ScratchFilterSettingWindow.xaml の相互作用ロジック
    /// </summary>
    [ToString]
    internal partial class ScratchFilterSettingWindow : Window, IDisposable
    {
        #region Fields

        /// <summary>
        /// アンマネージリソースが解放されたかどうかです。
        /// </summary>
        private bool disposed;

        /// <summary>
        /// オリジナル画像です。
        /// </summary>
        private Image originalImage;

        #endregion

        #region Properties

        /// <summary>
        /// 表示画像です。
        /// </summary>
        /// <value>
        /// 表示画像
        /// </value>
        public Image Image
        {
            get;
            private set;
        }

        /// <summary>
        /// 傷フィルタの設定です。
        /// </summary>
        /// <value>
        /// 傷フィルタの設定
        /// </value>
        public ScratchFilterSetting ScratchFilterSetting
        {
            get;
            private set;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        /// <param name="imageViewerAddOn">イメージビューワのアドオン</param>
        internal ScratchFilterSettingWindow(ImageViewerAddOn imageViewerAddOn)
        {
            if (imageViewerAddOn == null)
            {
                throw new ArgumentNullException(nameof(imageViewerAddOn));
            }

            InitializeComponent();

            originalImage = imageViewerAddOn.GetCurrentDisplayedImageAsBitmap();

            ScratchFilterSetting = ScratchFilterSettingManager.Instance.GetSetting(imageViewerAddOn.CameraFQID.ObjectId);

            DataContext = this;
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
            ScratchFilterSettingManager.Instance.Save();

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

        /// <summary>
        /// アンマネージリソースを解放し、必要に応じてマネージリソースも解放します。
        /// </summary>
        /// <param name="disposing">マネージリソースとアンマネージリソースの両方を解放する場合は <c>true</c>。アンマネージリソースだけを解放する場合は <c>false</c>。</param>
        private void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                if (originalImage != null)
                {
                    originalImage.Dispose();
                }

                if (Image != null)
                {
                    Image.Dispose();
                }
            }

            disposed = true;
        }

        /// <summary>
        /// アンマネージリソースの解放またはリセットに関連付けられているアプリケーション定義のタスクを実行します。
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
