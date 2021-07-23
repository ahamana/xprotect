//
// (C) 2020 Canon Inc.
//
// This file contains trade secrets of Canon Inc. No part may be reproduced
// or transmitted in any form by any means or for any purpose without the
// express written permission of Canon Inc.
//

using System;
using System.Drawing;

namespace ScratchFilter.Common.Data
{
    /// <summary>
    /// 録画映像の画像を収集するためのインターフェースです。
    /// </summary>
    /// <typeparam name="TImageData">画像データの型</typeparam>
    /// <seealso cref="IDisposable" />
    internal interface IVideoImageCollector<TImageData> : IDisposable
        where TImageData : class
    {
        #region Methods

        /// <summary>
        /// 指定された日時の画像を取得します。
        /// </summary>
        /// <param name="dateTime">日時</param>
        /// <returns>画像データ</returns>
        TImageData? GetImage(DateTime dateTime);

        /// <summary>
        /// 指定された日時、もしくは指定された日時以前の最も近い日時の画像を取得します。
        /// </summary>
        /// <param name="dateTime">日時</param>
        /// <returns>画像データ</returns>
        TImageData? GetImageAtOrBefore(DateTime dateTime);

        /// <summary>
        /// 指定された日時に最も近い日時の画像を取得します。
        /// </summary>
        /// <param name="dateTime">日時</param>
        /// <returns>画像データ</returns>
        TImageData? GetImageNearest(DateTime dateTime);

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
