using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Tesseract
{
    public static class PixConverter
    {
        private static readonly BitmapToPixConverter bitmapConverter = new BitmapToPixConverter();
        private static readonly PixToBitmapConverter pixConverter = new PixToBitmapConverter();

        public static Pix ToPix(Bitmap img)
        {
            return bitmapConverter.Convert(img);
        }

        public static Bitmap ToBitmap(Pix pix)
        {
            return pixConverter.Convert(pix);
        }
    }
}
