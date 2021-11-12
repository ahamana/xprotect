//
// (C) 2020 Canon Inc.
//
// This file contains trade secrets of Canon Inc. No part may be reproduced
// or transmitted in any form by any means or for any purpose without the
// express written permission of Canon Inc.
//

using System.Drawing;
using System.IO;

using VideoOS.Platform.Data;

namespace ScratchFilter.Extensions;

/// <summary>
/// <see cref="JPEGData" /> の拡張メソッドです。
/// </summary>
internal static class JPEGDataExtensions
{
    #region Methods

    /// <summary>
    /// 画像を取得します。
    /// </summary>
    /// <param name="jpegData"><see cref="JPEGData" /></param>
    /// <returns>画像</returns>
    internal static Bitmap GetBitmap(this JPEGData jpegData)
    {
        using var stream = new MemoryStream(jpegData.Bytes);

        return new(stream);
    }

    #endregion Methods
}
