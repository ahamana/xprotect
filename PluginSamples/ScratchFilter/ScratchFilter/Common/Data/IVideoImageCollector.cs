//
// (C) 2020 Canon Inc.
//
// This file contains trade secrets of Canon Inc. No part may be reproduced
// or transmitted in any form by any means or for any purpose without the
// express written permission of Canon Inc.
//

using System;
using System.Collections.Generic;

namespace ScratchFilter.Common.Data;

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
    /// 指定された日時、もしくは指定された日時以降の最も近い日時の画像を取得します。
    /// </summary>
    /// <param name="dateTime">日時</param>
    /// <returns>画像データ</returns>
    TImageData? GetImageAtOrAfter(DateTime dateTime);

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
    /// 最も日時の古い画像を取得します。
    /// </summary>
    /// <returns>画像データ</returns>
    TImageData? GetFirstImage();

    /// <summary>
    /// 最も日時の新しい画像を取得します。
    /// </summary>
    /// <returns>画像データ</returns>
    TImageData? GetLastImage();

    /// <summary>
    /// 指定された日時以降の画像を取得します。
    /// </summary>
    /// <param name="dateTime">日時</param>
    /// <param name="maxCount">画像の最大取得数</param>
    /// <returns>画像データの一覧</returns>
    IEnumerable<TImageData> GetImages(DateTime dateTime, int maxCount = int.MaxValue);

    /// <summary>
    /// 指定された日時以降の指定された時間幅に存在する画像を取得します。
    /// </summary>
    /// <param name="dateTime">日時</param>
    /// <param name="timeSpan">時間幅</param>
    /// <param name="maxCount">画像の最大取得数</param>
    /// <returns>画像データの一覧</returns>
    IEnumerable<TImageData> GetImages(DateTime dateTime, TimeSpan timeSpan, int maxCount = int.MaxValue);

    #endregion Methods
}
