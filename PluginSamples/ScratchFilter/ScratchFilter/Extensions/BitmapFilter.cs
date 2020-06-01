using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Drawing;

namespace ScratchFilter.Extensions
{
    /// <summary>
    /// <see cref="Bitmap" /> の調整を行うための拡張メソッドです。
    /// </summary>
    internal static class BitmapFilter
    {
        #region Methods

        /// <summary>
        /// 画像のコントラストを調整します。
        /// </summary>
        /// <param name="mat">画像</param>
        /// <param name="contrast">コントラストの調整に使用する値</param>
        private static void AdjustContrast(Mat mat, float contrast)
        {
            using (Mat dst = mat * contrast)
            {
                dst.CopyTo(mat);
            }
        }

        /// <summary>
        /// 画像の明るさを調整します。
        /// </summary>
        /// <param name="mat">画像</param>
        /// <param name="brightness">明るさの調整に使用する値</param>
        private static void AdjustBrightness(Mat mat, float brightness)
        {
            using (Mat dst = mat + brightness)
            {
                dst.CopyTo(mat);
            }
        }

        /// <summary>
        /// 画像の彩度を調整します。
        /// </summary>
        /// <param name="mat">画像</param>
        /// <param name="brightness">彩度の調整に使用する値</param>
        private static void AdjustSaturation(Mat mat, float saturation)
        {
            Cv2.CvtColor(mat, mat, ColorConversionCodes.BGR2HSV);

            Cv2.Split(mat, out Mat[] channels);

            channels[2] = channels[2] * saturation;

            Cv2.Merge(channels, mat);

            Cv2.CvtColor(mat, mat, ColorConversionCodes.HSV2BGR);
        }

        /// <summary>
        /// 画像のガンマを調整します。
        /// </summary>
        /// <param name="mat">画像</param>
        /// <param name="brightness">ガンマの調整に使用する値</param>
        private static void AdjustGamma(Mat mat, float gamma)
        {
            byte[] lut = new byte[256];

            for (int i = 0; i < lut.Length; i++)
            {
                lut[i] = (byte)(Math.Pow(i / 255.0, 1.0 / gamma) * 255.0);
            }

            Cv2.LUT(mat, lut, mat);
        }

        /// <summary>
        /// 画像を調整します。
        /// </summary>
        /// <param name="mat">画像</param>
        /// <param name="contrast">明るさの調整に使用する値</param>
        /// <param name="brightness">明るさの調整に使用する値</param>
        /// <param name="saturation">彩度の調整に使用する値</param>
        /// <param name="gamma">ガンマの調整に使用する値</param>
        private static void Adjust(Mat mat, float contrast, float brightness, float saturation, float gamma)
        {
            AdjustContrast(mat, contrast);
            AdjustBrightness(mat, brightness);
            AdjustSaturation(mat, saturation);
            AdjustGamma(mat, gamma);
        }

        /// <summary>
        /// 画像を調整します。
        /// </summary>
        /// <param name="src">調整する画像</param>
        /// <param name="contrast">明るさの調整に使用する値</param>
        /// <param name="brightness">明るさの調整に使用する値</param>
        /// <param name="saturation">彩度の調整に使用する値</param>
        /// <param name="gamma">ガンマの調整に使用する値</param>
        /// <returns>調整した画像</returns>
        internal static Bitmap Adjust(this Bitmap src, float contrast, float brightness, float saturation, float gamma)
        {
            if (src == null)
            {
                throw new ArgumentNullException(nameof(src));
            }

            using (Mat mat = src.ToMat())
            {
                Adjust(mat, contrast, brightness, saturation, gamma);

                return mat.ToBitmap();
            }
        }

        /// <summary>
        /// 画像を調整します。
        /// </summary>
        /// <param name="src">調整する画像</param>
        /// <param name="dst">調整結果を反映する画像</param>
        /// <param name="contrast">明るさの調整に使用する値</param>
        /// <param name="brightness">明るさの調整に使用する値</param>
        /// <param name="saturation">彩度の調整に使用する値</param>
        /// <param name="gamma">ガンマの調整に使用する値</param>
        internal static void Adjust(this Bitmap src, Bitmap dst, float contrast, float brightness, float saturation, float gamma)
        {
            if (src == null)
            {
                throw new ArgumentNullException(nameof(src));
            }

            if (dst == null)
            {
                throw new ArgumentNullException(nameof(dst));
            }

            using (Mat mat = src.ToMat())
            {
                Adjust(mat, contrast, brightness, saturation, gamma);

                mat.ToBitmap(dst);
            }
        }

        #endregion
    }
}
