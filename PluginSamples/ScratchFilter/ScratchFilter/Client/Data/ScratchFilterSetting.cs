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

using Prism.Mvvm;

using VideoOS.Platform;

namespace ScratchFilter.Client.Data;

/// <summary>
/// 傷フィルタの設定です。
/// </summary>
/// <seealso cref="BindableBase" />
[ToString]
internal sealed class ScratchFilterSetting : BindableBase
{
    #region Fields

    /// <summary>
    /// デフォルトの画像のコントラストです。
    /// </summary>
    private static readonly int DefaultImageContrast = 0;

    /// <summary>
    /// デフォルトの画像の明るさです。
    /// </summary>
    private static readonly int DefaultImageBrightness = 0;

    /// <summary>
    /// デフォルトの画像の彩度です。
    /// </summary>
    private static readonly int DefaultImageSaturation = 0;

    /// <summary>
    /// デフォルトの画像のガンマです。
    /// </summary>
    private static readonly float DefaultImageGamma = 1;

    /// <summary>
    /// 画像のコントラストです。
    /// </summary>
    private int imageContrast = DefaultImageContrast;

    /// <summary>
    /// 画像の明るさです。
    /// </summary>
    private int imageBrightness = DefaultImageBrightness;

    /// <summary>
    /// 画像の彩度です。
    /// </summary>
    private int imageSaturation = DefaultImageSaturation;

    /// <summary>
    /// 画像のガンマです。
    /// </summary>
    private float imageGamma = DefaultImageGamma;

    #endregion Fields

    #region Properties

    /// <summary>
    /// カメラの ID です。
    /// </summary>
    /// <value>カメラの ID</value>
    public Guid CameraId { get; init; }

    /// <summary>
    /// カメラ名です。
    /// </summary>
    /// <value>カメラ名</value>
    public string? CameraName
    {
        get
        {
            var camera = Configuration.Instance.GetItem(CameraId, Kind.Camera);

            return camera?.Name;
        }
    }

    /// <summary>
    /// 画像のコントラストです。
    /// </summary>
    /// <value>画像のコントラスト</value>
    public int ImageContrast
    {
        get => imageContrast;
        set => SetProperty(ref imageContrast, value);
    }

    /// <summary>
    /// 画像の明るさです。
    /// </summary>
    /// <value>画像の明るさ</value>
    public int ImageBrightness
    {
        get => imageBrightness;
        set => SetProperty(ref imageBrightness, value);
    }

    /// <summary>
    /// 画像の彩度です。
    /// </summary>
    /// <value>画像の彩度</value>
    public int ImageSaturation
    {
        get => imageSaturation;
        set => SetProperty(ref imageSaturation, value);
    }

    /// <summary>
    /// 画像のガンマです。
    /// </summary>
    /// <value>画像のガンマ</value>
    public float ImageGamma
    {
        get => imageGamma;
        set => SetProperty(ref imageGamma, value);
    }

    #endregion Properties
}
