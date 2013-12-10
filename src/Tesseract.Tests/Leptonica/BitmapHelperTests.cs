using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tesseract.Tests.Leptonica
{
    [TestFixture]
    public class BitmapHelperTests
    {
        [Test]
        public void ConvertRgb555ToPixColor()
        {
            ushort originalVal = 0x39EC;
            var convertedValue = BitmapHelper.ConvertRgb555ToRGBA(originalVal);
            Assert.That(convertedValue, Is.EqualTo(0x737B63FF));
        }

        [Test]
        [TestCase(0xB9EC, 0x737B63FF)]
        [TestCase(0x39EC, 0x737B6300)]
        public void ConvertArgb555ToPixColor(int originalVal, int expectedVal)
        {
            var convertedValue = BitmapHelper.ConvertArgb1555ToRGBA((ushort)originalVal);
            Assert.That(convertedValue, Is.EqualTo((uint)expectedVal));
        }

        [Test]
        public void ConvertRgb565ToPixColor()
        {
            ushort originalVal = 0x73CC;
            var convertedValue = BitmapHelper.ConvertRgb565ToRGBA(originalVal);
            Assert.That(convertedValue, Is.EqualTo(0x737963FF));
        }
    }
}
