using OpenCvSharp;
using OpenCvSharp.Extensions;
using ScratchFilter.Properties;
using System;
using System.Drawing;
using System.Windows.Forms;
using VideoOS.Platform;
using VideoOS.Platform.Client;

namespace ScratchFilter.Client.Toolbar
{
    /// <summary>
    /// ツールバーの傷フィルタ機能用インスタンスです。
    /// </summary>
    /// <seealso cref="ToolbarPluginInstance" />
    internal class ScratchFilterToolbarPluginInstance : ToolbarPluginInstance
    {
        #region Fields

        /// <summary>
        /// 作動中であるかどうかです。
        /// </summary>
        private bool isActive;

        /// <summary>
        /// イメージビューワのアドオンです。
        /// </summary>
        private ImageViewerAddOn imageViewerAddOn;

        #endregion

        #region Properties

        /// <summary>
        /// プラグインの説明です。
        /// </summary>
        /// <value>
        /// プラグインの説明
        /// </value>
        private string Description
        {
            get
            {
                if (isActive)
                {
                    return Resources.ToolbarPlugin_Description_Active;
                }
                else
                {
                    return Resources.ToolbarPlugin_Description_Inactive;
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// 映像が表示されているカメラの ID を取得します。
        /// </summary>
        /// <returns>映像が表示されているカメラの ID</returns>
        private Guid GetCurrentCameraId()
        {
            Guid cameraId;

            Guid.TryParse(Monitor.Properties["CurrentCameraId"], out cameraId);

            return cameraId;
        }

        /// <summary>
        /// イメージビューワの表示画像を処理します。
        /// </summary>
        private void HandleDisplayedImage()
        {
            imageViewerAddOn.ClearOverlay(default);

            if (!isActive)
            {
                return;
            }

            using (Bitmap original = imageViewerAddOn.GetCurrentDisplayedImageAsBitmap())
            {
                if (original == null)
                {
                    return;
                }

                using (Mat mat = original.ToMat())
                {
                    Cv2.CvtColor(mat, mat, ColorConversionCodes.RGB2GRAY);

                    using (Bitmap overlay = mat.ToBitmap())
                    {
                        imageViewerAddOn.SetOverlay(overlay, default, true, true, true, 1, DockStyle.None, DockStyle.None, 0, 0);
                    }
                }
            }
        }

        /// <summary>
        /// イメージビューワが追加された時に呼び出されます。
        /// </summary>
        /// <param name="imageViewerAddOn">イメージビューワのアドオン</param>
        private void NewImageViewerControlEventHandler(ImageViewerAddOn imageViewerAddOn)
        {
            if (this.imageViewerAddOn != null)
            {
                return;
            }

            Guid cameraId = GetCurrentCameraId();

            if (cameraId == imageViewerAddOn.CameraFQID.ObjectId)
            {
                imageViewerAddOn.CloseEvent += ImageViewerCloseEventHandler;
                imageViewerAddOn.ImageDisplayedEvent += ImageViewerImageDisplayedEventHandler;
                imageViewerAddOn.RecordedImageReceivedEvent += ImageViewerRecordedImageReceivedEventHandler;

                this.imageViewerAddOn = imageViewerAddOn;
            }
        }

        /// <summary>
        /// イメージビューワが破棄された時に呼び出されます。
        /// </summary>
        /// <param name="sender">通知元</param>
        /// <param name="e">イベント引数</param>
        private void ImageViewerCloseEventHandler(object sender, EventArgs e)
        {
            imageViewerAddOn.CloseEvent -= ImageViewerCloseEventHandler;
            imageViewerAddOn.ImageDisplayedEvent -= ImageViewerImageDisplayedEventHandler;
            imageViewerAddOn.RecordedImageReceivedEvent -= ImageViewerRecordedImageReceivedEventHandler;
        }

        /// <summary>
        /// イメージビューワにライブ映像の画像が表示された時に呼び出されます。
        /// </summary>
        /// <param name="sender">通知元</param>
        /// <param name="e">イベント引数</param>
        private void ImageViewerImageDisplayedEventHandler(object sender, ImageDisplayedEventArgs e)
        {
            HandleDisplayedImage();
        }

        /// <summary>
        /// イメージビューワに録画映像の画像が表示された時に呼び出されます。
        /// </summary>
        /// <param name="sender">通知元</param>
        /// <param name="e">イベント引数</param>
        private void ImageViewerRecordedImageReceivedEventHandler(object sender, RecordedImageReceivedEventArgs e)
        {
            HandleDisplayedImage();
        }

        /// <summary>
        /// アイコンを読み込みます。
        /// </summary>
        protected override void LoadIcon()
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

            Title = Description;
            Tooltip = Description;
            Enabled = true;

            ClientControl.Instance.NewImageViewerControlEvent += NewImageViewerControlEventHandler;
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

        /// <summary>
        /// ツールバー用プラグインのメニューが押下された時に呼び出されます。
        /// </summary>
        public override void Activate()
        {
            isActive = !isActive;

            Title = Description;
            Tooltip = Description;
        }

        #endregion
    }

    /// <summary>
    /// ツールバーの傷フィルタ機能です。
    /// </summary>
    /// <seealso cref="ToolbarPlugin" />
    internal class ScratchFilterToolbarPlugin : ToolbarPlugin
    {
        #region Properties

        /// <summary>
        /// ID です。
        /// </summary>
        /// <value>
        /// ID
        /// </value>
        public override Guid Id
        {
            get;
        } = ScratchFilterDefinition.ToolbarPluginId;

        #endregion

        #region Methods

        /// <summary>
        /// ツールバーの傷フィルタ機能用インスタンスを生成します。
        /// </summary>
        /// <returns>ツールバーの傷フィルタ機能用インスタンス</returns>
        public override ViewItemToolbarPluginInstance GenerateViewItemToolbarPluginInstance()
        {
            return new ScratchFilterToolbarPluginInstance();
        }

        #endregion
    }
}
