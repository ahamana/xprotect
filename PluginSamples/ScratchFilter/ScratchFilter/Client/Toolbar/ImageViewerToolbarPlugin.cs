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
        /// <returns>
        /// 映像が表示されているカメラの ID
        /// </returns>
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
            if (ImageViewerAddOn == null)
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
        /// イメージビューワが破棄された時に発生します。
        /// </summary>
        /// <param name="sender">通知元</param>
        /// <param name="e">イベント引数</param>
        private void OnImageViewerClose(object sender, EventArgs e)
        {
            UnregisterImageViewerEvents();
        }

        /// <summary>
        /// イメージビューワのサイズ、表示位置が変更された時に発生します。
        /// </summary>
        /// <param name="sender">通知元</param>
        /// <param name="e">イベント引数</param>
        protected virtual void OnImageViewerUserControlSizeOrLocationChanged(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// イメージビューワのライブストリームの XML の情報が利用可能になった時に発生します。
        /// </summary>
        /// <param name="sender">通知元</param>
        /// <param name="e">イベント引数</param>
        protected virtual void OnImageViewerLiveStreamInformation(object sender, LiveStreamInformationEventArgs e)
        {
        }

        /// <summary>
        /// イメージビューワの表示モードが「ライブ」に変更された時に発生します。
        /// </summary>
        /// <param name="sender">通知元</param>
        /// <param name="e">イベント引数</param>
        protected virtual void OnImageViewerStartLive(object sender, PassRequestEventArgs e)
        {
        }

        /// <summary>
        /// イメージビューワの表示モードが「再生」もしくは「設定」に変更された時に発生します。
        /// </summary>
        /// <param name="sender">通知元</param>
        /// <param name="e">イベント引数</param>
        protected virtual void OnImageViewerStopLive(object sender, PassRequestEventArgs e)
        {
        }

        /// <summary>
        /// イメージビューワのプロパティが変更された時に発生します。
        /// </summary>
        /// <param name="sender">通知元</param>
        /// <param name="e">イベント引数</param>
        protected virtual void OnImageViewerPropertyChanged(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// イメージビューワにライブ映像の画像が表示された時に発生します。
        /// </summary>
        /// <param name="sender">通知元</param>
        /// <param name="e">イベント引数</param>
        protected virtual void OnImageViewerImageDisplayed(object sender, ImageDisplayedEventArgs e)
        {
        }

        /// <summary>
        /// イメージビューワに録画映像の画像が表示された時に発生します。
        /// </summary>
        /// <param name="sender">通知元</param>
        /// <param name="e">イベント引数</param>
        protected virtual void OnImageViewerRecordedImageReceived(object sender, RecordedImageReceivedEventArgs e)
        {
        }

        /// <summary>
        /// イメージビューワ上でマウスがクリックされた時に発生します。
        /// </summary>
        /// <param name="sender">通知元</param>
        /// <param name="e">イベント引数</param>
        protected virtual void OnImageViewerMouseClick(object sender, MouseEventArgs e)
        {
        }

        /// <summary>
        /// イメージビューワ上でマウスがダブルクリックされた時に発生します。
        /// </summary>
        /// <param name="sender">通知元</param>
        /// <param name="e">イベント引数</param>
        protected virtual void OnImageViewerMouseDoubleClick(object sender, MouseEventArgs e)
        {
        }

        /// <summary>
        /// イメージビューワ上でマウスが動かされた時に発生します。
        /// </summary>
        /// <param name="sender">通知元</param>
        /// <param name="e">イベント引数</param>
        protected virtual void OnImageViewerMouseMove(object sender, MouseEventArgs e)
        {
        }

        /// <summary>
        /// イメージビューワ上でマウスが右クリックされた時に発生します。
        /// </summary>
        /// <param name="sender">通知元</param>
        /// <param name="e">イベント引数</param>
        protected virtual void OnImageViewerMouseRightClick(object sender, MouseEventArgs e)
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

            ClientControl.Instance.NewImageViewerControlEvent += OnNewImageViewerControl;

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

            ClientControl.Instance.NewImageViewerControlEvent -= OnNewImageViewerControl;
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
