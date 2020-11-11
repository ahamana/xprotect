using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace ScratchFilter.Extensions
{
    /// <summary>
    /// <see cref="Bitmap" /> の拡張メソッドです。
    /// </summary>
    internal static class BitmapExtensions
    {
        #region Methods

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeleteObject(IntPtr hObject);

        /// <summary>
        /// <see cref="Bitmap" /> を <see cref="BitmapSource" /> に変換します。
        /// </summary>
        /// <param name="bitmap">変換する <see cref="Bitmap" /></param>
        /// <returns>変換した <see cref="BitmapSource" /></returns>
        /// <exception cref="ArgumentNullException"><paramref name="bitmap" /> が <c>null</c> の場合にスローされます。</exception>
        internal static BitmapSource ToBitmapSource(this Bitmap bitmap)
        {
            if (bitmap is null)
            {
                throw new ArgumentNullException(nameof(bitmap));
            }

            IntPtr hBitmap = bitmap.GetHbitmap();

            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty,
                                                             BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                DeleteObject(hBitmap);
            }
        }

        #endregion Methods
    }
}
