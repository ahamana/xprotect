//
// (C) 2020 Canon Inc.
//
// This file contains trade secrets of Canon Inc. No part may be reproduced
// or transmitted in any form by any means or for any purpose without the
// express written permission of Canon Inc.
//

using System.Drawing;

namespace ScratchFilter.Extensions;

/// <summary>
/// <see cref="Image" /> の拡張メソッドです。
/// </summary>
internal static class ImageExtensions
{
    #region Methods

    /// <summary>
    /// <see cref="Image" /> を <see cref="byte" /> 配列に変換します。
    /// </summary>
    /// <param name="image">変換する <see cref="Image" /></param>
    /// <returns>変換した <see cref="byte" /> 配列</returns>
    internal static byte[] ToByteArray(this Image image)
    {
        var converter = new ImageConverter();

        return (byte[])converter.ConvertTo(image, typeof(byte[]));
    }

    #endregion Methods
}
