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
using System.Windows;

using ScratchFilter.Client.ViewModels.Toolbar;

namespace ScratchFilter.Client.Views.Toolbar;

/// <summary>
/// ScratchFilterSettingWindow.xaml の相互作用ロジック
/// </summary>
internal sealed partial class ScratchFilterSettingWindow : Window
{
    #region Constructors

    /// <summary>
    /// コンストラクタです。
    /// </summary>
    /// <param name="cameraId">カメラの ID</param>
    /// <exception cref="ArgumentException"><paramref name="cameraId" /> が <see cref="Guid.Empty" /> の場合にスローされます。</exception>
    internal ScratchFilterSettingWindow(Guid cameraId)
    {
        if (cameraId == Guid.Empty)
        {
            throw new ArgumentException(nameof(cameraId));
        }

        InitializeComponent();

        DataContext = new ScratchFilterSettingWindowViewModel(cameraId);
    }

    #endregion Constructors
}
