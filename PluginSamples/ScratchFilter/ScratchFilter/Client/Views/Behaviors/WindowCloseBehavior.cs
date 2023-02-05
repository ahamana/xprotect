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
using System.Windows;

using Microsoft.Xaml.Behaviors;

namespace ScratchFilter.Client.Views.Behaviors;

/// <summary>
/// ウィンドウを閉じる時のビヘイビアです。
/// </summary>
/// <seealso cref="Behavior{Window}" />
[ToString]
internal sealed class WindowCloseBehavior : Behavior<Window>
{
    #region Methods

    /// <summary>
    /// ウィンドウが閉じる時に発生します。
    /// </summary>
    /// <param name="sender">イベントハンドラがアタッチされるオブジェクト</param>
    /// <param name="e">イベントのデータ</param>
    private void OnWindowClosed(object sender, EventArgs e)
    {
        (AssociatedObject.DataContext as IDisposable)?.Dispose();
    }

    /// <inheritdoc />
    protected sealed override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.Closed += OnWindowClosed;
    }

    /// <inheritdoc />
    protected sealed override void OnDetaching()
    {
        base.OnDetaching();
        AssociatedObject.Closed -= OnWindowClosed;
    }

    #endregion Methods
}
