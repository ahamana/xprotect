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
