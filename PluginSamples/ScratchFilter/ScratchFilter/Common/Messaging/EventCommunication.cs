//
// MIT License
//
// (c) 2020 Akihiro Hamana
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
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

namespace ScratchFilter.Common.Messaging;

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

        var eventMessage = new Message(MessageId.Control.TriggerCommand)
        {
            RelatedFQID = relatedItem?.FQID
        };

        EnvironmentManager.Instance.PostMessage(eventMessage, userDefinedEvent.FQID);
    }

    /// <summary>
    /// アナリティクスイベントを通知します。
    /// </summary>
    /// <param name="eventName">イベント名</param>
    /// <param name="device">デバイス</param>
    /// <param name="description">説明</param>
    internal void SendAnalyticsEvent(string eventName, Item device, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(eventName))
        {
            return;
        }

        var eventSource = new EventSource
        {
            FQID = device.FQID
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

        var eventMessage = new Message(MessageId.Server.NewEventCommand)
        {
            Data = analyticsEvent
        };

        EnvironmentManager.Instance.PostMessage(eventMessage);
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
    /// <param name="message">メッセージ</param>
    /// <param name="description">説明</param>
    internal void SendAlarm(string alarmName, Item device, string message, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(alarmName))
        {
            return;
        }

        if (string.IsNullOrEmpty(message))
        {
            return;
        }

        var eventSource = new EventSource
        {
            FQID = device.FQID
        };

        var eventHeader = new EventHeader
        {
            ID = Guid.NewGuid(),
            Name = alarmName,
            Message = message,
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

        var eventMessage = new Message(MessageId.Server.NewAlarmCommand)
        {
            Data = alarm
        };

        EnvironmentManager.Instance.PostMessage(eventMessage);
    }

    #endregion Methods
}
