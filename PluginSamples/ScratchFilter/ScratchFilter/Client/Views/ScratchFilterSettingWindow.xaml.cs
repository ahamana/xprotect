//
// (C) 2020 Canon Inc.
//
// This file contains trade secrets of Canon Inc. No part may be reproduced
// or transmitted in any form by any means or for any purpose without the
// express written permission of Canon Inc.
//

using System.Windows;

using ScratchFilter.Client.ViewModels;

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
        internal ScratchFilterSettingWindow(ImageViewerAddOn imageViewerAddOn)
        {
            InitializeComponent();

            DataContext = new ScratchFilterSettingWindowViewModel(imageViewerAddOn);
        }

        #endregion Constructors
    }
}
