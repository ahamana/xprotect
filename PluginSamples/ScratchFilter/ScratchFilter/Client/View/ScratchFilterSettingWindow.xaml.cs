using OpenCvSharp;
using OpenCvSharp.Extensions;
using ScratchFilter.Client.Data;
using ScratchFilter.Extensions;
using ScratchFilter.Live;
using System;
using System.Drawing;
using System.Windows;
using VideoOS.Platform.Client;
using Window = System.Windows.Window;

namespace ScratchFilter.Client.View
{
    /// <summary>
    /// ScratchFilterSettingWindow.xaml の相互作用ロジック
    /// </summary>
    [ToString]
    internal sealed partial class ScratchFilterSettingWindow : Window, IDisposable
    {
        #region Fields

        /// <summary>
        /// アンマネージリソースが解放されたかどうかです。
        /// </summary>
        private bool disposed;

        /// <summary>
        /// 傷フィルタの設定です。
        /// </summary>
        private readonly ScratchFilterSetting setting;

        /// <summary>
        /// オリジナル画像です。
        /// </summary>
        private readonly Bitmap originalImage;

        #endregion

        #region Constructors

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        /// <param name="imageViewerAddOn">イメージビューワのアドオン</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="imageViewerAddOn" /> が <c>null</c> の場合にスローされます。
        /// </exception>
        internal ScratchFilterSettingWindow(ImageViewerAddOn imageViewerAddOn)
        {
            if (imageViewerAddOn == null)
            {
                throw new ArgumentNullException(nameof(imageViewerAddOn));
            }

            InitializeComponent();

            setting = ScratchFilterSettingManager.Instance.GetSetting(imageViewerAddOn.CameraFQID.ObjectId);

            DataContext = setting;

            originalImage = GetOriginalImage(imageViewerAddOn);

            ChangePreviewImage();
        }

        #endregion

        #region Methods

        /// <summary>
        /// オリジナル画像を取得します。
        /// </summary>
        /// <param name="imageViewerAddOn">イメージビューワのアドオン</param>
        /// <returns>
        /// オリジナル画像
        /// </returns>
        private Bitmap GetOriginalImage(ImageViewerAddOn imageViewerAddOn)
        {
            Bitmap image = imageViewerAddOn.GetCurrentDisplayedImageAsBitmap();

            if (image == null)
            {
                using (IImageCollector imageCollector = new BitmapCollector(imageViewerAddOn.CameraFQID))
                {
                    image = imageCollector.GetImage();
                }
            }

            return image;
        }

        /// <summary>
        /// プレビュー画像を変更します。
        /// </summary>
        private void ChangePreviewImage()
        {
            using (Bitmap bitmap = originalImage.Adjust(setting.ImageContrast, setting.ImageBrightness,
                                                        setting.ImageSaturation, setting.ImageGamma))
            using (Mat mat = bitmap.ToMat())
            {
                previewImage.Source = mat.ToBitmapSource();
            }
        }

        /// <summary>
        /// 画像のコントラストのスライダーの値が変更された時に発生します。
        /// </summary>
        /// <param name="sender">通知元</param>
        /// <param name="e">イベント引数</param>
        private void ImageContrastSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ChangePreviewImage();
        }


        /// <summary>
        /// 画像の明るさのスライダーの値が変更された時に発生します。
        /// </summary>
        /// <param name="sender">通知元</param>
        /// <param name="e">イベント引数</param>
        private void ImageBrightnessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ChangePreviewImage();
        }

        /// <summary>
        /// 画像の彩度のスライダーの値が変更された時に発生します。
        /// </summary>
        /// <param name="sender">通知元</param>
        /// <param name="e">イベント引数</param>
        private void ImageSaturationSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ChangePreviewImage();
        }

        /// <summary>
        /// 画像のガンマのスライダーの値が変更された時に発生します。
        /// </summary>
        /// <param name="sender">通知元</param>
        /// <param name="e">イベント引数</param>
        private void ImageGammaSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ChangePreviewImage();
        }

        /// <summary>
        /// OK ボタンが押下された時に発生します。
        /// </summary>
        /// <param name="sender">通知元</param>
        /// <param name="e">イベント引数</param>
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            ScratchFilterSettingManager.Instance.Save(setting);

            Close();
        }

        /// <summary>
        /// キャンセルボタンが押下された時に発生します。
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
                originalImage?.Dispose();
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
