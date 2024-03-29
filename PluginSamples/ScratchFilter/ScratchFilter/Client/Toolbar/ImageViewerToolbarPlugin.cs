﻿/*
 * MIT License
 *
 * (c) 2020 Akihiro Hamana
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System;
using System.Windows.Forms;

using VideoOS.Platform;
using VideoOS.Platform.Client;

namespace ScratchFilter.Client.Toolbar;

/// <summary>
/// イメージビューワを扱うツールバー用プラグインのインスタンスの抽象クラスです。
/// </summary>
/// <seealso cref="ToolbarPluginInstance" />
[ToString]
internal abstract class ImageViewerToolbarPluginInstance : ToolbarPluginInstance
{
    #region Properties

    /// <summary>
    /// 映像を表示するカメラの ID です。
    /// </summary>
    /// <value>映像を表示するカメラの ID</value>
    private protected Guid CameraId { get; private set; }

    #endregion Properties

    #region Methods

    /// <summary>
    /// イメージビューワのイベントを登録します。
    /// </summary>
    /// <param name="imageViewerAddOn">イメージビューワのアドオン</param>
    private void RegisterImageViewerEvents(ImageViewerAddOn imageViewerAddOn)
    {
        imageViewerAddOn.CloseEvent += OnImageViewerClose;
        imageViewerAddOn.UserControlSizeOrLocationChangedEvent += OnImageViewerUserControlSizeOrLocationChanged;
        imageViewerAddOn.IndependentPlaybackModeChangedEvent += OnImageViewerIndependentPlaybackModeChanged;
        imageViewerAddOn.LiveStreamInformationEvent += OnImageViewerLiveStreamInformation;
        imageViewerAddOn.StartLiveEvent += OnImageViewerStartLive;
        imageViewerAddOn.StopLiveEvent += OnImageViewerStopLive;
        imageViewerAddOn.PropertyChangedEvent += OnImageViewerPropertyChanged;
        imageViewerAddOn.ImageDisplayedEvent += OnImageViewerImageDisplayed;
        imageViewerAddOn.RecordedImageReceivedEvent += OnImageViewerRecordedImageReceived;
        imageViewerAddOn.MouseClickEvent += OnImageViewerMouseClick;
        imageViewerAddOn.MouseDoubleClickEvent += OnImageViewerMouseDoubleClick;
        imageViewerAddOn.MouseMoveEvent += OnImageViewerMouseMove;
        imageViewerAddOn.MouseRightClickEvent += OnImageViewerMouseRightClick;
    }

    /// <summary>
    /// イメージビューワのイベントの登録を解除します。
    /// </summary>
    /// <param name="imageViewerAddOn">イメージビューワのアドオン</param>
    private void UnregisterImageViewerEvents(ImageViewerAddOn imageViewerAddOn)
    {
        imageViewerAddOn.CloseEvent -= OnImageViewerClose;
        imageViewerAddOn.UserControlSizeOrLocationChangedEvent -= OnImageViewerUserControlSizeOrLocationChanged;
        imageViewerAddOn.IndependentPlaybackModeChangedEvent -= OnImageViewerIndependentPlaybackModeChanged;
        imageViewerAddOn.LiveStreamInformationEvent -= OnImageViewerLiveStreamInformation;
        imageViewerAddOn.StartLiveEvent -= OnImageViewerStartLive;
        imageViewerAddOn.StopLiveEvent -= OnImageViewerStopLive;
        imageViewerAddOn.PropertyChangedEvent -= OnImageViewerPropertyChanged;
        imageViewerAddOn.ImageDisplayedEvent -= OnImageViewerImageDisplayed;
        imageViewerAddOn.RecordedImageReceivedEvent -= OnImageViewerRecordedImageReceived;
        imageViewerAddOn.MouseClickEvent -= OnImageViewerMouseClick;
        imageViewerAddOn.MouseDoubleClickEvent -= OnImageViewerMouseDoubleClick;
        imageViewerAddOn.MouseMoveEvent -= OnImageViewerMouseMove;
        imageViewerAddOn.MouseRightClickEvent -= OnImageViewerMouseRightClick;
    }

    /// <summary>
    /// イメージビューワが追加された時に発生します。
    /// </summary>
    /// <param name="imageViewerAddOn">イメージビューワのアドオン</param>
    private void OnNewImageViewerControl(ImageViewerAddOn imageViewerAddOn)
    {
        if (CameraId != Guid.Empty)
        {
            return;
        }

        CameraId = imageViewerAddOn.CameraFQID.ObjectId;

        RegisterImageViewerEvents(imageViewerAddOn);
    }

    /// <summary>
    /// イメージビューワが破棄された時に発生します。
    /// </summary>
    /// <param name="sender">イベントハンドラがアタッチされるオブジェクト</param>
    /// <param name="e">イベントのデータ</param>
    private void OnImageViewerClose(object sender, EventArgs e)
    {
        if (sender is not ImageViewerAddOn imageViewerAddOn)
        {
            return;
        }

        UnregisterImageViewerEvents(imageViewerAddOn);
    }

    /// <summary>
    /// イメージビューワのサイズ、表示位置が変更された時に発生します。
    /// </summary>
    /// <param name="sender">イベントハンドラがアタッチされるオブジェクト</param>
    /// <param name="e">イベントのデータ</param>
    private void OnImageViewerUserControlSizeOrLocationChanged(object sender, EventArgs e)
    {
        if (sender is not ImageViewerAddOn imageViewerAddOn)
        {
            return;
        }

        OnImageViewerUserControlSizeOrLocationChanged(imageViewerAddOn, e);
    }

    /// <summary>
    /// イメージビューワの独立再生モードの有効・無効が切り替わった時に発生します。
    /// </summary>
    /// <param name="sender">イベントハンドラがアタッチされるオブジェクト</param>
    /// <param name="e">イベントのデータ</param>
    private void OnImageViewerIndependentPlaybackModeChanged(object sender, IndependentPlaybackModeEventArgs e)
    {
        if (sender is not ImageViewerAddOn imageViewerAddOn)
        {
            return;
        }

        OnImageViewerIndependentPlaybackModeChanged(imageViewerAddOn, e);
    }

    /// <summary>
    /// イメージビューワのライブストリームの XML の情報が利用可能になった時に発生します。
    /// </summary>
    /// <param name="sender">イベントハンドラがアタッチされるオブジェクト</param>
    /// <param name="e">イベントのデータ</param>
    private void OnImageViewerLiveStreamInformation(object sender, LiveStreamInformationEventArgs e)
    {
        if (sender is not ImageViewerAddOn imageViewerAddOn)
        {
            return;
        }

        OnImageViewerLiveStreamInformation(imageViewerAddOn, e);
    }

    /// <summary>
    /// イメージビューワの表示モードが「ライブ」に変更された時に発生します。
    /// </summary>
    /// <param name="sender">イベントハンドラがアタッチされるオブジェクト</param>
    /// <param name="e">イベントのデータ</param>
    private void OnImageViewerStartLive(object sender, PassRequestEventArgs e)
    {
        if (sender is not ImageViewerAddOn imageViewerAddOn)
        {
            return;
        }

        OnImageViewerStartLive(imageViewerAddOn, e);
    }

    /// <summary>
    /// イメージビューワの表示モードが「再生」、もしくは「設定」に変更された時に発生します。
    /// </summary>
    /// <param name="sender">イベントハンドラがアタッチされるオブジェクト</param>
    /// <param name="e">イベントのデータ</param>
    private void OnImageViewerStopLive(object sender, PassRequestEventArgs e)
    {
        if (sender is not ImageViewerAddOn imageViewerAddOn)
        {
            return;
        }

        OnImageViewerStopLive(imageViewerAddOn, e);
    }

    /// <summary>
    /// イメージビューワのプロパティが変更された時に発生します。
    /// </summary>
    /// <param name="sender">イベントハンドラがアタッチされるオブジェクト</param>
    /// <param name="e">イベントのデータ</param>
    private void OnImageViewerPropertyChanged(object sender, EventArgs e)
    {
        if (sender is not ImageViewerAddOn imageViewerAddOn)
        {
            return;
        }

        OnImageViewerPropertyChanged(imageViewerAddOn, e);
    }

    /// <summary>
    /// イメージビューワにライブ映像の画像が表示された時に発生します。
    /// </summary>
    /// <param name="sender">イベントハンドラがアタッチされるオブジェクト</param>
    /// <param name="e">イベントのデータ</param>
    private void OnImageViewerImageDisplayed(object sender, ImageDisplayedEventArgs e)
    {
        if (sender is not ImageViewerAddOn imageViewerAddOn)
        {
            return;
        }

        OnImageViewerImageDisplayed(imageViewerAddOn, e);
    }

    /// <summary>
    /// イメージビューワに録画映像の画像が表示された時に発生します。
    /// </summary>
    /// <param name="sender">イベントハンドラがアタッチされるオブジェクト</param>
    /// <param name="e">イベントのデータ</param>
    private void OnImageViewerRecordedImageReceived(object sender, RecordedImageReceivedEventArgs e)
    {
        if (sender is not ImageViewerAddOn imageViewerAddOn)
        {
            return;
        }

        OnImageViewerRecordedImageReceived(imageViewerAddOn, e);
    }

    /// <summary>
    /// イメージビューワ上でマウスがクリックされた時に発生します。
    /// </summary>
    /// <param name="sender">イベントハンドラがアタッチされるオブジェクト</param>
    /// <param name="e">イベントのデータ</param>
    private void OnImageViewerMouseClick(object sender, MouseEventArgs e)
    {
        if (sender is not ImageViewerAddOn imageViewerAddOn)
        {
            return;
        }

        OnImageViewerMouseClick(imageViewerAddOn, e);
    }

    /// <summary>
    /// イメージビューワ上でマウスがダブルクリックされた時に発生します。
    /// </summary>
    /// <param name="sender">イベントハンドラがアタッチされるオブジェクト</param>
    /// <param name="e">イベントのデータ</param>
    private void OnImageViewerMouseDoubleClick(object sender, MouseEventArgs e)
    {
        if (sender is not ImageViewerAddOn imageViewerAddOn)
        {
            return;
        }

        OnImageViewerMouseDoubleClick(imageViewerAddOn, e);
    }

    /// <summary>
    /// イメージビューワ上でマウスが動かされた時に発生します。
    /// </summary>
    /// <param name="sender">イベントハンドラがアタッチされるオブジェクト</param>
    /// <param name="e">イベントのデータ</param>
    private void OnImageViewerMouseMove(object sender, MouseEventArgs e)
    {
        if (sender is not ImageViewerAddOn imageViewerAddOn)
        {
            return;
        }

        OnImageViewerMouseMove(imageViewerAddOn, e);
    }

    /// <summary>
    /// イメージビューワ上でマウスが右クリックされた時に発生します。
    /// </summary>
    /// <param name="sender">イベントハンドラがアタッチされるオブジェクト</param>
    /// <param name="e">イベントのデータ</param>
    private void OnImageViewerMouseRightClick(object sender, MouseEventArgs e)
    {
        if (sender is not ImageViewerAddOn imageViewerAddOn)
        {
            return;
        }

        OnImageViewerMouseRightClick(imageViewerAddOn, e);
    }

    /// <summary>
    /// イメージビューワのサイズ、表示位置が変更された時に発生します。
    /// </summary>
    /// <param name="imageViewerAddOn">イメージビューワのアドオン</param>
    /// <param name="e">イベントのデータ</param>
    private protected virtual void OnImageViewerUserControlSizeOrLocationChanged(ImageViewerAddOn imageViewerAddOn, EventArgs e) { }

    /// <summary>
    /// イメージビューワの独立再生モードの有効・無効が切り替わった時に発生します。
    /// </summary>
    /// <param name="imageViewerAddOn">イメージビューワのアドオン</param>
    /// <param name="e">イベントのデータ</param>
    private protected virtual void OnImageViewerIndependentPlaybackModeChanged(ImageViewerAddOn imageViewerAddOn, IndependentPlaybackModeEventArgs e) { }

    /// <summary>
    /// イメージビューワのライブストリームの XML の情報が利用可能になった時に発生します。
    /// </summary>
    /// <param name="imageViewerAddOn">イメージビューワのアドオン</param>
    /// <param name="e">イベントのデータ</param>
    private protected virtual void OnImageViewerLiveStreamInformation(ImageViewerAddOn imageViewerAddOn, LiveStreamInformationEventArgs e) { }

    /// <summary>
    /// イメージビューワの表示モードが「ライブ」に変更された時に発生します。
    /// </summary>
    /// <param name="imageViewerAddOn">イメージビューワのアドオン</param>
    /// <param name="e">イベントのデータ</param>
    private protected virtual void OnImageViewerStartLive(ImageViewerAddOn imageViewerAddOn, PassRequestEventArgs e) { }

    /// <summary>
    /// イメージビューワの表示モードが「再生」、もしくは「設定」に変更された時に発生します。
    /// </summary>
    /// <param name="imageViewerAddOn">イメージビューワのアドオン</param>
    /// <param name="e">イベントのデータ</param>
    private protected virtual void OnImageViewerStopLive(ImageViewerAddOn imageViewerAddOn, PassRequestEventArgs e) { }

    /// <summary>
    /// イメージビューワのプロパティが変更された時に発生します。
    /// </summary>
    /// <param name="imageViewerAddOn">イメージビューワのアドオン</param>
    /// <param name="e">イベントのデータ</param>
    private protected virtual void OnImageViewerPropertyChanged(ImageViewerAddOn imageViewerAddOn, EventArgs e) { }

    /// <summary>
    /// イメージビューワにライブ映像の画像が表示された時に発生します。
    /// </summary>
    /// <param name="imageViewerAddOn">イメージビューワのアドオン</param>
    /// <param name="e">イベントのデータ</param>
    private protected virtual void OnImageViewerImageDisplayed(ImageViewerAddOn imageViewerAddOn, ImageDisplayedEventArgs e) { }

    /// <summary>
    /// イメージビューワに録画映像の画像が表示された時に発生します。
    /// </summary>
    /// <param name="imageViewerAddOn">イメージビューワのアドオン</param>
    /// <param name="e">イベントのデータ</param>
    private protected virtual void OnImageViewerRecordedImageReceived(ImageViewerAddOn imageViewerAddOn, RecordedImageReceivedEventArgs e) { }

    /// <summary>
    /// イメージビューワ上でマウスがクリックされた時に発生します。
    /// </summary>
    /// <param name="imageViewerAddOn">イメージビューワのアドオン</param>
    /// <param name="e">イベントのデータ</param>
    private protected virtual void OnImageViewerMouseClick(ImageViewerAddOn imageViewerAddOn, MouseEventArgs e) { }

    /// <summary>
    /// イメージビューワ上でマウスがダブルクリックされた時に発生します。
    /// </summary>
    /// <param name="imageViewerAddOn">イメージビューワのアドオン</param>
    /// <param name="e">イベントのデータ</param>
    private protected virtual void OnImageViewerMouseDoubleClick(ImageViewerAddOn imageViewerAddOn, MouseEventArgs e) { }

    /// <summary>
    /// イメージビューワ上でマウスが動かされた時に発生します。
    /// </summary>
    /// <param name="imageViewerAddOn">イメージビューワのアドオン</param>
    /// <param name="e">イベントのデータ</param>
    private protected virtual void OnImageViewerMouseMove(ImageViewerAddOn imageViewerAddOn, MouseEventArgs e) { }

    /// <summary>
    /// イメージビューワ上でマウスが右クリックされた時に発生します。
    /// </summary>
    /// <param name="imageViewerAddOn">イメージビューワのアドオン</param>
    /// <param name="e">イベントのデータ</param>
    private protected virtual void OnImageViewerMouseRightClick(ImageViewerAddOn imageViewerAddOn, MouseEventArgs e) { }

    /// <inheritdoc />
    public override void Init(Item viewItemInstance, Item window)
    {
        base.Init(viewItemInstance, window);

        ClientControl.Instance.NewImageViewerControlEvent += OnNewImageViewerControl;

        Enabled = true;
    }

    /// <inheritdoc />
    public override void Close()
    {
        base.Close();

        ClientControl.Instance.NewImageViewerControlEvent -= OnNewImageViewerControl;
    }

    #endregion Methods
}

/// <summary>
/// イメージビューワを扱うツールバー用プラグインの抽象クラスです。
/// </summary>
/// <seealso cref="ToolbarPlugin" />
[ToString]
internal abstract class ImageViewerToolbarPlugin : ToolbarPlugin { }
