using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tesseract.Tests.Leptonica
{
    public class ConvertBitmapToPixTests
    {
        const string DataDirectory = @"Data\Conversion\";

        [Test]
        public unsafe void Convert_BitmapToPix(
            [Values("photo.jpg", "photo.bmp", "photo_8.bmp", "photo_24.bmp", "photo.png", "photo_8.png", "photo_24.png", "photo_32.png", "photo.tif", "photo.gif")]
            string sourceFile)
        {
            var sourceFilePath = Path.Combine(DataDirectory, sourceFile);
            var bitmapConverter = new BitmapToPixConverter();
            using (var source = new Bitmap(sourceFilePath)) {
                using (var dest = bitmapConverter.Convert(source)) {
                    AssertAreEquivalent(source, dest, true);

                    //dest.Save("converted_img.bmp");
                }
            }
        }

        [Test]
        public unsafe void Convert_PixToBitmap(
            [Values("photo.jpg", "photo.bmp", "photo_8.bmp", "photo_24.bmp", "photo.png", "photo_8.png", "photo_24.png", "photo_32.png", "photo.tif", "photo.gif")]
            string sourceFile,
            [Values(true, false)]
            bool includeAlpha)
        {
            var sourceFilePath = Path.Combine(DataDirectory, sourceFile);
            var converter = new PixToBitmapConverter();
            using (var source = Pix.LoadFromFile(sourceFilePath)) {
                using (var dest = converter.Convert(source, includeAlpha)) {
                    AssertAreEquivalent(dest, source, includeAlpha);

                   // dest.Save("converted_pix.bmp");
                }
            }
        }


        private void AssertAreEquivalent(Bitmap bmp, Pix pix, bool checkAlpha)
        {
            // verify img metadata
            Assert.That(pix.Width, Is.EqualTo(bmp.Width));
            Assert.That(pix.Height, Is.EqualTo(bmp.Height));
            //Assert.That(pix.Resolution.X, Is.EqualTo(bmp.HorizontalResolution));
            //Assert.That(pix.Resolution.Y, Is.EqualTo(bmp.VerticalResolution));

            // do some random sampling over image
            var height = pix.Height;
            var width = pix.Width;
            for (int y = 0; y < height; y += height / 4) {
                for (int x = 0; x < width; x += width / 4) {
                    Color sourcePixel = (Color)bmp.GetPixel(x, y);
                    Color destPixel = GetPixel(pix, x, y);
                    if (checkAlpha) {
                        Assert.That(destPixel, Is.EqualTo(sourcePixel), "Expected pixel at <{0},{1}> to be same in both source and dest.", x, y);
                    } else {
                        Assert.That(destPixel, Is.EqualTo(sourcePixel).Using<Color>((c1, c2) => (c1.Red == c2.Red && c1.Blue == c2.Blue && c1.Green == c2.Green) ? 0 : 1), "Expected pixel at <{0},{1}> to be same in both source and dest.", x, y);
                    }
                }
            }
        }
        
        private unsafe Color GetPixel(Pix pix, int x, int y)
        {
            var pixData = pix.GetData();

            if (pix.Colormap != null) {
                var pixLine = (uint*)pixData.Data + pixData.WordsPerLine * y;
                var pixValue = (int)PixData.GetDataByte(pixLine, x);
                return pix.Colormap[pixValue];
            } else {
                var pixLine = (uint*)pixData.Data + pixData.WordsPerLine * y;
                return Color.FromRgba(pixLine[x]);
            }
        }
    }
}
