/*
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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using VideoOS.Platform;
using VideoOS.Platform.Data;

namespace ScratchFilter.Common.Data;

/// <summary>
/// 録画映像の画像を収集するための抽象クラスです。
/// </summary>
/// <typeparam name="TVideoSource"><see cref="VideoSource" /> のサブタイプ</typeparam>
/// <typeparam name="TImageData">画像データの型</typeparam>
/// <seealso cref="IVideoImageCollector{TImageData}" />
[ToString]
internal abstract class VideoImageCollector<TVideoSource, TImageData> : IVideoImageCollector<TImageData>
    where TVideoSource : VideoSource
    where TImageData : class
{
    #region Fields

    /// <summary>
    /// アンマネージリソースが解放されたかどうかです。
    /// </summary>
    private bool isDisposed;

    /// <summary>
    /// 録画映像のソースです。
    /// </summary>
    private protected readonly TVideoSource videoSource;

    #endregion Fields

    #region Constructors

    /// <summary>
    /// コンストラクタです。
    /// </summary>
    /// <param name="cameraFQID">カメラの完全修飾 ID</param>
    private protected VideoImageCollector(FQID cameraFQID) : this(cameraFQID.ObjectId) { }

    /// <summary>
    /// コンストラクタです。
    /// </summary>
    /// <param name="cameraFQID">カメラの完全修飾 ID</param>
    /// <param name="imageSize">画像のサイズ</param>
    /// <remarks>
    /// 画像のサイズの幅と高さに 0 を指定した場合は、実際の解像度の画像が収集されます。
    /// </remarks>
    private protected VideoImageCollector(FQID cameraFQID, Size imageSize) : this(cameraFQID.ObjectId, imageSize) { }

    /// <summary>
    /// コンストラクタです。
    /// </summary>
    /// <param name="cameraId">カメラの ID</param>
    /// <exception cref="ArgumentException"><paramref name="cameraId" /> が <see cref="Guid.Empty" /> の場合にスローされます。</exception>
    private protected VideoImageCollector(Guid cameraId) : this(cameraId, Size.Empty) { }

    /// <summary>
    /// コンストラクタです。
    /// </summary>
    /// <param name="cameraId">カメラの ID</param>
    /// <param name="imageSize">画像のサイズ</param>
    /// <exception cref="ArgumentException"><paramref name="cameraId" /> が <see cref="Guid.Empty" /> の場合にスローされます。</exception>
    /// <remarks>
    /// 画像のサイズの幅と高さに 0 を指定した場合は、実際の解像度の画像が収集されます。
    /// </remarks>
    private protected VideoImageCollector(Guid cameraId, Size imageSize)
    {
        if (cameraId == Guid.Empty)
        {
            throw new ArgumentException(nameof(cameraId));
        }

        var camera = Configuration.Instance.GetItem(cameraId, Kind.Camera);

        videoSource = GenerateVideoSource(camera);

        // 幅と高さに 0 を指定した場合は、実際の解像度の画像が取得される。
        videoSource.Init(imageSize.Width, imageSize.Height);
    }

    #endregion Constructors

    #region Methods

    /// <summary>
    /// アンマネージリソースを解放し、必要に応じてマネージリソースも解放します。
    /// </summary>
    /// <param name="disposing">マネージリソースとアンマネージリソースの両方を解放する場合は <c>true</c>。アンマネージリソースだけを解放する場合は <c>false</c>。</param>
    private void Dispose(bool disposing)
    {
        if (isDisposed)
        {
            return;
        }

        if (disposing)
        {
            videoSource.Close();
        }

        isDisposed = true;
    }

    /// <summary>
    /// 録画映像のソースを生成します。
    /// </summary>
    /// <param name="camera">カメラ</param>
    /// <returns>録画映像のソース</returns>
    private protected abstract TVideoSource GenerateVideoSource(Item camera);

    /// <inheritdoc cref="IVideoImageCollector{TImageData}.GetFirstImage" />
    public abstract TImageData? GetFirstImage();

    /// <inheritdoc cref="IVideoImageCollector{TImageData}.GetLastImage" />
    public abstract TImageData? GetLastImage();

    /// <inheritdoc cref="IVideoImageCollector{TImageData}.GetImageAtOrAfter(DateTime)" />
    public TImageData? GetImageAtOrAfter(DateTime dateTime) =>
        (TImageData?)videoSource.Get(dateTime.ToUniversalTime());

    /// <inheritdoc cref="IVideoImageCollector{TImageData}.GetImageAtOrBefore(DateTime)" />
    public TImageData? GetImageAtOrBefore(DateTime dateTime) =>
        (TImageData?)videoSource.GetAtOrBefore(dateTime.ToUniversalTime());

    /// <inheritdoc cref="IVideoImageCollector{TImageData}.GetImageNearest(DateTime)" />
    public TImageData? GetImageNearest(DateTime dateTime) =>
        (TImageData?)videoSource.GetNearest(dateTime.ToUniversalTime());

    /// <inheritdoc cref="IVideoImageCollector{TImageData}.GetImages(DateTime, int)" />
    public IEnumerable<TImageData> GetImages(DateTime dateTime, int maxCount = int.MaxValue) =>
        GetImages(dateTime, TimeSpan.MaxValue, maxCount);

    /// <inheritdoc cref="IVideoImageCollector{TImageData}.GetImages(DateTime, TimeSpan, int)" />
    public IEnumerable<TImageData> GetImages(DateTime dateTime, TimeSpan timeSpan, int maxCount = int.MaxValue) =>
        videoSource.Get(dateTime, timeSpan, maxCount).Cast<TImageData>();

    /// <inheritdoc cref="IDisposable.Dispose" />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion Methods
}
