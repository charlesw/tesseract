using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tesseract.Tests.Leptonica.PixTests
{
    [TestFixture]
    public class ScewTests
    {
        [Test]
        public void DescewTest()
        {
            using(var sourcePix = Pix.LoadFromFile(@".\Data\Scew\scewed-phototest.png")) {
                Scew scew;
                using (var descewedImage = sourcePix.Deskew(new ScewSweep(range: 45), Pix.DefaultBinarySearchReduction, Pix.DefaultBinaryThreshold, out scew)) {
                    Assert.That(scew.Angle, Is.EqualTo(-9.953125F).Within(0.00001));
                    Assert.That(scew.Confidence, Is.EqualTo(3.782913F).Within(0.00001));
                    descewedImage.Save("descewedImage.bmp");
                }
            }
        }
    }
}
