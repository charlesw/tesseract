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
                    Console.Write("Test");
                    descewedImage.Save("descewedImage.bmp");
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
                    binarizedImage.Save("binarizedImage.bmp");
                }

                using (var binarizedImage = sourcePix.BinarizeOtsuAdaptiveThreshold(200, 200, 10, 10, 0.1F))
                {
                    Assert.That(binarizedImage, Is.Not.Null);
                    Assert.That(binarizedImage.Handle, Is.Not.EqualTo(IntPtr.Zero));
                    binarizedImage.Save("binarizedImage.bmp");
                }
            }
        }
    }
}