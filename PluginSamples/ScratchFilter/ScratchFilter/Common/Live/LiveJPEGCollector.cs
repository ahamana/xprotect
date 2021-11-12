//
// (C) 2020 Canon Inc.
//
// This file contains trade secrets of Canon Inc. No part may be reproduced
// or transmitted in any form by any means or for any purpose without the
// express written permission of Canon Inc.
//

using System;
using System.Drawing;
using System.IO;

using VideoOS.Platform;
using VideoOS.Platform.Live;

namespace ScratchFilter.Common.Live;

/// <summary>
/// ライブ映像の JPEG 画像を収集するためのクラスです。
/// </summary>
/// <seealso cref="LiveImageCollector{JPEGLiveSource, LiveSourceContent}" />
[ToString]
internal sealed class LiveJPEGCollector : LiveImageCollector<JPEGLiveSource, LiveSourceContent>
{
    #region Constructors

    /// <summary>
    /// コンストラクタです。
    /// </summary>
    /// <param name="cameraFQID">カメラの完全修飾 ID</param>
    internal LiveJPEGCollector(FQID cameraFQID) : base(cameraFQID) { }

    /// <summary>
    /// コンストラクタです。
    /// </summary>
    /// <param name="cameraFQID">カメラの完全修飾 ID</param>
    /// <param name="imageSize">画像のサイズ</param>
    /// <remarks>
    /// 画像のサイズの幅と高さに 0 を指定した場合は、実際の解像度の画像が収集されます。
    /// </remarks>
    internal LiveJPEGCollector(FQID cameraFQID, Size imageSize) : base(cameraFQID, imageSize) { }

    /// <summary>
    /// コンストラクタです。
    /// </summary>
    /// <param name="cameraId">カメラの ID</param>
    internal LiveJPEGCollector(Guid cameraId) : base(cameraId) { }

    /// <summary>
    /// コンストラクタです。
    /// </summary>
    /// <param name="cameraId">カメラの ID</param>
    /// <param name="imageSize">画像のサイズ</param>
    /// <remarks>
    /// 画像のサイズの幅と高さに 0 を指定した場合は、実際の解像度の画像が収集されます。
    /// </remarks>
    internal LiveJPEGCollector(Guid cameraId, Size imageSize) : base(cameraId, imageSize) { }

    #endregion Constructors

    #region Methods

    /// <inheritdoc />
    private protected sealed override JPEGLiveSource GenerateVideoLiveSource(Item camera) =>
        new(camera)
        {
            // 無圧縮の画像を取得
            Compression = 100
        };

    /// <inheritdoc />
    private protected sealed override Stream GenerateImageStream(LiveSourceContent liveContent) =>
        new MemoryStream(liveContent.Content);

    #endregion Methods
}
