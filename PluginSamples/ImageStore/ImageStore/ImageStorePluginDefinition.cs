using ImageStore.Client.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using VideoOS.Platform;
using VideoOS.Platform.Data;
using VideoOS.Platform.Live;
using VideoOS.Platform.Messaging;

namespace ImageStore
{
    /// <summary>
    /// The PluginDefinition is the 'entry' point to any plugin.
    /// This is the starting point for any plugin development and the class MUST be available for a plugin to be loaded.
    /// Several PluginDefinitions are allowed to be available within one DLL.
    /// Here the references to all other plugin known objects and classes are defined.
    /// The class is an abstract class where all implemented methods and properties need to be declared with override.
    /// The class is constructed when the environment is loading the DLL.
    /// </summary>
    /// <seealso cref="VideoOS.Platform.PluginDefinition" />
    public class ImageStorePluginDefinition : PluginDefinition
    {
        #region Fields

        /// <summary>
        /// バージョン情報です。
        /// </summary>
        private static readonly FileVersionInfo FileVersionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);

        /// <summary>
        /// プラグインの ID です。
        /// </summary>
        private static readonly Guid PluginId = new Guid("65accde7-fbac-4c56-b98b-9b4458627bda");

        /// <summary>
        /// メッセージに関する通信です。
        /// </summary>
        private MessageCommunication messageCommunication;

        /// <summary>
        /// メッセージを受信するためのフィルタです。
        /// </summary>
        private readonly List<object> messageCommunicationFilters = new List<object>();

        /// <summary>
        /// 設定です。
        /// </summary>
        private ImageStoreSettings settings;

        /// <summary>
        /// ライブ映像のソースです。
        /// </summary>
        private readonly IDictionary<FQID, LiveSource> liveSources = new Dictionary<FQID, LiveSource>();

        #endregion Fields

        #region Properties

        /// <summary>
        /// ID です。
        /// </summary>
        /// <value>
        /// ID
        /// </value>
        public override Guid Id { get; } = PluginId;

        /// <summary>
        /// 製品名です。
        /// </summary>
        /// <value>
        /// 製品名
        /// </value>
        public override string Name { get; } = FileVersionInfo.ProductName;

        /// <summary>
        /// 会社名です。
        /// </summary>
        /// <value>
        /// 会社名
        /// </value>
        public override string Manufacturer { get; } = FileVersionInfo.CompanyName;

        /// <summary>
        /// プラグインのバージョンです。
        /// </summary>
        /// <value>
        /// プラグインのバージョン
        /// </value>
        public override string VersionString { get; } = FileVersionInfo.ProductVersion;

        /// <summary>
        /// トップレベルで使用するアイコンです。
        /// </summary>
        /// <value>
        /// トップレベルで使用するアイコン
        /// </value>
        public override Image Icon { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// 画像を保存するファイルのパスを生成します。
        /// </summary>
        /// <param name="cameraName">カメラ名</param>
        /// <returns>画像を保存するファイルのパス</returns>
        private string GenerateImageFilePath(string cameraName)
        {
            DateTime now = DateTime.Now;

            string filePath = Path.Combine(settings.OutputDir,
                                           now.ToString("yyyy-MM-dd"),
                                           cameraName,
                                           $"{now.ToString("yyyy-MM-dd_HH-mm-ss-fff")}.jpg");

            return filePath;
        }

        /// <summary>
        /// ライブ映像の新しいフレームを取得した時に発生します。
        /// </summary>s
        /// <param name="sender">イベントハンドラがアタッチされるオブジェクト</param>
        /// <param name="e">イベントのデータ</param>
        private void OnLiveSourceLiveContentEvent(object sender, EventArgs e)
        {
            JPEGLiveSource liveSource = sender as JPEGLiveSource;
            LiveContentEventArgs args = e as LiveContentEventArgs;

            if (args == null || args.LiveContent == null)
            {
                return;
            }

            string filePath = GenerateImageFilePath(liveSource.Item.Name);

            string dirPath = Path.GetDirectoryName(filePath);

            Directory.CreateDirectory(dirPath);

            try
            {
                using (LiveSourceContent liveContent = args.LiveContent)
                {
                    File.WriteAllBytes(filePath, liveContent.Content);
                }
            }
            catch
            {
                // Do nothing.
            }
        }

        /// <summary>
        /// 新規イベントが発生した場合に呼び出されます。
        /// </summary>
        /// <param name="message">新規イベントの内容</param>
        /// <param name="destination">通知先</param>
        /// <param name="sender">通知元</param>
        /// <returns>戻り値</returns>
        private object NewEventIndicationReceiver(Message message, FQID destination, FQID sender)
        {
            BaseEvent eventData = message.Data as BaseEvent;

            if (eventData == null)
            {
                return null;
            }

            EventSource eventSource = eventData.EventHeader.Source;

            switch (eventData.EventHeader.Message)
            {
                case "Recording Started":
                    if (!liveSources.ContainsKey(eventSource.FQID))
                    {
                        Item camera = new Item(eventSource.FQID, eventSource.Name);

                        VideoLiveSource liveSource = new JPEGLiveSource(camera)
                        {
                            LiveModeStart = true,
                            // 幅と高さに 0 を指定して、実際の解像度の画像を取得
                            Width = 0,
                            Height = 0
                        };

                        if (Enumerable.Range(1, 100).Contains(settings.Compression))
                        {
                            liveSource.Compression = settings.Compression;
                        }

                        liveSource.LiveContentEvent += OnLiveSourceLiveContentEvent;

                        liveSource.Init();

                        liveSources.Add(eventSource.FQID, liveSource);
                    }

                    break;

                case "Recording Stopped":
                    if (liveSources.ContainsKey(eventSource.FQID))
                    {
                        LiveSource liveSource = liveSources[eventSource.FQID];

                        liveSource.Close();

                        liveSources.Remove(eventSource.FQID);
                    }

                    break;
            }

            return null;
        }

        /// <summary>
        /// 初期化処理を行います。
        /// </summary>
        public override void Init()
        {
            MessageCommunicationManager.Start(EnvironmentManager.Instance.MasterSite.ServerId);

            messageCommunication = MessageCommunicationManager.Get(EnvironmentManager.Instance.MasterSite.ServerId);

            messageCommunicationFilters.Add(messageCommunication.RegisterCommunicationFilter(NewEventIndicationReceiver,
                                                                                             new CommunicationIdFilter(MessageId.Server.NewEventIndication)));

            settings = ImageStoreSettings.Load();
        }

        /// <summary>
        /// 終了処理を行います。
        /// </summary>
        public override void Close()
        {
            messageCommunicationFilters.ForEach(messageCommunication.UnRegisterCommunicationFilter);

            MessageCommunicationManager.Stop(EnvironmentManager.Instance.MasterSite.ServerId);
        }

        #endregion Methods
    }
}
