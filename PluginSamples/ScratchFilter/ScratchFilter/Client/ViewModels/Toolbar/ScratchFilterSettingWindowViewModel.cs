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

using Prism.Mvvm;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Notifiers;

using ScratchFilter.Client.Data;
using ScratchFilter.Client.Views.Toolbar;
using ScratchFilter.Common.Live;
using ScratchFilter.Extensions;

namespace ScratchFilter.Client.ViewModels.Toolbar;

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
    private readonly CompositeDisposable disposable = new();

    /// <summary>
    /// 傷フィルタの設定です。
    /// </summary>
    private readonly ScratchFilterSetting setting;

    /// <summary>
    /// オリジナル画像です。
    /// </summary>
    private readonly Bitmap? originalImage;

    #endregion Fields

    #region Constructors

    /// <summary>
    /// コンストラクタです。
    /// </summary>
    /// <param name="cameraId">カメラの ID</param>
    /// <exception cref="ArgumentException"><paramref name="cameraId" /> が <see cref="Guid.Empty" /> の場合にスローされます。</exception>
    internal ScratchFilterSettingWindowViewModel(Guid cameraId)
    {
        if (cameraId == Guid.Empty)
        {
            throw new ArgumentException(nameof(cameraId));
        }

        IsSettable = new(true);

        PreviewImage = new ReactivePropertySlim<ImageSource>().AddTo(disposable);

        ErrorMessageVisibility = new ReactivePropertySlim<Visibility>(Visibility.Hidden).AddTo(disposable);

        setting = ScratchFilterSettingManager.Instance.GetSetting(cameraId);

        CameraName = setting.ObserveProperty(setting => setting.CameraName).ToReadOnlyReactivePropertySlim().AddTo(disposable);

        ImageContrast = ReactiveProperty.FromObject(setting, setting => setting.ImageContrast).AddTo(disposable);
        ImageContrast.Subscribe(_ => ChangePreviewImage());

        ImageBrightness = ReactiveProperty.FromObject(setting, setting => setting.ImageBrightness).AddTo(disposable);
        ImageBrightness.Subscribe(_ => ChangePreviewImage());

        ImageSaturation = ReactiveProperty.FromObject(setting, setting => setting.ImageSaturation).AddTo(disposable);
        ImageSaturation.Subscribe(_ => ChangePreviewImage());

        ImageGamma = ReactiveProperty.FromObject(setting, setting => setting.ImageGamma).AddTo(disposable);
        ImageGamma.Subscribe(_ => ChangePreviewImage());

        using var imageCollector = new LiveBitmapCollector(cameraId);

        originalImage = imageCollector.GetImage()?.AddTo(disposable);

        if (originalImage is null)
        {
            ErrorMessageVisibility.Value = Visibility.Visible;
            IsSettable.TurnOff();
        }

        SaveCommand = IsSettable.ToReactiveCommand<Window>().WithSubscribe(SaveSettings).AddTo(disposable);

        CancelCommand = new ReactiveCommand<Window>().WithSubscribe(Exit);

        ChangePreviewImage();
    }

    #endregion Constructors

    #region Properties

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
    /// エラーメッセージの表示状態です。
    /// </summary>
    /// <value>エラーメッセージの表示状態</value>
    public IReactiveProperty<Visibility> ErrorMessageVisibility { get; }

    /// <summary>
    /// カメラ名です。
    /// </summary>
    /// <value>カメラ名</value>
    public IReadOnlyReactiveProperty<string?> CameraName { get; }

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
                    .Contrast(ImageContrast.Value)
                    .Brightness(ImageBrightness.Value)
                    .Saturation(ImageSaturation.Value)
                    .Gamma(ImageGamma.Value);

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

        window.Close();
    }

    /// <summary>
    /// 処理を終了します。
    /// </summary>
    /// <param name="window">ウィンドウ</param>
    private void Exit(Window window)
    {
        window.Close();
    }

    /// <inheritdoc cref="IDisposable.Dispose" />
    public void Dispose()
    {
        disposable.Dispose();
    }

    #endregion Methods
}
