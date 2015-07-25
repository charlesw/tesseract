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
        const string ResultsDirectory = @"Results\Conversion\";

        // Test for [Issue #166](https://github.com/charlesw/tesseract/issues/166)
        [Test]
        public unsafe void Convert_ScaledBitmapToPix()
        {
            if (!Directory.Exists(ResultsDirectory)) Directory.CreateDirectory(ResultsDirectory);

            var sourceFile = "photo_rgb_32bpp.tif";
            var sourceFilePath = Path.Combine(DataDirectory, sourceFile);
            var bitmapConverter = new BitmapToPixConverter();
            using (var source = new Bitmap(sourceFilePath)) {
                using (var scaledSource = new Bitmap(source, new Size(source.Width * 2, source.Height * 2))) {
                    Assert.That(BitmapHelper.GetBPP(scaledSource), Is.EqualTo(32));
                    using (var dest = bitmapConverter.Convert(scaledSource)) {
                        var destFilename = "ScaledBitmapToPix_rgb_32bpp.tif";
                        dest.Save(Path.Combine(ResultsDirectory, destFilename), ImageFormat.Tiff);

                        AssertAreEquivalent(scaledSource, dest, true);
                    }
                }
            }
        }

        [Test]
        [TestCase(1)] // Note: 1bpp will not save pixmap when writing out the result, this is a limitation of leptonica (see pixWriteToTiffStream)
        [TestCase(4, Ignore = true, Reason = "4bpp images not supported.")]
        [TestCase(8)]
        [TestCase(32)]
        public unsafe void Convert_BitmapToPix(int depth)
        {
            if (!Directory.Exists(ResultsDirectory)) Directory.CreateDirectory(ResultsDirectory);

            string pixType;
            if (depth < 16) pixType = "palette";
            else if (depth == 16) pixType = "grayscale";
            else pixType = "rgb";

            var sourceFile = String.Format("photo_{0}_{1}bpp.tif", pixType, depth);
            var sourceFilePath = Path.Combine(DataDirectory, sourceFile);
            var bitmapConverter = new BitmapToPixConverter();
            using (var source = new Bitmap(sourceFilePath)) {
                Assert.That(BitmapHelper.GetBPP(source), Is.EqualTo(depth));
                using (var dest = bitmapConverter.Convert(source)) {
                    var destFilename = String.Format("BitmapToPix_{0}_{1}bpp.tif", pixType, depth);
                    dest.Save(Path.Combine(ResultsDirectory, destFilename), ImageFormat.Tiff);

                    AssertAreEquivalent(source, dest, true);
                }
            }
        }

        /// <summary>
        /// Test case for https://github.com/charlesw/tesseract/issues/180
        /// </summary>
        [Test]
        public unsafe void Convert_BitmapToPix_Format8bppIndexed()
        {
            if (!Directory.Exists(ResultsDirectory)) Directory.CreateDirectory(ResultsDirectory);

            var sourceFile = "photo_palette_8bpp.png";
            var sourceFilePath = Path.Combine(DataDirectory, sourceFile);
            var bitmapConverter = new BitmapToPixConverter();
            using (var source = new Bitmap(sourceFilePath)) {
                Assert.That(BitmapHelper.GetBPP(source), Is.EqualTo(8));
                Assert.That(source.PixelFormat, Is.EqualTo(PixelFormat.Format8bppIndexed));
                using (var dest = bitmapConverter.Convert(source)) {
                    var destFilename = "BitmapToPix_palette_8bpp.png";
                    dest.Save(Path.Combine(ResultsDirectory, destFilename), ImageFormat.Png);

                    AssertAreEquivalent(source, dest, true);
                }
            }
        }

        [Test]
        [TestCase(1, true, false)]
        [TestCase(1, false, false)]
        [TestCase(4, false, false, Ignore = true, Reason = "4bpp images not supported.")]
        [TestCase(4, true, false, Ignore = true, Reason = "4bpp images not supported.")]
        [TestCase(8, false, false)]
        [TestCase(8, true, false, Ignore = true, Reason = "Haven't yet created a 8bpp grayscale test image.")]
        [TestCase(32, false, true)]
        [TestCase(32, false, false)]
        public unsafe void Convert_PixToBitmap(int depth, bool isGrayscale, bool includeAlpha)
        {
            if (!Directory.Exists(ResultsDirectory)) Directory.CreateDirectory(ResultsDirectory);

            bool hasPalette = depth < 16 && !isGrayscale;
            string pixType;
            if (isGrayscale) pixType = "grayscale";
            else if (hasPalette) pixType = "palette";
            else pixType = "rgb";

            var sourceFile = String.Format("photo_{0}_{1}bpp.tif", pixType, depth);
            var sourceFilePath = Path.Combine(DataDirectory, sourceFile);
            var converter = new PixToBitmapConverter();
            using (var source = Pix.LoadFromFile(sourceFilePath)) {
                Assert.That(source.Depth, Is.EqualTo(depth));
                if (hasPalette) {
                    Assert.That(source.Colormap, Is.Not.Null, "Expected source image to have color map\\palette.");
                } else {
                    Assert.That(source.Colormap, Is.Null, "Expected source image to be grayscale.");
                }
                using (var dest = converter.Convert(source, includeAlpha)) {
                    var destFilename = String.Format("PixToBitmap_{0}_{1}bpp.tif", pixType, depth);
                    dest.Save(Path.Combine(ResultsDirectory, destFilename), System.Drawing.Imaging.ImageFormat.Tiff);

                    AssertAreEquivalent(dest, source, includeAlpha);
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
            for (int y = 0; y < height; y += height) {
                for (int x = 0; x < width; x += width) {
                    PixColor sourcePixel = (PixColor)bmp.GetPixel(x, y);
                    PixColor destPixel = GetPixel(pix, x, y);
                    if (checkAlpha) {
                        Assert.That(destPixel, Is.EqualTo(sourcePixel), "Expected pixel at <{0},{1}> to be same in both source and dest.", x, y);
                    } else {
                        Assert.That(destPixel, Is.EqualTo(sourcePixel).Using<PixColor>((c1, c2) => (c1.Red == c2.Red && c1.Blue == c2.Blue && c1.Green == c2.Green) ? 0 : 1), "Expected pixel at <{0},{1}> to be same in both source and dest.", x, y);
                    }
                }
            }
        }
        
        private unsafe PixColor GetPixel(Pix pix, int x, int y)
        {
            var pixDepth = pix.Depth;
            var pixData = pix.GetData();
            var pixLine = (uint*)pixData.Data + pixData.WordsPerLine * y;
            uint pixValue;
            if (pixDepth == 1) {
                pixValue = PixData.GetDataBit(pixLine, x);
            } else if (pixDepth == 4) {
                pixValue = PixData.GetDataQBit(pixLine, x);
            } else if (pixDepth == 8) {
                pixValue = PixData.GetDataByte(pixLine, x);
            } else if (pixDepth == 32) {
                pixValue = PixData.GetDataFourByte(pixLine, x);
            } else {
                throw new ArgumentException(String.Format("Bit depth of {0} is not supported.", pix.Depth), "pix");
            }

            if (pix.Colormap != null) {
                return pix.Colormap[(int)pixValue];
            } else {
                if (pixDepth == 32) {
                    return PixColor.FromRgba(pixValue);
                } else {
                    byte grayscale = (byte)(pixValue * 255 / ((1 << 16) - 1));
                    return new PixColor(grayscale, grayscale, grayscale);
                }
            }
        }
    }
}
