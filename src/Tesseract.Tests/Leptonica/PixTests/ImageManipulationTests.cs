using System.IO;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Tesseract.Tests.Leptonica.PixTests
{
    [TestFixture]
    public class ImageManipulationTests
    {
        const string ResultsDirectory = @"Results\ImageManipulation\";

        [Test]
        public void DescewTest()
        {
            using (var sourcePix = Pix.LoadFromFile(@".\Data\Scew\scewed-phototest.png"))
            {
                Scew scew;
                using (var descewedImage = sourcePix.Deskew(new ScewSweep(range: 45), Pix.DefaultBinarySearchReduction, Pix.DefaultBinaryThreshold, out scew))
                {
                    Assert.That(scew.Angle, Is.EqualTo(-9.953125F).Within(0.00001));
                    Assert.That(scew.Confidence, Is.EqualTo(3.782913F).Within(0.00001));

                    SaveResult(descewedImage, "descewedImage.png");
                }
            }
        }

        [Test]
        public void OtsuBinarizationTest()
        {
            using (var sourcePix = Pix.LoadFromFile(@".\Data\Binarization\neo-8bit.png"))
            {
                using (var binarizedImage = sourcePix.BinarizeOtsuAdaptiveThreshold(200, 200, 10, 10, 0.1F))
                {
                    Assert.That(binarizedImage, Is.Not.Null);
                    Assert.That(binarizedImage.Handle, Is.Not.EqualTo(IntPtr.Zero));
                    SaveResult(binarizedImage, "binarizedOtsuImage.png");
                }
            }
        }

        [Test]
        public void SauvolaBinarizationTest()
        {
            using (var sourcePix = Pix.LoadFromFile(@".\Data\Binarization\neo-8bit-grayscale.png"))
            {
                using (var grayscalePix = sourcePix.ConvertRGBToGray(1, 1, 1))
                {
                    using (var binarizedImage = grayscalePix.BinarizeSauvola(10, 0.35f, false))
                    {
                        Assert.That(binarizedImage, Is.Not.Null);
                        Assert.That(binarizedImage.Handle, Is.Not.EqualTo(IntPtr.Zero));
                        SaveResult(binarizedImage, "binarizedSauvolaImage.png");
                    }
                }
            }
        }

        [Test]
        public void SauvolaTiledBinarizationTest()
        {
            using (var sourcePix = Pix.LoadFromFile(@".\Data\Binarization\neo-8bit-grayscale.png"))
            {
                using (var grayscalePix = sourcePix.ConvertRGBToGray(1, 1, 1))
                {
                    using (var binarizedImage = grayscalePix.BinarizeSauvolaTiled(10, 0.35f, 2, 2))
                    {
                        Assert.That(binarizedImage, Is.Not.Null);
                        Assert.That(binarizedImage.Handle, Is.Not.EqualTo(IntPtr.Zero));
                        SaveResult(binarizedImage, "binarizedSauvolaTiledImage.png");
                    }
                }
            }
        }

        [Test]
        public void ConvertRGBToGrayTest()
        {
            using (var sourcePix = Pix.LoadFromFile(@".\Data\Conversion\photo_rgb_32bpp.tif"))
            using (var grayscaleImage = sourcePix.ConvertRGBToGray())
            {
                Assert.That(grayscaleImage.Depth, Is.EqualTo(8));
                SaveResult(grayscaleImage, "grayscaleImage.jpg");
            }
        }

        [Test]
        [TestCase(45)]
        [TestCase(80)]
        [TestCase(90)]
        [TestCase(180)]
        [TestCase(270)]
        public void Rotate_ShouldBeAbleToRotateImageByXDegrees(float angle)
        {
            const string FileNameFormat = "rotation_{0}degrees.jpg";
            float angleAsRadians = (float)(angle * Math.PI / 180.0f);
            using (var sourcePix = Pix.LoadFromFile(@".\Data\Conversion\photo_rgb_32bpp.tif"))
            {
                using (var result = sourcePix.Rotate(angleAsRadians, RotationMethod.AreaMap))
                {
                    // TODO: Visualy confirm successful rotation and then setup an assertion to compare that result is the same.
                    var filename = String.Format(FileNameFormat, angle);
                    SaveResult(result, filename);
                }
            }
        }

        [Test]
        public void RemoveLinesTest()
        {
            using (var sourcePix = Pix.LoadFromFile(@".\Data\processing\table.png"))
            {
                // remove horizontal lines
                using (var result = sourcePix.RemoveLines())
                {
                    // rotate 90 degrees cw
                    using (var result1 = result.Rotate90(1))
                    {
                        // effectively remove vertical lines
                        using (var result2 = result1.RemoveLines())
                        {
                            // rotate 90 degrees ccw
                            using (var result3 = result2.Rotate90(-1))
                            {
                                // TODO: Visualy confirm successful rotation and then setup an assertion to compare that result is the same.
                                SaveResult(result3, "tableBordersRemoved.png");
                            }
                        }
                    }
                }
            }
        }

        [Test]
        public void LineRemovalTest()
        {
            if (!Directory.Exists(ResultsDirectory)) Directory.CreateDirectory(ResultsDirectory);

            using (var pixs = Pix.LoadFromFile(@".\Data\processing\dave-orig.png"))
            {
                float angle, conf;
                IntPtr pix1, pix2, pix3, pix4, pix5;
                IntPtr pix6, pix7, pix8, pix9;

                /* threshold to binary, extracting much of the lines */
                pix1 = Interop.LeptonicaApi.Native.pixThresholdToBinary(pixs.Handle, 170);
                Interop.LeptonicaApi.Native.pixWrite(Path.Combine(ResultsDirectory, "dave-proc1.png"), new HandleRef(this, pix1), Tesseract.ImageFormat.Png);
                Interop.LeptonicaApi.Native.pixDisplayWrite(new HandleRef(this, pix1), 1);

                /* find the skew angle and deskew using an interpolated
                 * rotator for anti-aliasing (to avoid jaggies) */
                Interop.LeptonicaApi.Native.pixFindSkew(new HandleRef(this, pix1), out angle, out conf);
                pix2 = Interop.LeptonicaApi.Native.pixRotateAMGray(pixs.Handle, (float)(Pix.Deg2Rad * angle), (byte)255);
                Interop.LeptonicaApi.Native.pixWrite(Path.Combine(ResultsDirectory, "dave-proc2.png"), new HandleRef(this, pix2), Tesseract.ImageFormat.Png);
                Interop.LeptonicaApi.Native.pixDisplayWrite(new HandleRef(this, pix2), 1);

                /* extract the lines to be removed */
                pix3 = Interop.LeptonicaApi.Native.pixCloseGray(new HandleRef(this, pix2), 51, 1);
                Interop.LeptonicaApi.Native.pixWrite(Path.Combine(ResultsDirectory, "dave-proc3.png"), new HandleRef(this, pix3), Tesseract.ImageFormat.Png);
                Interop.LeptonicaApi.Native.pixDisplayWrite(new HandleRef(this, pix3), 1);

                /* solidify the lines to be removed */
                pix4 = Interop.LeptonicaApi.Native.pixErodeGray(new HandleRef(this, pix3), 1, 5);
                Interop.LeptonicaApi.Native.pixWrite(Path.Combine(ResultsDirectory, "dave-proc4.png"), new HandleRef(this, pix4), Tesseract.ImageFormat.Png);
                Interop.LeptonicaApi.Native.pixDisplayWrite(new HandleRef(this, pix4), 1);

                /* clean the background of those lines */
                pix5 = Interop.LeptonicaApi.Native.pixThresholdToValue(new HandleRef(this, IntPtr.Zero), new HandleRef(this, pix4), 210, 255);
                Interop.LeptonicaApi.Native.pixWrite(Path.Combine(ResultsDirectory, "dave-proc5.png"), new HandleRef(this, pix5), Tesseract.ImageFormat.Png);
                Interop.LeptonicaApi.Native.pixDisplayWrite(new HandleRef(this, pix5), 1);

                pix6 = Interop.LeptonicaApi.Native.pixThresholdToValue(new HandleRef(this, IntPtr.Zero), new HandleRef(this, pix5), 200, 0);
                Interop.LeptonicaApi.Native.pixWrite(Path.Combine(ResultsDirectory, "dave-proc6.png"), new HandleRef(this, pix6), Tesseract.ImageFormat.Png);
                Interop.LeptonicaApi.Native.pixDisplayWrite(new HandleRef(this, pix6), 1);

                /* get paint-through mask for changed pixels */
                pix7 = Interop.LeptonicaApi.Native.pixThresholdToBinary(new HandleRef(this, pix6), 210);
                Interop.LeptonicaApi.Native.pixWrite(Path.Combine(ResultsDirectory, "dave-proc7.png"), new HandleRef(this, pix7), Tesseract.ImageFormat.Png);
                Interop.LeptonicaApi.Native.pixDisplayWrite(new HandleRef(this, pix7), 1);

                /* add the inverted, cleaned lines to orig.  Because
                 * the background was cleaned, the inversion is 0,
                 * so when you add, it doesn't lighten those pixels.
                 * It only lightens (to white) the pixels in the lines! */
                Interop.LeptonicaApi.Native.pixInvert(new HandleRef(this, pix6), new HandleRef(this, pix6));
                pix8 = Interop.LeptonicaApi.Native.pixAddGray(new HandleRef(this, IntPtr.Zero), new HandleRef(this, pix2), new HandleRef(this, pix6));
                Interop.LeptonicaApi.Native.pixWrite(Path.Combine(ResultsDirectory, "dave-proc8.png"), new HandleRef(this, pix8), Tesseract.ImageFormat.Png);
                Interop.LeptonicaApi.Native.pixDisplayWrite(new HandleRef(this, pix8), 1);

                pix9 = Interop.LeptonicaApi.Native.pixOpenGray(new HandleRef(this, pix8), 1, 9);
                Interop.LeptonicaApi.Native.pixWrite(Path.Combine(ResultsDirectory, "dave-proc9.png"), new HandleRef(this, pix9), Tesseract.ImageFormat.Png);
                Interop.LeptonicaApi.Native.pixDisplayWrite(new HandleRef(this, pix9), 1);

                Interop.LeptonicaApi.Native.pixCombineMasked(new HandleRef(this, pix8), new HandleRef(this, pix9), new HandleRef(this, pix7));
                Interop.LeptonicaApi.Native.pixWrite(Path.Combine(ResultsDirectory, "dave-result.png"), new HandleRef(this, pix8), Tesseract.ImageFormat.Png);
                Interop.LeptonicaApi.Native.pixDisplayWrite(new HandleRef(this, pix8), 1);

                Interop.LeptonicaApi.Native.pixDisplayMultiple(Path.Combine(ResultsDirectory, "dave-proc*.png"));

                // resource cleanup
                Interop.LeptonicaApi.Native.pixDestroy(ref pix1);
                Interop.LeptonicaApi.Native.pixDestroy(ref pix2);
                Interop.LeptonicaApi.Native.pixDestroy(ref pix3);
                Interop.LeptonicaApi.Native.pixDestroy(ref pix4);
                Interop.LeptonicaApi.Native.pixDestroy(ref pix5);
                Interop.LeptonicaApi.Native.pixDestroy(ref pix6);
                Interop.LeptonicaApi.Native.pixDestroy(ref pix7);
                Interop.LeptonicaApi.Native.pixDestroy(ref pix8);
                Interop.LeptonicaApi.Native.pixDestroy(ref pix9);
            }
        }

        [Test]
        public void Rotate90Test()
        {
            using (var sourcePix = Pix.LoadFromFile(@".\Data\Conversion\photo_palette_8bpp.png"))
            {
                // rotate 90 degrees cw
                using (var result = sourcePix.Rotate90(1))
                {
                    // TODO: Visualy confirm successful rotation and then setup an assertion to compare that result is the same.
                    SaveResult(result, "imageRotated90.png");
                }
            }
        }

        [Test]
        public void Scale_RGB_ShouldBeScaledBySpecifiedFactor(
            [Values(0.25f, 0.5f, 0.75f, 1, 1.25f, 1.5f, 1.75f, 2, 4, 8)]  float scale)
        {
            const string FileNameFormat = "scale_{0}.jpg";
            using (var sourcePix = Pix.LoadFromFile(@".\Data\Conversion\photo_rgb_32bpp.tif"))
            {
                using (var result = sourcePix.Scale(scale, scale))
                {
                    Assert.That(result.Width, Is.EqualTo((int)Math.Round(sourcePix.Width * scale)));
                    Assert.That(result.Height, Is.EqualTo((int)Math.Round(sourcePix.Height * scale)));

                    // TODO: Visualy confirm successful rotation and then setup an assertion to compare that result is the same.
                    var filename = String.Format(FileNameFormat, scale);
                    SaveResult(result, filename);
                }
            }
        }

        private void SaveResult(Pix result, string filename)
        {
            if (!Directory.Exists(ResultsDirectory)) Directory.CreateDirectory(ResultsDirectory);

            result.Save(Path.Combine(ResultsDirectory, filename));
        }
    }
}