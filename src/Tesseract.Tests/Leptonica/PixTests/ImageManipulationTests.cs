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
                    SaveResult(binarizedImage, "binarizedImage.png");
                }
            }
        }

        [Test]
        public void ConvertRGBToGrayTest()
        {
            using (var sourcePix = Pix.LoadFromFile(@".\Data\Conversion\photo_rgb_32bpp.tif"))
            using (var grayscaleImage = sourcePix.ConvertRGBToGray(1.0F, 1.0F, 1.0F))
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
                      
        private void SaveResult(Pix result, string filename)
        {
            if (!Directory.Exists(ResultsDirectory)) Directory.CreateDirectory(ResultsDirectory);
        	
            result.Save(Path.Combine(ResultsDirectory, filename));
        }
    }
}