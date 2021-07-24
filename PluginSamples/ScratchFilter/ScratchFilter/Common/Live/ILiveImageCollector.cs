//
// (C) 2020 Canon Inc.
//
// This file contains trade secrets of Canon Inc. No part may be reproduced
// or transmitted in any form by any means or for any purpose without the
// express written permission of Canon Inc.
//

using System;
using System.Drawing;

namespace ScratchFilter.Common.Live
{
    /// <summary>
    /// ライブ映像の画像を収集するためのインターフェースです。
    /// </summary>
    /// <seealso cref="IDisposable" />
    internal interface ILiveImageCollector : IDisposable
    {
        #region Methods

        /// <summary>
        /// 画像を取得します。
        /// </summary>
        /// <returns>画像</returns>
        Bitmap? GetImage();

        /// <summary>
        /// 収集する画像のサイズを設定します。
        /// </summary>
        /// <param name="imageSize">画像のサイズ</param>
        /// <remarks>
        /// 幅と高さに 0 を指定した場合は、実際の解像度の画像が収集されます。
        /// </remarks>
        void SetImageSize(Size imageSize);

        #endregion Methods
    }
}
