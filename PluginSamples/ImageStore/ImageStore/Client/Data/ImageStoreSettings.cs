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

namespace ImageStore.Client.Data;

/// <summary>
/// 設定です。
/// </summary>
public sealed class ImageStoreSettings
{
    #region Properties

    /// <summary>
    /// JPEG ファイルを出力するディレクトリです。
    /// </summary>
    /// <value>JPEG ファイルを出力するディレクトリ</value>
    public string OutputDir { get; set; } = string.Empty;

    /// <summary>
    /// 出力する JPEG ファイルの圧縮です。
    /// </summary>
    /// <value>出力する JPEG ファイルの圧縮</value>
    public ushort Compression { get; set; }

    #endregion Properties
}
