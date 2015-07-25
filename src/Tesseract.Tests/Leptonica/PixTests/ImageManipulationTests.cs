using System.IO;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            using (var sourcePix = Pix.LoadFromFile(@".\Data\Binarization\neo-8bit-grayscale.png")) {
                using (var grayscalePix = sourcePix.ConvertRGBToGray(1, 1, 1)) {
                    using (var binarizedImage = grayscalePix.BinarizeSauvola(10, 0.35f, false)) {
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
            using (var sourcePix = Pix.LoadFromFile(@".\Data\Binarization\neo-8bit-grayscale.png")) {
                using (var grayscalePix = sourcePix.ConvertRGBToGray(1, 1, 1)) {
                    using (var binarizedImage = grayscalePix.BinarizeSauvolaTiled(10, 0.35f, 2, 2)) {
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
        	using (var sourcePix = Pix.LoadFromFile(@".\Data\Conversion\photo_rgb_32bpp.tif")) {
        		using (var result = sourcePix.Rotate(angleAsRadians, RotationMethod.AreaMap))
	            {
        			// TODO: Visualy confirm successful rotation and then setup an assertion to compare that result is the same.
        			var filename = String.Format(FileNameFormat, angle);
        			SaveResult(result, filename);
	            }
        	}
        }


        [Test]
        public void Scale_RGB_ShouldBeScaledBySpecifiedFactor(
            [Values(0.25f, 0.5f, 0.75f, 1, 1.25f, 1.5f, 1.75f, 2, 4, 8)]  float scale)
        {
        	const string FileNameFormat = "scale_{0}.jpg";
            using (var sourcePix = Pix.LoadFromFile(@".\Data\Conversion\photo_rgb_32bpp.tif")) {
                using (var result = sourcePix.Scale(scale, scale)) {
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