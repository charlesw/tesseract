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
    public class ImageManipulationTests : TesseractTestBase
    {
        const string ResultsDirectory = @"Results/ImageManipulation/";

        [Test]
        public void DescewTest()
        {
            var sourcePixPath = TestFilePath(@"Scew/scewed-phototest.png");
            using (var sourcePix = Pix.LoadFromFile(sourcePixPath))
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
            var sourcePixFilename = TestFilePath(@"Binarization/neo-8bit.png");
            using (var sourcePix = Pix.LoadFromFile(sourcePixFilename))
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
            string sourcePixFilename = TestFilePath(@"Binarization/neo-8bit-grayscale.png");
            using (var sourcePix = Pix.LoadFromFile(sourcePixFilename))
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
            string sourcePixFilename = TestFilePath(@"Binarization/neo-8bit-grayscale.png");
            using (var sourcePix = Pix.LoadFromFile(sourcePixFilename))
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
            var sourcePixFilename = TestFilePath(@"Conversion/photo_rgb_32bpp.tif");
            using (var sourcePix = Pix.LoadFromFile(sourcePixFilename))
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
            float angleAsRadians = MathHelper.ToRadians(angle);

            var sourcePixFilename = TestFilePath(@"Conversion/photo_rgb_32bpp.tif");
            using (var sourcePix = Pix.LoadFromFile(sourcePixFilename))
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
            var sourcePixFilename = TestFilePath(@"processing/table.png");
            using (var sourcePix = Pix.LoadFromFile(sourcePixFilename))
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
        public void DespeckleTest()
        {
            var sourcePixFilename = TestFilePath(@"processing/w91frag.jpg");
            using (var sourcePix = Pix.LoadFromFile(sourcePixFilename))
            {
                // remove speckles
                using (var result = sourcePix.Despeckle(Pix.SEL_STR2, 2))
                {
                    // TODO: Visualy confirm successful despeckle and then setup an assertion to compare that result is the same.
                    SaveResult(result, "w91frag-despeckled.png");
                }
            }
        }

        [Test]
        public void Scale_RGB_ShouldBeScaledBySpecifiedFactor(
            [Values(0.25f, 0.5f, 0.75f, 1, 1.25f, 1.5f, 1.75f, 2, 4, 8)]  float scale)
        {
            const string FileNameFormat = "scale_{0}.jpg";

            var sourcePixFilename = TestFilePath(@"Conversion/photo_rgb_32bpp.tif");
            using (var sourcePix = Pix.LoadFromFile(sourcePixFilename))
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
            var runFilename = TestResultRunFile(Path.Combine(ResultsDirectory, filename));
            result.Save(runFilename);
        }
    }
}