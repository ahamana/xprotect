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
        /// イメージビューワのアドオンです。
        /// </summary>
        /// <value>
        /// イメージビューワのアドオン
        /// </value>
        protected ImageViewerAddOn ImageViewerAddOn
        {
            get;
            private set;
        }

        #endregion

        #region Methods

        /// <summary>
        /// 映像が表示されているカメラの ID を取得します。
        /// </summary>
        /// <returns>映像が表示されているカメラの ID</returns>
        private Guid GetCurrentCameraId()
        {
            Guid.TryParse(Monitor.Properties["CurrentCameraId"], out Guid cameraId);

            return cameraId;
        }

        /// <summary>
        /// イメージビューワのイベントを登録します。
        /// </summary>
        private void RegisterImageViewerEvents()
        {
            if (ImageViewerAddOn == null)
            {
                return;
            }

            ImageViewerAddOn.CloseEvent += ImageViewerCloseEventHandler;
            ImageViewerAddOn.UserControlSizeOrLocationChangedEvent += ImageViewerUserControlSizeOrLocationChangedEventHandler;
            ImageViewerAddOn.LiveStreamInformationEvent += ImageViewerLiveStreamInformationEventHandler;
            ImageViewerAddOn.StartLiveEvent += ImageViewerStartLiveEventHandler;
            ImageViewerAddOn.StopLiveEvent += ImageViewerStopLiveEventHandler;
            ImageViewerAddOn.PropertyChangedEvent += ImageViewerPropertyChangedEventHandler;
            ImageViewerAddOn.ImageDisplayedEvent += ImageViewerImageDisplayedEventHandler;
            ImageViewerAddOn.RecordedImageReceivedEvent += ImageViewerRecordedImageReceivedEventHandler;
            ImageViewerAddOn.MouseClickEvent += ImageViewerMouseClickEventHandler;
            ImageViewerAddOn.MouseDoubleClickEvent += ImageViewerMouseDoubleClickEventHandler;
            ImageViewerAddOn.MouseMoveEvent += ImageViewerMouseMoveEventHandler;
            ImageViewerAddOn.MouseRightClickEvent += ImageViewerMouseRightClickEventHandler;
        }

        /// <summary>
        /// イメージビューワのイベントの登録を解除します。
        /// </summary>
        private void UnregisterImageViewerEvents()
        {
            if (ImageViewerAddOn == null)
            {
                return;
            }

            ImageViewerAddOn.CloseEvent -= ImageViewerCloseEventHandler;
            ImageViewerAddOn.UserControlSizeOrLocationChangedEvent -= ImageViewerUserControlSizeOrLocationChangedEventHandler;
            ImageViewerAddOn.LiveStreamInformationEvent -= ImageViewerLiveStreamInformationEventHandler;
            ImageViewerAddOn.StartLiveEvent -= ImageViewerStartLiveEventHandler;
            ImageViewerAddOn.StopLiveEvent -= ImageViewerStopLiveEventHandler;
            ImageViewerAddOn.PropertyChangedEvent -= ImageViewerPropertyChangedEventHandler;
            ImageViewerAddOn.ImageDisplayedEvent -= ImageViewerImageDisplayedEventHandler;
            ImageViewerAddOn.RecordedImageReceivedEvent -= ImageViewerRecordedImageReceivedEventHandler;
            ImageViewerAddOn.MouseClickEvent -= ImageViewerMouseClickEventHandler;
            ImageViewerAddOn.MouseDoubleClickEvent -= ImageViewerMouseDoubleClickEventHandler;
            ImageViewerAddOn.MouseMoveEvent -= ImageViewerMouseMoveEventHandler;
            ImageViewerAddOn.MouseRightClickEvent -= ImageViewerMouseRightClickEventHandler;
        }

        /// <summary>
        /// イメージビューワが追加された時に呼び出されます。
        /// </summary>
        /// <param name="imageViewerAddOn">イメージビューワのアドオン</param>
        private void NewImageViewerControlEventHandler(ImageViewerAddOn imageViewerAddOn)
        {
            if (ImageViewerAddOn != null)
            {
                return;
            }

            Guid cameraId = GetCurrentCameraId();

            if (cameraId == imageViewerAddOn.CameraFQID.ObjectId)
            {
                ImageViewerAddOn = imageViewerAddOn;

                RegisterImageViewerEvents();
            }
        }

        /// <summary>
        /// イメージビューワが破棄された時に呼び出されます。
        /// </summary>
        /// <param name="sender">通知元</param>
        /// <param name="e">イベント引数</param>
        private void ImageViewerCloseEventHandler(object sender, EventArgs e)
        {
            UnregisterImageViewerEvents();
        }

        /// <summary>
        /// イメージビューワのサイズ、表示位置が変更された時に呼び出されます。
        /// </summary>
        /// <param name="sender">通知元</param>
        /// <param name="e">イベント引数</param>
        protected virtual void ImageViewerUserControlSizeOrLocationChangedEventHandler(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// イメージビューワのライブストリームの XML の情報が利用可能になった時に呼び出されます。
        /// </summary>
        /// <param name="sender">通知元</param>
        /// <param name="e">イベント引数</param>
        protected virtual void ImageViewerLiveStreamInformationEventHandler(object sender, LiveStreamInformationEventArgs e)
        {
        }

        /// <summary>
        /// イメージビューワの表示モードが「ライブ」に変更された時に呼び出されます。
        /// </summary>
        /// <param name="sender">通知元</param>
        /// <param name="e">イベント引数</param>
        protected virtual void ImageViewerStartLiveEventHandler(object sender, PassRequestEventArgs e)
        {
        }

        /// <summary>
        /// イメージビューワの表示モードが「再生」もしくは「設定」に変更された時に呼び出されます。
        /// </summary>
        /// <param name="sender">通知元</param>
        /// <param name="e">イベント引数</param>
        protected virtual void ImageViewerStopLiveEventHandler(object sender, PassRequestEventArgs e)
        {
        }

        /// <summary>
        /// イメージビューワのプロパティが変更された時に呼び出されます。
        /// </summary>
        /// <param name="sender">通知元</param>
        /// <param name="e">イベント引数</param>
        protected virtual void ImageViewerPropertyChangedEventHandler(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// イメージビューワにライブ映像の画像が表示された時に呼び出されます。
        /// </summary>
        /// <param name="sender">通知元</param>
        /// <param name="e">イベント引数</param>
        protected virtual void ImageViewerImageDisplayedEventHandler(object sender, ImageDisplayedEventArgs e)
        {
        }

        /// <summary>
        /// イメージビューワに録画映像の画像が表示された時に呼び出されます。
        /// </summary>
        /// <param name="sender">通知元</param>
        /// <param name="e">イベント引数</param>
        protected virtual void ImageViewerRecordedImageReceivedEventHandler(object sender, RecordedImageReceivedEventArgs e)
        {
        }

        /// <summary>
        /// イメージビューワ上でマウスがクリックされた時に呼び出されます。
        /// </summary>
        /// <param name="sender">通知元</param>
        /// <param name="e">イベント引数</param>
        protected virtual void ImageViewerMouseClickEventHandler(object sender, MouseEventArgs e)
        {
        }

        /// <summary>
        /// イメージビューワ上でマウスがダブルクリックされた時に呼び出されます。
        /// </summary>
        /// <param name="sender">通知元</param>
        /// <param name="e">イベント引数</param>
        protected virtual void ImageViewerMouseDoubleClickEventHandler(object sender, MouseEventArgs e)
        {
        }

        /// <summary>
        /// イメージビューワ上でマウスが動かされた時に呼び出されます。
        /// </summary>
        /// <param name="sender">通知元</param>
        /// <param name="e">イベント引数</param>
        protected virtual void ImageViewerMouseMoveEventHandler(object sender, MouseEventArgs e)
        {
        }

        /// <summary>
        /// イメージビューワ上でマウスが右クリックされた時に呼び出されます。
        /// </summary>
        /// <param name="sender">通知元</param>
        /// <param name="e">イベント引数</param>
        protected virtual void ImageViewerMouseRightClickEventHandler(object sender, MouseEventArgs e)
        {
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

            ClientControl.Instance.NewImageViewerControlEvent += NewImageViewerControlEventHandler;

            Enabled = true;
        }

        /// <summary>
        /// 終了処理です。
        /// </summary>
        public override void Close()
        {
            if (Window == null)
            {
                return;
            }

            base.Close();

            ClientControl.Instance.NewImageViewerControlEvent -= NewImageViewerControlEventHandler;
        }

        #endregion
    }

    /// <summary>
    /// イメージビューワを扱うツールバー用プラグインの抽象クラスです。
    /// </summary>
    /// <seealso cref="ToolbarPlugin" />
    [ToString]
    internal abstract class ImageViewerToolbarPlugin : ToolbarPlugin
    {
    }
}
