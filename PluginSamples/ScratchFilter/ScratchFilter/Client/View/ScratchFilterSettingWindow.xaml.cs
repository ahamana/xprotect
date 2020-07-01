using ImageProcessor;
using OpenCvSharp.Extensions;
using ScratchFilter.Client.Data;
using ScratchFilter.Common.Live;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Media;
using VideoOS.Platform;
using VideoOS.Platform.Client;
using Brush = System.Drawing.Brush;
using Size = System.Drawing.Size;
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

            if (originalImage == null)
            {
                previewImage.Source = CreateTextImage(Properties.Resources.Toolbar_ScratchFilterSetting_Message_ImageCaptureFailure,
                                                      new Size((int)previewImage.Width, (int)(imageViewerAddOn.Size.Height * previewImage.Width / imageViewerAddOn.Size.Width)));

                imageContrastSlider.IsEnabled = false;
                imageBrightnessSlider.IsEnabled = false;
                imageSaturationSlider.IsEnabled = false;
                imageGammaSlider.IsEnabled = false;
                okButton.IsEnabled = false;
            }
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
                using IImageCollector imageCollector = new BitmapCollector(imageViewerAddOn.CameraFQID);

                image = imageCollector.GetImage();
            }

            return image;
        }

        /// <summary>
        /// テキスト画像を生成します。
        /// </summary>
        /// <param name="text">テキスト</param>
        /// <param name="size">画像のサイズ</param>
        /// <returns>
        /// テキスト画像
        /// </returns>
        private ImageSource CreateTextImage(string text, Size size)
        {
            using Bitmap image = new Bitmap(size.Width, size.Height);
            using Graphics graphics = Graphics.FromImage(image);
            using Font font = new Font(FontFamily.Source, (float)FontSize, GraphicsUnit.Pixel);
            using Brush brush = new SolidBrush(ClientControl.Instance.Theme.TextColor);

            graphics.Clear(ClientControl.Instance.Theme.BorderColor);

            Rectangle rectangle = new Rectangle()
            {
                Size = image.Size
            };

            StringFormat format = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            graphics.DrawString(text, font, brush, rectangle, format);

            return image.ToBitmapSource();
        }

        /// <summary>
        /// プレビュー画像を変更します。
        /// </summary>
        private void ChangePreviewImage()
        {
            if (originalImage == null)
            {
                return;
            }

            using ImageFactory imageFactory = new ImageFactory();

            imageFactory.Load(originalImage)
                        .Contrast(setting.ImageContrast)
                        .Brightness(setting.ImageBrightness)
                        .Saturation(setting.ImageSaturation)
                        .Gamma(setting.ImageGamma);

            using Bitmap image = new Bitmap(imageFactory.Image);

            previewImage.Source = image.ToBitmapSource();
        }

        /// <summary>
        /// 画像のコントラストのスライダーの値が変更された時に発生します。
        /// </summary>
        /// <param name="sender">イベントハンドラがアタッチされるオブジェクト</param>
        /// <param name="e">イベントのデータ</param>
        private void ImageContrastSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ChangePreviewImage();
        }


        /// <summary>
        /// 画像の明るさのスライダーの値が変更された時に発生します。
        /// </summary>
        /// <param name="sender">イベントハンドラがアタッチされるオブジェクト</param>
        /// <param name="e">イベントのデータ</param>
        private void ImageBrightnessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ChangePreviewImage();
        }

        /// <summary>
        /// 画像の彩度のスライダーの値が変更された時に発生します。
        /// </summary>
        /// <param name="sender">イベントハンドラがアタッチされるオブジェクト</param>
        /// <param name="e">イベントのデータ</param>
        private void ImageSaturationSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ChangePreviewImage();
        }

        /// <summary>
        /// 画像のガンマのスライダーの値が変更された時に発生します。
        /// </summary>
        /// <param name="sender">イベントハンドラがアタッチされるオブジェクト</param>
        /// <param name="e">イベントのデータ</param>
        private void ImageGammaSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ChangePreviewImage();
        }

        /// <summary>
        /// OK ボタンが押下された時に発生します。
        /// </summary>
        /// <param name="sender">イベントハンドラがアタッチされるオブジェクト</param>
        /// <param name="e">イベントのデータ</param>
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            ScratchFilterSettingManager.Instance.Save(setting);

            Close();
        }

        /// <summary>
        /// キャンセルボタンが押下された時に発生します。
        /// </summary>
        /// <param name="sender">イベントハンドラがアタッチされるオブジェクト</param>
        /// <param name="e">イベントのデータ</param>
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
