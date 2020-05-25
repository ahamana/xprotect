using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using VideoOS.Platform;
using VideoOS.Platform.ConfigurationItems;
using VideoOS.Platform.Data;
using VideoOS.Platform.Messaging;
using VideoOS.Platform.Util;
using AnalyticsEvent = VideoOS.Platform.Data.AnalyticsEvent;

namespace ScratchFilter.Common.Messaging
{
    /// <summary>
    /// イベントに関する通信です。
    /// </summary>
    [ToString]
    internal class EventCommunication
    {
        #region Fields

        /// <summary>
        /// ベンダー名です。
        /// </summary>
        private static readonly string VendorName = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).CompanyName;

        #endregion

        #region Methods

        /// <summary>
        /// イベントサーバのホスト名を取得します。
        /// </summary>
        /// <returns>イベントサーバのホスト名</returns>
        private string GetEventServerHostname()
        {
            // イベントサーバのサービス URL の情報を取得
            Configuration.ServiceURIInfo serviceURIInfo =
                Configuration.Instance.GetRegisteredServiceUriInfo(Configuration.MAPServiceType, EnvironmentManager.Instance.MasterSite.ServerId).First();

            Uri serviceUri = UrlUtil.BuildUri(serviceURIInfo.UriArray.First());

            return serviceUri.Host;
        }

        /// <summary>
        /// ユーザ定義イベントを通知します。
        /// </summary>
        /// <param name="userDefinedEvent">ユーザ定義イベント</param>
        /// <param name="relatedItem">関連項目</param>
        internal void SendUserDefinedEvent(Item userDefinedEvent, Item relatedItem = null)
        {
            if (userDefinedEvent == null)
            {
                return;
            }

            if (userDefinedEvent.FQID.Kind != Kind.TriggerEvent)
            {
                return;
            }

            Message message = new Message(MessageId.Control.TriggerCommand, relatedItem?.FQID);

            EnvironmentManager.Instance.SendMessage(message, userDefinedEvent.FQID);
        }

        /// <summary>
        /// アナリティクスイベントを通知します。
        /// </summary>
        /// <param name="eventName">イベント名</param>
        /// <param name="device">デバイス</param>
        /// <param name="description">説明</param>
        internal void SendAnalyticsEvent(string eventName, Item device, string description = null)
        {
            if (string.IsNullOrEmpty(eventName))
            {
                return;
            }

            if (device == null)
            {
                return;
            }

            EventSource eventSource = new EventSource()
            {
                // Send empty - it is possible to send without an eventsource, but the intended design is that there should be a source
                // the FQID is primamry used to match up the ObjectId with the camera.
                FQID = device.FQID,
                // If FQID is null, then the Name can be an IP address, and the event server will make a lookup to find the camera
                Name = device.Name
            };

            EventHeader eventHeader = new EventHeader()
            {
                ID = Guid.NewGuid(),
                Message = eventName,
                Source = eventSource,
                Timestamp = DateTime.Now
            };

            Vendor vendor = new Vendor
            {
                Name = VendorName
            };

            AnalyticsEvent analyticsEvent = new AnalyticsEvent()
            {
                EventHeader = eventHeader,
                Location = device.Name,
                // Description を設定するとアラーム情報の「手順」に反映される。
                Description = description,
                Vendor = vendor
            };

            Message message = new Message(MessageId.Server.NewEventCommand, analyticsEvent);

            EnvironmentManager.Instance.SendMessage(message);
        }

        /// <summary>
        /// ジェネリックイベントを通知します。
        /// </summary>
        /// <param name="dataSource">ジェネリックイベントのデータソース</param>
        /// <param name="text">通知する文字列</param>
        internal void SendGenericEvent(GenericEventDataSource dataSource, string text)
        {
            if (dataSource == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            using (TcpClient client = new TcpClient(GetEventServerHostname(), dataSource.DataSourcePort))
            using (Stream stream = client.GetStream())
            {
                byte[] buffer = Encoding.UTF8.GetBytes(text);

                stream.Write(buffer, 0, buffer.Length);
            }
        }

        /// <summary>
        /// アラームを通知します。
        /// </summary>
        /// <param name="alarmName">アラーム名</param>
        /// <param name="device">デバイス</param>
        /// <param name="description">説明</param>
        internal void SendAlarm(string alarmName, Item device, string description = null)
        {
            if (string.IsNullOrEmpty(alarmName))
            {
                return;
            }

            if (device == null)
            {
                return;
            }

            EventSource eventSource = new EventSource()
            {
                // Send empty - it is possible to send without an eventsource, but the intended design is that there should be a source
                // the FQID is primamry used to match up the ObjectId with the camera.
                FQID = device.FQID,
                // If FQID is null, then the Name can be an IP address, and the event server will make a lookup to find the camera
                Name = device.Name
            };

            EventHeader eventHeader = new EventHeader()
            {
                ID = Guid.NewGuid(),
                Name = alarmName,
                Message = alarmName,
                Source = eventSource,
                Timestamp = DateTime.Now
            };

            Vendor vendor = new Vendor
            {
                Name = VendorName
            };

            Alarm alarm = new Alarm()
            {
                EventHeader = eventHeader,
                Location = device.Name,
                // Description を設定するとアラーム情報の「手順」に反映される。
                Description = description,
                Vendor = vendor
            };

            Message message = new Message(MessageId.Server.NewAlarmCommand, alarm);

            EnvironmentManager.Instance.SendMessage(message);
        }

        #endregion
    }
}
