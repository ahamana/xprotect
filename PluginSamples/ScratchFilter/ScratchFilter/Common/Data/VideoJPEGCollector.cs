﻿/*
 * MIT License
 *
 * (c) 2020 Akihiro Hamana
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System;
using System.Drawing;

using VideoOS.Platform;
using VideoOS.Platform.Data;

namespace ScratchFilter.Common.Data;

/// <summary>
/// 録画映像の JPEG 画像を収集するためのクラスです。
/// </summary>
/// <seealso cref="VideoImageCollector{JPEGVideoSource, JPEGData}" />
[ToString]
internal sealed class VideoJPEGCollector : VideoImageCollector<JPEGVideoSource, JPEGData>
{
    #region Constructors

    /// <summary>
    /// コンストラクタです。
    /// </summary>
    /// <param name="cameraFQID">カメラの完全修飾 ID</param>
    internal VideoJPEGCollector(FQID cameraFQID) : base(cameraFQID) { }

    /// <summary>
    /// コンストラクタです。
    /// </summary>
    /// <param name="cameraFQID">カメラの完全修飾 ID</param>
    /// <param name="imageSize">画像のサイズ</param>
    /// <remarks>
    /// 画像のサイズの幅と高さに 0 を指定した場合は、実際の解像度の画像が収集されます。
    /// </remarks>
    internal VideoJPEGCollector(FQID cameraFQID, Size imageSize) : base(cameraFQID, imageSize) { }

    /// <summary>
    /// コンストラクタです。
    /// </summary>
    /// <param name="cameraId">カメラの ID</param>
    internal VideoJPEGCollector(Guid cameraId) : base(cameraId) { }

    /// <summary>
    /// コンストラクタです。
    /// </summary>
    /// <param name="cameraId">カメラの ID</param>
    /// <param name="imageSize">画像のサイズ</param>
    /// <remarks>
    /// 画像のサイズの幅と高さに 0 を指定した場合は、実際の解像度の画像が収集されます。
    /// </remarks>
    internal VideoJPEGCollector(Guid cameraId, Size imageSize) : base(cameraId, imageSize) { }

    #endregion Constructors

    #region Methods

    /// <inheritdoc />
    private protected sealed override JPEGVideoSource GenerateVideoSource(Item camera) =>
        new(camera)
        {
            // 無圧縮の画像を取得
            Compression = 100
        };

    /// <inheritdoc />
    public sealed override JPEGData? GetFirstImage() =>
        videoSource.GetBegin();

    /// <inheritdoc />
    public sealed override JPEGData? GetLastImage() =>
        videoSource.GetEnd();

    #endregion Methods
}
