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
        [TestCase("photo.jpg")]
        [TestCase("photo.bmp")]
        [TestCase("photo_8.bmp")]
        [TestCase("photo_24.bmp")]
        [TestCase("photo.png")]
        [TestCase("photo_8.png")]
        [TestCase("photo_24.png")]
        [TestCase("photo_32.png")]
        [TestCase("photo.tif")]
        [TestCase("photo.gif")]
        public unsafe void Convert_BitmapToPix(string sourceFile)
        {
            var sourceFilePath = Path.Combine(DataDirectory, sourceFile);
            var bitmapConverter = new BitmapToPixConverter();
            using (var source = new Bitmap(sourceFilePath)) {
                //Console.WriteLine("Img format: {0}", img.PixelFormat);
                using (var dest = bitmapConverter.Convert(source)) {
                    AssertAreEquivalent(source, dest);

                    //dest.Save("converted_img.bmp");
                }
            }
        }


        private void AssertAreEquivalent(Bitmap bmp, Pix pix)
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
                    Assert.That(destPixel, Is.EqualTo(sourcePixel), "Expected pixel at <{0},{1}> to be same in both source and dest.");
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
