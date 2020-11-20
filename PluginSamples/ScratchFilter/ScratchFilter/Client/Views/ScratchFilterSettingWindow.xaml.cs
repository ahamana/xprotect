//
// (C) 2020 Canon Inc.
//
// This file contains trade secrets of Canon Inc. No part may be reproduced
// or transmitted in any form by any means or for any purpose without the
// express written permission of Canon Inc.
//

using ScratchFilter.Client.ViewModels;
using System;
using System.Windows;
using VideoOS.Platform.Client;

namespace ScratchFilter.Client.Views
{
    /// <summary>
    /// ScratchFilterSettingWindow.xaml の相互作用ロジック
    /// </summary>
    internal sealed partial class ScratchFilterSettingWindow : Window
    {
        #region Constructors

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        /// <param name="imageViewerAddOn">イメージビューワのアドオン</param>
        /// <exception cref="ArgumentNullException"><paramref name="imageViewerAddOn" /> が <c>null</c> の場合にスローされます。</exception>
        internal ScratchFilterSettingWindow(ImageViewerAddOn imageViewerAddOn)
        {
            if (imageViewerAddOn is null)
            {
                throw new ArgumentNullException(nameof(imageViewerAddOn));
            }

            InitializeComponent();

            DataContext = new ScratchFilterSettingWindowViewModel(imageViewerAddOn);
        }

        #endregion Constructors
    }
}
