//
// (C) 2020 Canon Inc.
//
// This file contains trade secrets of Canon Inc. No part may be reproduced
// or transmitted in any form by any means or for any purpose without the
// express written permission of Canon Inc.
//

using System;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
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
    internal sealed class EventCommunication
    {
        #region Fields

        /// <summary>
        /// プラグインの定義です。
        /// </summary>
        private static readonly PluginDefinition PluginDefinition;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// 静的コンストラクタです。
        /// </summary>
        static EventCommunication()
        {
            var pluginId = Guid.Parse(Assembly.GetExecutingAssembly().GetCustomAttribute<GuidAttribute>().Value);

            PluginDefinition = PluginManager.GetPluginDefinition(pluginId);
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// イベントサーバのホスト名を取得します。
        /// </summary>
        /// <returns>イベントサーバのホスト名</returns>
        private string GetEventServerHostname()
        {
            // イベントサーバのサービス URL の情報を取得
            var serviceURIInfo =
                Configuration.Instance.GetRegisteredServiceUriInfo(Configuration.MAPServiceType, EnvironmentManager.Instance.MasterSite.ServerId).First();

            var serviceUri = UrlUtil.BuildUri(serviceURIInfo.UriArray.First());

            return serviceUri.Host;
        }

        /// <summary>
        /// ユーザ定義イベントを通知します。
        /// </summary>
        /// <param name="userDefinedEvent">ユーザ定義イベント</param>
        /// <param name="relatedItem">関連項目</param>
        internal void SendUserDefinedEvent(Item userDefinedEvent, Item? relatedItem = null)
        {
            if (userDefinedEvent.FQID.Kind != Kind.TriggerEvent)
            {
                return;
            }

            var message = new Message(MessageId.Control.TriggerCommand)
            {
                RelatedFQID = relatedItem?.FQID
            };

            EnvironmentManager.Instance.PostMessage(message, userDefinedEvent.FQID);
        }

        /// <summary>
        /// アナリティクスイベントを通知します。
        /// </summary>
        /// <param name="eventName">イベント名</param>
        /// <param name="device">デバイス</param>
        /// <param name="description">説明</param>
        internal void SendAnalyticsEvent(string eventName, Item device, string? description = null)
        {
            if (string.IsNullOrEmpty(eventName))
            {
                return;
            }

            var eventSource = new EventSource
            {
                // Send empty - it is possible to send without an eventsource, but the intended design is that there should be a source.
                // The FQID is primamry used to match up the ObjectId with the camera.
                FQID = device.FQID,
                // If FQID is null, then the Name can be an IP address, and the event server will make a lookup to find the camera.
                Name = device.Name
            };

            var eventHeader = new EventHeader
            {
                ID = Guid.NewGuid(),
                Name = eventName,
                Message = eventName,
                Source = eventSource,
                Timestamp = DateTime.Now
            };

            var vendor = new Vendor
            {
                Name = PluginDefinition.Manufacturer
            };

            var analyticsEvent = new AnalyticsEvent
            {
                EventHeader = eventHeader,
                Location = device.Name,
                Description = description,
                Vendor = vendor
            };

            var message = new Message(MessageId.Server.NewEventCommand)
            {
                Data = analyticsEvent
            };

            EnvironmentManager.Instance.PostMessage(message);
        }

        /// <summary>
        /// ジェネリックイベントを通知します。
        /// </summary>
        /// <param name="dataSource">ジェネリックイベントのデータソース</param>
        /// <param name="text">通知する文字列</param>
        internal void SendGenericEvent(GenericEventDataSource dataSource, string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            using var client = new TcpClient(GetEventServerHostname(), dataSource.DataSourcePort);
            using var stream = client.GetStream();

            var buffer = Encoding.UTF8.GetBytes(text);

            stream.Write(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// アラームを通知します。
        /// </summary>
        /// <param name="alarmName">アラーム名</param>
        /// <param name="device">デバイス</param>
        /// <param name="description">説明</param>
        internal void SendAlarm(string alarmName, Item device, string? description = null)
        {
            if (string.IsNullOrEmpty(alarmName))
            {
                return;
            }

            var eventSource = new EventSource
            {
                // Send empty - it is possible to send without an eventsource, but the intended design is that there should be a source.
                // The FQID is primamry used to match up the ObjectId with the camera.
                FQID = device.FQID,
                // If FQID is null, then the Name can be an IP address, and the event server will make a lookup to find the camera.
                Name = device.Name
            };

            var eventHeader = new EventHeader
            {
                ID = Guid.NewGuid(),
                Name = alarmName,
                Message = alarmName,
                Source = eventSource,
                Timestamp = DateTime.Now
            };

            var vendor = new Vendor
            {
                Name = PluginDefinition.Manufacturer
            };

            var alarm = new Alarm
            {
                EventHeader = eventHeader,
                Location = device.Name,
                Description = description,
                Vendor = vendor
            };

            var message = new Message(MessageId.Server.NewAlarmCommand)
            {
                Data = alarm
            };

            EnvironmentManager.Instance.PostMessage(message);
        }

        #endregion Methods
    }
}
