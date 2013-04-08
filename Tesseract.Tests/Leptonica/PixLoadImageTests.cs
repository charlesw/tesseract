using System.Diagnostics;
using System.Drawing;
using System.IO;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tesseract.Tests.Leptonica
{
    [TestFixture]
    public class PixLoadImageTests
    {

        const string DataDirectory = @"Data\Conversion\";

        [Test]
        public void CanLoadFromDisk(
            [Values("photo_grayscale_1bpp.tif")]
            string sourcefile)
        {
            using (var image = new Bitmap(Path.Combine(DataDirectory, sourcefile)))
            using (var pix = Pix.LoadFromFile(Path.Combine(DataDirectory, sourcefile)))
            {
                Assert.AreEqual(image.Width, pix.Width);
                Assert.AreEqual(image.Height, pix.Height); 
            }
        }

        [Test]
        public void CanLoadFromByteArray(
            [Values("photo_grayscale_1bpp.tif")]
            string sourcefile)
        {
            var bytes = File.ReadAllBytes(Path.Combine(DataDirectory, sourcefile));
            using (var image = new Bitmap(Path.Combine(DataDirectory, sourcefile)))
            using (var pix = Pix.LoadFromByteArray(bytes))
            {
                Assert.AreEqual(image.Width, pix.Width);
                Assert.AreEqual(image.Height, pix.Height);
            }
        }

    }
}
