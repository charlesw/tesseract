using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tesseract.Tests.Leptonica.PixTests
{
    [TestFixture]
    public class PixATests : TesseractTestBase
    {
        [Test]
        public void CanCreatePixArray()
        {
            using (var pixA = PixArray.Create(0))
            {
                Assert.That(pixA.Count, Is.EqualTo(0));
            }
        }

        [Test]
        public void CanAddPixToPixArray()
        {
            var sourcePixPath = TestFilePath(@"Ocr/phototest.tif");
            using (var pixA = PixArray.Create(0))
            {
                using (var sourcePix = Pix.LoadFromFile(sourcePixPath))
                {
                    pixA.Add(sourcePix);
                    Assert.That(pixA.Count, Is.EqualTo(1));
                    using (var targetPix = pixA.GetPix(0))
                    {
                        Assert.That(targetPix, Is.EqualTo(sourcePix));
                    }
                }
            }
        }

        [Test]
        public void CanRemovePixFromArray()
        {
            var sourcePixPath = TestFilePath(@"Ocr/phototest.tif");
            using (var pixA = PixArray.Create(0))
            {
                using (var sourcePix = Pix.LoadFromFile(sourcePixPath))
                {
                    pixA.Add(sourcePix);
                }

                pixA.Remove(0);
                Assert.That(pixA.Count, Is.EqualTo(0));
            }
        }

        [Test]
        public void CanClearPixArray()
        {
            var sourcePixPath = TestFilePath(@"Ocr/phototest.tif");
            using (var pixA = PixArray.Create(0))
            {
                using (var sourcePix = Pix.LoadFromFile(sourcePixPath))
                {
                    pixA.Add(sourcePix);
                }

                pixA.Clear();

                Assert.That(pixA.Count, Is.EqualTo(0));
            }
        }
    }
}
