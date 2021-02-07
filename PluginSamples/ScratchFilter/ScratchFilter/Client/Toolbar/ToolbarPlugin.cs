//
// (C) 2020 Canon Inc.
//
// This file contains trade secrets of Canon Inc. No part may be reproduced
// or transmitted in any form by any means or for any purpose without the
// express written permission of Canon Inc.
//

using System;
using System.Collections.Generic;

using VideoOS.Platform;
using VideoOS.Platform.Client;
using VideoOS.Platform.Messaging;

namespace ScratchFilter.Client.Toolbar
{
    /// <summary>
    /// ツールバー用プラグインのインスタンスのベースとなる抽象クラスです。
    /// </summary>
    /// <seealso cref="ViewItemToolbarPluginInstance" />
    /// <seealso cref="IDisposable" />
    [ToString]
    internal abstract class ToolbarPluginInstance : ViewItemToolbarPluginInstance, IDisposable
    {
        #region Fields

        /// <summary>
        /// メッセージの受信オブジェクトです。
        /// </summary>
        private readonly List<object> messageReceivers = new List<object>();

        /// <summary>
        /// アンマネージリソースが解放されたかどうかです。
        /// </summary>
        private bool disposed;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        protected ToolbarPluginInstance()
        {
            Enabled = false;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// 表示先のウィンドウです。
        /// </summary>
        /// <value>表示先のウィンドウ</value>
        protected Item? Window { get; private set; }

        /// <summary>
        /// 表示先のモニターです。
        /// </summary>
        /// <value>表示先のモニター</value>
        protected Item? Monitor { get; private set; }

        /// <summary>
        /// プラグインの説明です。
        /// </summary>
        /// <value>プラグインの説明</value>
        protected virtual string? Description { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// テーマが変更された時に発生します。
        /// </summary>
        /// <param name="message">テーマ変更の内容</param>
        /// <param name="destination">通知先</param>
        /// <param name="sender">通知元</param>
        /// <returns><c>null</c></returns>
        private object? ThemeChangedIndicationReceiver(Message message, FQID destination, FQID sender)
        {
            ReloadIcon();

            return null;
        }

        /// <summary>
        /// アンマネージリソースを解放し、必要に応じてマネージリソースも解放します。
        /// </summary>
        /// <param name="disposing">マネージリソースとアンマネージリソースの両方を解放する場合は <c>true</c>。アンマネージリソースだけを解放する場合は <c>false</c>。</param>
        private void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                Icon?.Dispose();
            }

            disposed = true;
        }

        /// <summary>
        /// アイコンを読み込みます。
        /// </summary>
        protected abstract void LoadIcon();

        /// <summary>
        /// アイコンを再読み込みします。
        /// </summary>
        protected void ReloadIcon()
        {
            // Dispose メソッドの実行のみとして、null は設定しない。 ※ null を設定すると、デフォルトアイコンが一瞬表示されてしまう。
            Icon?.Dispose();

            LoadIcon();
        }

        /// <summary>
        /// ツールバー用プラグインのインスタンスが UI に追加された時に発生します。
        /// </summary>
        /// <param name="viewItemInstance">ツールバー用プラグインの表示先のモニター</param>
        /// <param name="window">ツールバー用プラグインの表示先のウィンドウ</param>
        public override void Init(Item viewItemInstance, Item window)
        {
            // ※「再生タブ → エクスポート → カメラ選択」を行うと、window が null の状態で呼ばれる。
            if (window is null)
            {
                return;
            }

            Window = window;
            Monitor = viewItemInstance;
            Title = Description;
            Tooltip = Description;

            LoadIcon();

            messageReceivers.Add(EnvironmentManager.Instance.RegisterReceiver(ThemeChangedIndicationReceiver,
                                                                              new MessageIdFilter(MessageId.SmartClient.ThemeChangedIndication)));
        }

        /// <summary>
        /// 終了処理を行います。
        /// </summary>
        public override void Close()
        {
            if (Window is null)
            {
                return;
            }

            messageReceivers.ForEach(EnvironmentManager.Instance.UnRegisterReceiver);

            Dispose();
        }

        /// <summary>
        /// アンマネージリソースの解放またはリセットに関連付けられているアプリケーション定義のタスクを実行します。
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion Methods
    }

    /// <summary>
    /// ツールバー用プラグインのベースとなる抽象クラスです。
    /// </summary>
    /// <seealso cref="ViewItemToolbarPlugin" />
    [ToString]
    internal abstract class ToolbarPlugin : ViewItemToolbarPlugin
    {
        #region Properties

        /// <summary>
        /// 名前です。
        /// </summary>
        /// <remarks>Smart Client の UI のどこにも表示されず、使用されることはありません。</remarks>
        /// <value>名前</value>
        public sealed override string? Name { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// 初期化処理を行います。
        /// </summary>
        public override void Init()
        {
            ViewItemToolbarPlaceDefinition.ViewItemIds = new List<Guid>
            {
                ViewAndLayoutItem.CameraBuiltinId
            };

            ViewItemToolbarPlaceDefinition.WorkSpaceIds = new List<Guid>
            {
                ClientControl.LiveBuildInWorkSpaceId,
                ClientControl.PlaybackBuildInWorkSpaceId
            };

            ViewItemToolbarPlaceDefinition.WorkSpaceStates = new List<WorkSpaceState>
            {
                WorkSpaceState.Normal
            };
        }

        /// <summary>
        /// 終了処理を行います。
        /// </summary>
        public override void Close() { }

        #endregion Methods
    }
}
