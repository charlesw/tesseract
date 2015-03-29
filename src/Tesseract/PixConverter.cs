using System.Drawing;

namespace Tesseract
{
    public static class PixConverter
    {
        private static readonly BitmapToPixConverter bitmapConverter = new BitmapToPixConverter();
        private static readonly PixToBitmapConverter pixConverter = new PixToBitmapConverter();

        public static Bitmap ToBitmap(Pix pix)
        {
            return pixConverter.Convert(pix);
        }

        public static Pix ToPix(Bitmap img)
        {
            return bitmapConverter.Convert(img);
        }
    }
}