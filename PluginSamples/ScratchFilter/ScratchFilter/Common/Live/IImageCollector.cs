using System;
using System.Drawing;

namespace ScratchFilter.Common.Live
{
    /// <summary>
    /// 画像を収集するためのインターフェースです。
    /// </summary>
    /// <seealso cref="IDisposable" />
    internal interface IImageCollector : IDisposable
    {
        #region Methods

        /// <summary>
        /// 画像を取得します。
        /// </summary>
        /// <returns>画像</returns>
        Bitmap GetImage();

        #endregion Methods
    }
}
