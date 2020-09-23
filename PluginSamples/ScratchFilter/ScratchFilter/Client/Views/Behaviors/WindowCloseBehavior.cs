﻿using Microsoft.Xaml.Behaviors;
using System;
using System.Windows;

namespace ScratchFilter.Client.Views.Behaviors
{
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

        /// <summary>
        /// ビヘイビアが <see cref="Behavior{Window}.AssociatedObject" /> にアタッチされた後で呼び出されます。
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Closed += OnWindowClosed;
        }

        /// <summary>
        /// ビヘイビアが <see cref="Behavior{Window}.AssociatedObject" /> からデタッチされる時、その前に呼び出されます。
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.Closed -= OnWindowClosed;
        }

        #endregion Methods
    }
}