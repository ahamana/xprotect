//
// (C) 2020 Canon Inc.
//
// This file contains trade secrets of Canon Inc. No part may be reproduced
// or transmitted in any form by any means or for any purpose without the
// express written permission of Canon Inc.
//

using System;
using System.Windows;

using ScratchFilter.Client.ViewModels;

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
}
