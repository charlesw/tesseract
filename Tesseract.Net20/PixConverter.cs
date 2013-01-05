using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Tesseract
{
    public static class PixConverter
    {
        public static Pix ToPix(Bitmap img)
        {
            var converter = new BitmapToPixConverter();
            return converter.Convert(img);
        }
    }
}
