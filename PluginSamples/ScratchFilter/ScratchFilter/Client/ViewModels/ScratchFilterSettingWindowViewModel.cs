//
// (C) 2020 Canon Inc.
//
// This file contains trade secrets of Canon Inc. No part may be reproduced
// or transmitted in any form by any means or for any purpose without the
// express written permission of Canon Inc.
//

using System;
using System.Drawing;
using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using ImageProcessor;

using Prism.Commands;
using Prism.Mvvm;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Notifiers;

using ScratchFilter.Client.Data;
using ScratchFilter.Common.Live;
using ScratchFilter.Extensions;
using ScratchFilter.Properties;

using VideoOS.Platform;
using VideoOS.Platform.Client;

using Size = System.Drawing.Size;
using SystemFonts = System.Drawing.SystemFonts;

namespace ScratchFilter.Client.ViewModels
{
    /// <summary>
    /// <see cref="ScratchFilterSettingWindow" /> の View Model です。
    /// </summary>
    /// <seealso cref="BindableBase" />
    /// <seealso cref="IDisposable" />
    [ToString]
    internal sealed class ScratchFilterSettingWindowViewModel : BindableBase, IDisposable
    {
        #region Fields

        /// <summary>
        /// 開放するリソースのグループです。
        /// </summary>
        private readonly CompositeDisposable disposable = new CompositeDisposable();

        /// <summary>
        /// 傷フィルタの設定です。
        /// </summary>
        private readonly ScratchFilterSetting setting;

        /// <summary>
        /// オリジナル画像です。
        /// </summary>
        private readonly Bitmap originalImage;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        /// <param name="imageViewerAddOn">イメージビューワのアドオン</param>
        /// <exception cref="ArgumentNullException"><paramref name="imageViewerAddOn" /> が <c>null</c> の場合にスローされます。</exception>
        internal ScratchFilterSettingWindowViewModel(ImageViewerAddOn imageViewerAddOn)
        {
            if (imageViewerAddOn is null)
            {
                throw new ArgumentNullException(nameof(imageViewerAddOn));
            }

            IsSettable = new BooleanNotifier(true);

            PreviewImage = new ReactivePropertySlim<ImageSource>().AddTo(disposable);

            setting = ScratchFilterSettingManager.Instance.GetSetting(imageViewerAddOn.CameraFQID.ObjectId);

            CameraName = setting.ObserveProperty(setting => setting.CameraName).ToReadOnlyReactivePropertySlim().AddTo(disposable);

            ImageContrast = ReactiveProperty.FromObject(setting, setting => setting.ImageContrast).AddTo(disposable);
            ImageContrast.Subscribe(_ => ChangePreviewImage());

            ImageBrightness = ReactiveProperty.FromObject(setting, setting => setting.ImageBrightness).AddTo(disposable);
            ImageBrightness.Subscribe(_ => ChangePreviewImage());

            ImageSaturation = ReactiveProperty.FromObject(setting, setting => setting.ImageSaturation).AddTo(disposable);
            ImageSaturation.Subscribe(_ => ChangePreviewImage());

            ImageGamma = ReactiveProperty.FromObject(setting, setting => setting.ImageGamma).AddTo(disposable);
            ImageGamma.Subscribe(_ => ChangePreviewImage());

            using var imageCollector = new BitmapCollector(imageViewerAddOn.CameraFQID);

            originalImage = imageCollector.GetImage()?.AddTo(disposable);

            if (originalImage is null)
            {
                var width = (int)PreviewImageWidth;
                var height = (int)(imageViewerAddOn.Size.Height * PreviewImageWidth / imageViewerAddOn.Size.Width);

                PreviewImage.Value = CreateTextImage(Resources.Toolbar_ScratchFilterSetting_Message_ImageCaptureFailure, new Size(width, height));

                IsSettable.TurnOff();
            }

            SaveCommand = IsSettable.ToReactiveCommand<Window>().WithSubscribe(SaveSettings).AddTo(disposable);

            CancelCommand = new DelegateCommand<Window>(Exit);

            ChangePreviewImage();
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// プレビュー画像の幅です。
        /// </summary>
        /// <value>プレビュー画像の幅</value>
        public double PreviewImageWidth { get; } = 640;

        /// <summary>
        /// 設定可能であるかどうかです。
        /// </summary>
        /// <value>設定可能であるかどうか</value>
        public BooleanNotifier IsSettable { get; }

        /// <summary>
        /// プレビュー画像です。
        /// </summary>
        /// <value>プレビュー画像</value>
        public IReactiveProperty<ImageSource> PreviewImage { get; }

        /// <summary>
        /// カメラ名です。
        /// </summary>
        /// <value>カメラ名</value>
        public IReadOnlyReactiveProperty<string> CameraName { get; }

        /// <summary>
        /// 画像のコントラストです。
        /// </summary>
        /// <value>画像のコントラスト</value>
        public IReactiveProperty<int> ImageContrast { get; }

        /// <summary>
        /// 画像の明るさです。
        /// </summary>
        /// <value>画像の明るさ</value>
        public IReactiveProperty<int> ImageBrightness { get; }

        /// <summary>
        /// 画像の彩度です。
        /// </summary>
        /// <value>画像の彩度</value>
        public IReactiveProperty<int> ImageSaturation { get; }

        /// <summary>
        /// 画像のガンマです。
        /// </summary>
        /// <value>画像のガンマ</value>
        public IReactiveProperty<float> ImageGamma { get; }

        /// <summary>
        /// 設定を保存するためのコマンドです。
        /// </summary>
        /// <value>設定を保存するためのコマンド</value>
        public ICommand SaveCommand { get; }

        /// <summary>
        /// キャンセルするためのコマンドです。
        /// </summary>
        /// <value>キャンセルするためのコマンド</value>
        public ICommand CancelCommand { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// テキスト画像を生成します。
        /// </summary>
        /// <param name="text">テキスト</param>
        /// <param name="size">画像のサイズ</param>
        /// <returns>テキスト画像</returns>
        private ImageSource CreateTextImage(string text, Size size)
        {
            using var image = new Bitmap(size.Width, size.Height);
            using var graphics = Graphics.FromImage(image);
            using var font = new Font(SystemFonts.DefaultFont.FontFamily, SystemFonts.DefaultFont.Size, GraphicsUnit.Pixel);
            using var brush = new SolidBrush(ClientControl.Instance.Theme.TextColor);

            graphics.Clear(ClientControl.Instance.Theme.BorderColor);

            var rectangle = new Rectangle
            {
                Size = image.Size
            };

            var format = new StringFormat
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
            if (originalImage is null)
            {
                return;
            }

            using var imageFactory = new ImageFactory();

            imageFactory.Load(originalImage)
                        .Contrast(setting.ImageContrast)
                        .Brightness(setting.ImageBrightness)
                        .Saturation(setting.ImageSaturation)
                        .Gamma(setting.ImageGamma);

            using var image = new Bitmap(imageFactory.Image);

            PreviewImage.Value = image.ToBitmapSource();
        }

        /// <summary>
        /// 設定を保存します。
        /// </summary>
        /// <param name="window">ウィンドウ</param>
        private void SaveSettings(Window window)
        {
            ScratchFilterSettingManager.Instance.Save(setting);

            window?.Close();
        }

        /// <summary>
        /// 処理を終了します。
        /// </summary>
        /// <param name="window">ウィンドウ</param>
        private void Exit(Window window)
        {
            window?.Close();
        }

        /// <summary>
        /// アンマネージリソースの解放またはリセットに関連付けられているアプリケーション定義のタスクを実行します。
        /// </summary>
        public void Dispose()
        {
            disposable.Dispose();
        }

        #endregion Methods
    }
}
