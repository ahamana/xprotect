﻿using System;
using System.Collections.Generic;
using VideoOS.Platform;
using VideoOS.Platform.Client;
using VideoOS.Platform.Messaging;

namespace ScratchFilter.Client.Toolbar
{
    /// <summary>
    /// ツールバー用プラグインのインスタンスのベースとなる抽象クラスです。
    /// </summary>
    internal abstract class ToolbarPluginInstance : ViewItemToolbarPluginInstance, IDisposable
    {
        #region Fields

        /// <summary>
        /// アンマネージリソースが解放されたかどうかです。
        /// </summary>
        private bool disposed;

        /// <summary>
        /// メッセージの受信オブジェクトです。
        /// </summary>
        private readonly List<object> messageReceivers = new List<object>();

        #endregion

        #region Properties

        /// <summary>
        /// 表示先のウィンドウです。
        /// </summary>
        /// <value>
        /// 表示先のウィンドウ
        /// </value>
        protected Item Window
        {
            get;
            private set;
        }

        /// <summary>
        /// 表示先のモニターです。
        /// </summary>
        /// <value>
        /// 表示先のモニター
        /// </value>
        protected Item Monitor
        {
            get;
            private set;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        protected ToolbarPluginInstance()
        {
            Enabled = false;
        }

        #endregion

        #region Methods

        /// <summary>
        /// アイコンを再読み込みします。
        /// </summary>
        private void ReloadIcon()
        {
            // アイコンが生成済みの場合
            if (Icon != null)
            {
                // Dispose メソッドの実行のみとして、null 設定はしない。
                // ※ null を設定すると、デフォルトアイコンが一瞬表示されてしまう。
                Icon.Dispose();
            }

            LoadIcon();
        }

        /// <summary>
        /// テーマが変更された時に呼び出されます。
        /// </summary>
        /// <param name="message">テーマ変更の内容</param>
        /// <param name="destination">通知先</param>
        /// <param name="sender">通知元</param>
        /// <returns><c>null</c></returns>
        private object ThemeChangedIndicationReceiver(Message message, FQID destination, FQID sender)
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
                if (Icon != null)
                {
                    Icon.Dispose();
                }
            }

            disposed = true;
        }

        /// <summary>
        /// アイコンを読み込みます。
        /// </summary>
        protected abstract void LoadIcon();

        /// <summary>
        /// ツールバー用プラグインのインスタンスが UI に追加された時に呼び出されます。
        /// </summary>
        /// <param name="viewItemInstance">ツールバー用プラグインの表示先のモニター</param>
        /// <param name="window">ツールバー用プラグインの表示先のウィンドウ</param>
        public override void Init(Item viewItemInstance, Item window)
        {
            // ※「再生タブ → エクスポート → カメラ選択」を行うと、window が null の状態で呼ばれる。
            if (window == null)
            {
                return;
            }

            Window = window;
            Monitor = viewItemInstance;

            LoadIcon();

            messageReceivers.Add(EnvironmentManager.Instance.RegisterReceiver(ThemeChangedIndicationReceiver,
                                                                              new MessageIdFilter(MessageId.SmartClient.ThemeChangedIndication)));
        }

        /// <summary>
        /// 終了処理を行います。
        /// </summary>
        public override void Close()
        {
            if (Window == null)
            {
                return;
            }

            messageReceivers.ForEach(EnvironmentManager.Instance.UnRegisterReceiver);
        }

        /// <summary>
        /// アンマネージリソースの解放またはリセットに関連付けられているアプリケーション定義のタスクを実行します。
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }

    /// <summary>
    /// ツールバー用プラグインのベースとなる抽象クラスです。
    /// </summary>
    /// <typeparam name="S"><see cref="ToolbarPlugin{S}" /> のサブタイプ</typeparam>
    internal abstract class ToolbarPlugin<S> : ViewItemToolbarPlugin where S : ToolbarPlugin<S>
    {
        #region Properties

        /// <summary>
        /// 名前です。
        /// </summary>
        public override string Name
        {
            get;
        } = typeof(S).Name;

        #endregion

        #region Methods

        /// <summary>
        /// 初期化処理を行います。
        /// </summary>
        public override void Init()
        {
            ViewItemToolbarPlaceDefinition.ViewItemIds = new List<Guid>()
            {
                ViewAndLayoutItem.CameraBuiltinId
            };

            ViewItemToolbarPlaceDefinition.WorkSpaceIds = new List<Guid>()
            {
                ClientControl.LiveBuildInWorkSpaceId,
                ClientControl.PlaybackBuildInWorkSpaceId
            };

            ViewItemToolbarPlaceDefinition.WorkSpaceStates = new List<WorkSpaceState>()
            {
                WorkSpaceState.Normal
            };
        }

        /// <summary>
        /// 終了処理を行います。
        /// </summary>
        public override void Close()
        {
            // Do nothing.
        }

        #endregion
    }
}