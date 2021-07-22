//
// (C) 2020 Canon Inc.
//
// This file contains trade secrets of Canon Inc. No part may be reproduced
// or transmitted in any form by any means or for any purpose without the
// express written permission of Canon Inc.
//

using System;
using System.Windows.Forms;

using VideoOS.Platform;
using VideoOS.Platform.Client;

namespace ScratchFilter.Client.Toolbar
{
    /// <summary>
    /// イメージビューワを扱うツールバー用プラグインのインスタンスの抽象クラスです。
    /// </summary>
    /// <seealso cref="ToolbarPluginInstance" />
    [ToString]
    internal abstract class ImageViewerToolbarPluginInstance : ToolbarPluginInstance
    {
        #region Properties

        /// <summary>
        /// 映像が表示されているカメラの ID です。
        /// </summary>
        /// <value>映像が表示されているカメラの ID</value>
        private Guid CameraId
        {
            get
            {
                Guid.TryParse(Monitor?.Properties["CurrentCameraId"], out var cameraId);

                return cameraId;
            }
        }

        /// <summary>
        /// イメージビューワのアドオンです。
        /// </summary>
        /// <value>イメージビューワのアドオン</value>
        private protected ImageViewerAddOn? ImageViewerAddOn { get; private set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// イメージビューワのイベントを登録します。
        /// </summary>
        private void RegisterImageViewerEvents()
        {
            if (ImageViewerAddOn is null)
            {
                return;
            }

            ImageViewerAddOn.CloseEvent += OnImageViewerClose;
            ImageViewerAddOn.UserControlSizeOrLocationChangedEvent += OnImageViewerUserControlSizeOrLocationChanged;
            ImageViewerAddOn.LiveStreamInformationEvent += OnImageViewerLiveStreamInformation;
            ImageViewerAddOn.StartLiveEvent += OnImageViewerStartLive;
            ImageViewerAddOn.StopLiveEvent += OnImageViewerStopLive;
            ImageViewerAddOn.PropertyChangedEvent += OnImageViewerPropertyChanged;
            ImageViewerAddOn.ImageDisplayedEvent += OnImageViewerImageDisplayed;
            ImageViewerAddOn.RecordedImageReceivedEvent += OnImageViewerRecordedImageReceived;
            ImageViewerAddOn.MouseClickEvent += OnImageViewerMouseClick;
            ImageViewerAddOn.MouseDoubleClickEvent += OnImageViewerMouseDoubleClick;
            ImageViewerAddOn.MouseMoveEvent += OnImageViewerMouseMove;
            ImageViewerAddOn.MouseRightClickEvent += OnImageViewerMouseRightClick;
        }

        /// <summary>
        /// イメージビューワのイベントの登録を解除します。
        /// </summary>
        private void UnregisterImageViewerEvents()
        {
            if (ImageViewerAddOn is null)
            {
                return;
            }

            ImageViewerAddOn.CloseEvent -= OnImageViewerClose;
            ImageViewerAddOn.UserControlSizeOrLocationChangedEvent -= OnImageViewerUserControlSizeOrLocationChanged;
            ImageViewerAddOn.LiveStreamInformationEvent -= OnImageViewerLiveStreamInformation;
            ImageViewerAddOn.StartLiveEvent -= OnImageViewerStartLive;
            ImageViewerAddOn.StopLiveEvent -= OnImageViewerStopLive;
            ImageViewerAddOn.PropertyChangedEvent -= OnImageViewerPropertyChanged;
            ImageViewerAddOn.ImageDisplayedEvent -= OnImageViewerImageDisplayed;
            ImageViewerAddOn.RecordedImageReceivedEvent -= OnImageViewerRecordedImageReceived;
            ImageViewerAddOn.MouseClickEvent -= OnImageViewerMouseClick;
            ImageViewerAddOn.MouseDoubleClickEvent -= OnImageViewerMouseDoubleClick;
            ImageViewerAddOn.MouseMoveEvent -= OnImageViewerMouseMove;
            ImageViewerAddOn.MouseRightClickEvent -= OnImageViewerMouseRightClick;
        }

        /// <summary>
        /// イメージビューワが追加された時に発生します。
        /// </summary>
        /// <param name="imageViewerAddOn">イメージビューワのアドオン</param>
        private void OnNewImageViewerControl(ImageViewerAddOn imageViewerAddOn)
        {
            if (ImageViewerAddOn is not null)
            {
                return;
            }

            if (CameraId == imageViewerAddOn.CameraFQID.ObjectId)
            {
                ImageViewerAddOn = imageViewerAddOn;

                RegisterImageViewerEvents();
            }
        }

        /// <summary>
        /// イメージビューワが破棄された時に発生します。
        /// </summary>
        /// <param name="sender">イベントハンドラがアタッチされるオブジェクト</param>
        /// <param name="e">イベントのデータ</param>
        private void OnImageViewerClose(object sender, EventArgs e)
        {
            UnregisterImageViewerEvents();
        }

        /// <summary>
        /// イメージビューワのサイズ、表示位置が変更された時に発生します。
        /// </summary>
        /// <param name="sender">イベントハンドラがアタッチされるオブジェクト</param>
        /// <param name="e">イベントのデータ</param>
        private protected virtual void OnImageViewerUserControlSizeOrLocationChanged(object sender, EventArgs e) { }

        /// <summary>
        /// イメージビューワのライブストリームの XML の情報が利用可能になった時に発生します。
        /// </summary>
        /// <param name="sender">イベントハンドラがアタッチされるオブジェクト</param>
        /// <param name="e">イベントのデータ</param>
        private protected virtual void OnImageViewerLiveStreamInformation(object sender, LiveStreamInformationEventArgs e) { }

        /// <summary>
        /// イメージビューワの表示モードが「ライブ」に変更された時に発生します。
        /// </summary>
        /// <param name="sender">イベントハンドラがアタッチされるオブジェクト</param>
        /// <param name="e">イベントのデータ</param>
        private protected virtual void OnImageViewerStartLive(object sender, PassRequestEventArgs e) { }

        /// <summary>
        /// イメージビューワの表示モードが「再生」もしくは「設定」に変更された時に発生します。
        /// </summary>
        /// <param name="sender">イベントハンドラがアタッチされるオブジェクト</param>
        /// <param name="e">イベントのデータ</param>
        private protected virtual void OnImageViewerStopLive(object sender, PassRequestEventArgs e) { }

        /// <summary>
        /// イメージビューワのプロパティが変更された時に発生します。
        /// </summary>
        /// <param name="sender">イベントハンドラがアタッチされるオブジェクト</param>
        /// <param name="e">イベントのデータ</param>
        private protected virtual void OnImageViewerPropertyChanged(object sender, EventArgs e) { }

        /// <summary>
        /// イメージビューワにライブ映像の画像が表示された時に発生します。
        /// </summary>
        /// <param name="sender">イベントハンドラがアタッチされるオブジェクト</param>
        /// <param name="e">イベントのデータ</param>
        private protected virtual void OnImageViewerImageDisplayed(object sender, ImageDisplayedEventArgs e) { }

        /// <summary>
        /// イメージビューワに録画映像の画像が表示された時に発生します。
        /// </summary>
        /// <param name="sender">イベントハンドラがアタッチされるオブジェクト</param>
        /// <param name="e">イベントのデータ</param>
        private protected virtual void OnImageViewerRecordedImageReceived(object sender, RecordedImageReceivedEventArgs e) { }

        /// <summary>
        /// イメージビューワ上でマウスがクリックされた時に発生します。
        /// </summary>
        /// <param name="sender">イベントハンドラがアタッチされるオブジェクト</param>
        /// <param name="e">イベントのデータ</param>
        private protected virtual void OnImageViewerMouseClick(object sender, MouseEventArgs e) { }

        /// <summary>
        /// イメージビューワ上でマウスがダブルクリックされた時に発生します。
        /// </summary>
        /// <param name="sender">イベントハンドラがアタッチされるオブジェクト</param>
        /// <param name="e">イベントのデータ</param>
        private protected virtual void OnImageViewerMouseDoubleClick(object sender, MouseEventArgs e) { }

        /// <summary>
        /// イメージビューワ上でマウスが動かされた時に発生します。
        /// </summary>
        /// <param name="sender">イベントハンドラがアタッチされるオブジェクト</param>
        /// <param name="e">イベントのデータ</param>
        private protected virtual void OnImageViewerMouseMove(object sender, MouseEventArgs e) { }

        /// <summary>
        /// イメージビューワ上でマウスが右クリックされた時に発生します。
        /// </summary>
        /// <param name="sender">イベントハンドラがアタッチされるオブジェクト</param>
        /// <param name="e">イベントのデータ</param>
        private protected virtual void OnImageViewerMouseRightClick(object sender, MouseEventArgs e) { }

        /// <summary>
        /// ツールバーのインスタンスが UI に追加されたときに呼ばれます。
        /// </summary>
        /// <param name="viewItemInstance">ツールバーのアイテムに関連したビューアイテムのインスタンス</param>
        /// <param name="window">ツールバーの表示先のウィンドウ</param>
        public override void Init(Item viewItemInstance, Item window)
        {
            // ※「再生タブ → エクスポート → カメラ選択」を行うと、window が null の状態で呼ばれる。
            if (window is null)
            {
                return;
            }

            base.Init(viewItemInstance, window);

            ClientControl.Instance.NewImageViewerControlEvent += OnNewImageViewerControl;

            Enabled = true;
        }

        /// <summary>
        /// 終了処理です。
        /// </summary>
        public override void Close()
        {
            if (Window is null)
            {
                return;
            }

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
}
