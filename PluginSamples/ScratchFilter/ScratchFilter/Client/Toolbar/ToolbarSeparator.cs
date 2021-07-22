﻿//
// (C) 2020 Canon Inc.
//
// This file contains trade secrets of Canon Inc. No part may be reproduced
// or transmitted in any form by any means or for any purpose without the
// express written permission of Canon Inc.
//

using System;
using System.Drawing;

using VideoOS.Platform;
using VideoOS.Platform.Client;
using VideoOS.Platform.UI;

namespace ScratchFilter.Client.Toolbar
{
    /// <summary>
    /// ツールバーのセパレータ用インスタンスです。
    /// </summary>
    /// <seealso cref="ToolbarPluginInstance" />
    [ToString]
    internal sealed class ToolbarSeparatorInstance : ToolbarPluginInstance
    {
        #region Fields

        /// <summary>
        /// セパレータのサイズです。
        /// </summary>
        private static readonly Size SeparatorSize = new(1, Util.ImageList.ImageSize.Height);

        #endregion Fields

        #region Methods

        /// <summary>
        /// セパレータ用のアイコンを生成します。
        /// </summary>
        /// <returns>セパレータ用のアイコン</returns>
        private Image CreateSeparatorIcon()
        {
            var icon = new Bitmap(SeparatorSize.Width, SeparatorSize.Height);

            using var graphics = Graphics.FromImage(icon);
            using var brush = new SolidBrush(ClientControl.Instance.Theme.SeparatorLinesColor);

            var rectangle = new Rectangle
            {
                Size = icon.Size
            };

            graphics.FillRectangle(brush, rectangle);

            return icon;
        }

        /// <summary>
        /// アイコンを読み込みます。
        /// </summary>
        private protected sealed override void LoadIcon()
        {
            Icon = CreateSeparatorIcon();
        }

        /// <summary>
        /// ツールバー用プラグインのメニューが押下された時に発生します。
        /// </summary>
        public sealed override void Activate()
        {
            // Do nothing.
        }

        #endregion Methods
    }

    /// <summary>
    /// ツールバーのセパレータです。
    /// </summary>
    /// <seealso cref="ToolbarPlugin" />
    [ToString]
    internal sealed class ToolbarSeparator : ToolbarPlugin
    {
        #region Properties

        /// <summary>
        /// ID です。
        /// </summary>
        /// <value>ID</value>
        public sealed override Guid Id { get; }

        /// <summary>
        /// 詳細項目への表示方法です。
        /// </summary>
        /// <value>詳細項目への表示方法</value>
        public sealed override ToolbarPluginOverflowMode ToolbarPluginOverflowMode { get; } = ToolbarPluginOverflowMode.NeverInOverflow;

        #endregion Properties

        #region Methods

        /// <summary>
        /// ツールバーのセパレータ用インスタンスを生成します。
        /// </summary>
        /// <returns>ツールバーのセパレータ用インスタンス</returns>
        public sealed override ViewItemToolbarPluginInstance GenerateViewItemToolbarPluginInstance()
        {
            return new ToolbarSeparatorInstance();
        }

        #endregion Methods
    }
}
