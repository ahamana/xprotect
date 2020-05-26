using System;
using System.Drawing;
using VideoOS.Platform;
using VideoOS.Platform.Client;

namespace ScratchFilter.Client.Toolbar
{
    /// <summary>
    /// ツールバーのセパレータ用インスタンスです。
    /// </summary>
    /// <seealso cref="ToolbarPluginInstance" />
    [ToString]
    internal class ToolbarSeparatorInstance : ToolbarPluginInstance
    {
        #region Fields

        /// <summary>
        /// セパレータのサイズ（px）です。
        /// </summary>
        private static readonly Size SeparatorSize = new Size(1, 16);

        #endregion

        #region Methods

        /// <summary>
        /// セパレータ用のアイコンを生成します。
        /// </summary>
        /// <returns>セパレータ用のアイコン</returns>
        private Image CreateSeparatorIcon()
        {
            Image icon = new Bitmap(SeparatorSize.Width, SeparatorSize.Height);

            using (Graphics graphics = Graphics.FromImage(icon))
            using (Brush brush = new SolidBrush(ClientControl.Instance.Theme.SeparatorLinesColor))
            {
                graphics.FillRectangle(brush, new Rectangle(Point.Empty, icon.Size));
            }

            return icon;
        }

        /// <summary>
        /// アイコンを読み込みます。
        /// </summary>
        protected override void LoadIcon()
        {
            Icon = CreateSeparatorIcon();
        }

        /// <summary>
        /// ツールバー用プラグインのメニューが押下された時に呼び出されます。
        /// </summary>
        public override void Activate()
        {
            // Do nothing.
        }

        #endregion
    }

    /// <summary>
    /// ツールバーのセパレータです。
    /// </summary>
    /// <seealso cref="ToolbarPlugin" />
    [ToString]
    internal class ToolbarSeparator : ToolbarPlugin
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
        }

        #endregion

        #region Methods

        /// <summary>
        /// ツールバーのセパレータ用インスタンスを生成します。
        /// </summary>
        /// <returns>ツールバーのセパレータ用インスタンス</returns>
        public override ViewItemToolbarPluginInstance GenerateViewItemToolbarPluginInstance()
        {
            return new ToolbarSeparatorInstance();
        }

        #endregion
    }
}
