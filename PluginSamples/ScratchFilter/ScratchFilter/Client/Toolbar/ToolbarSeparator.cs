using System;
using System.Drawing;
using System.Drawing.Imaging;
using VideoOS.Platform;
using VideoOS.Platform.Client;

namespace ScratchFilter.Client.Toolbar
{
    /// <summary>
    /// ツールバーのセパレータ用インスタンスです。
    /// </summary>
    /// <seealso cref="ToolbarPluginInstance" />
    internal class ToolbarSeparatorInstance : ToolbarPluginInstance
    {
        #region Fields

        /// <summary>
        /// Smart Client のテーマが「暗」の場合のセパレータの色です。
        /// </summary>
        private static readonly Color SeparatorDarkColor = Color.FromArgb(84, 92, 95);

        /// <summary>
        /// Smart Client のテーマが「明」の場合のセパレータの色です。
        /// </summary>
        private static readonly Color SeparatorLightColor = Color.FromArgb(192, 192, 192);

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
            Brush brush = ClientControl.Instance.Theme.ThemeType switch
            {
                ThemeType.Dark => new SolidBrush(SeparatorDarkColor),
                ThemeType.Light => new SolidBrush(SeparatorLightColor),
                _ => throw new MIPException()
            };

            Image icon = new Bitmap(SeparatorSize.Width, SeparatorSize.Height, PixelFormat.Format32bppArgb);

            using (Graphics graphics = Graphics.FromImage(icon))
            {
                graphics.FillRectangle(brush, 0, 0, icon.Width, icon.Height);
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
    /// <seealso cref="ToolbarPlugin{S}" />
    internal class ToolbarSeparator : ToolbarPlugin<ToolbarSeparator>
    {
        #region Properties

        /// <summary>
        /// ID です。
        /// </summary>
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
