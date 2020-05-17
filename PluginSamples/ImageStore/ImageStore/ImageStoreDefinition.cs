using ImageStore.Client.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public class ImageStoreDefinition : PluginDefinition
    {
        #region Fields

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
        private Settings settings;

        /// <summary>
        /// ライブ映像のソースです。
        /// </summary>
        private readonly IDictionary<FQID, LiveSource> liveSources = new Dictionary<FQID, LiveSource>();

        #endregion

        #region Properties

        /// <summary>
        /// Gets the unique id identifying this plugin component
        /// </summary>
        public override Guid Id
        {
            get;
        } = PluginId;

        /// <summary>
        /// Define name of top level Tree node - e.g. A product name
        /// </summary>
        public override string Name
        {
            get;
        } = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductName;

        /// <summary>
        /// Your company name
        /// </summary>
        public override string Manufacturer
        {
            get;
        } = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).CompanyName;

        /// <summary>
        /// Version of this plugin.
        /// </summary>
        public override string VersionString
        {
            get;
        } = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;

        /// <summary>
        /// Icon to be used on top level - e.g. a product or company logo
        /// </summary>
        public override System.Drawing.Image Icon
        {
            get;
        }

        #endregion

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
        /// JPEG が利用可能な場合、または何らかの例外が発生した場合に呼び出されます。
        /// </summary>s
        /// <param name="sender">通知元</param>
        /// <param name="e">イベント引数</param>
        private void JpegLiveContentEvent(object sender, EventArgs e)
        {
            JPEGLiveSource liveSource = sender as JPEGLiveSource;
            LiveContentEventArgs args = e as LiveContentEventArgs;

            if (args == null || args.LiveContent == null)
            {
                return;
            }

            string filePath = GenerateImageFilePath(liveSource.Item.Name);

            string dirPath = Path.GetDirectoryName(filePath);

            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            try
            {
                using (LiveSourceContent liveContent = args.LiveContent)
                using (MemoryStream memoryStream = new MemoryStream(liveContent.Content))
                using (FileStream fileStream = new FileStream(filePath, FileMode.CreateNew))
                {
                    memoryStream.WriteTo(fileStream);
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
                        JPEGLiveSource liveSource = new JPEGLiveSource(new Item(eventSource.FQID, eventSource.Name))
                        {
                            LiveModeStart = true
                        };

                        liveSource.Width = settings.Width;
                        liveSource.Height = settings.Height;
                        liveSource.SetWidthHeight();

                        if (Enumerable.Range(1, 100).Contains(settings.Compression))
                        {
                            liveSource.Compression = settings.Compression;
                        }

                        liveSource.LiveContentEvent += JpegLiveContentEvent;

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
        /// This method is called when the environment is up and running.
        /// Registration of Messages via RegisterReceiver can be done at this point.
        /// </summary>
        public override void Init()
        {
            MessageCommunicationManager.Start(EnvironmentManager.Instance.MasterSite.ServerId);

            messageCommunication = MessageCommunicationManager.Get(EnvironmentManager.Instance.MasterSite.ServerId);

            messageCommunicationFilters.Add(messageCommunication.RegisterCommunicationFilter(NewEventIndicationReceiver,
                                                                                             new CommunicationIdFilter(MessageId.Server.NewEventIndication)));

            settings = Settings.Load();
        }

        /// <summary>
        /// The main application is about to be in an undetermined state, either logging off or exiting.
        /// You can release resources at this point, it should match what you acquired during Init, so additional call to Init() will work.
        /// </summary>
        public override void Close()
        {
            messageCommunicationFilters.ForEach(messageCommunication.UnRegisterCommunicationFilter);

            MessageCommunicationManager.Stop(EnvironmentManager.Instance.MasterSite.ServerId);
        }

        #endregion
    }
}
