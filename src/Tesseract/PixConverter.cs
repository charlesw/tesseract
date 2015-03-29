using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Tesseract
{
    /// <summary>
    /// Handles converting between different image formats supported by DotNet.
    /// </summary>
    public static class PixConverter
    {
        private static readonly BitmapToPixConverter bitmapConverter = new BitmapToPixConverter();
        private static readonly PixToBitmapConverter pixConverter = new PixToBitmapConverter();

        /// <summary>
        /// Converts the specified <paramref name="pix"/> to a Bitmap.
        /// </summary>
        /// <param name="pix">The source image to be converted.</param>
        /// <returns>The converted pix as a <see cref="Bitmap"/>.</returns>
        public static Bitmap ToBitmap(Pix pix)
        {
            return pixConverter.Convert(pix);
        }

        /// <summary>
        /// Converts the specified <paramref name="img"/> to a Pix.
        /// </summary>
        /// <param name="img">The source image to be converted.</param>
        /// <returns>The converted bitmap image as a <see cref="Pix"/>.</returns>
        public static Pix ToPix(Bitmap img)
        {
            return bitmapConverter.Convert(img);
        }
    }
}