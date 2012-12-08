using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Tesseract.Tests.Leptonica
{
    [TestFixture]
    public class DataAccessTests
    {
        const int Width = 3308, Height = 4676;
              
        [Test]
        public void CanReadAndWrite1BitData()
        {
            const int depth = 1;
            const int length = Width * Height;

            uint sum = 0;
            using (var pix = (Pix1Bit)Pix.Create(Width, Height, depth)) {
                for (int i = 0; i < length; i++) {
                    uint val = (uint)(i % (1 << depth));
                    pix[i] = val;
                    var readVal = pix[i];
                    sum += readVal;
                }
            }
           // Assert.That(readVal, Is.EqualTo(val));
        }

        [Test]
        public void CanReadAndWrite2BitData()
        {
            const int depth = 2;
            const int length = Width * Height;

            uint sum = 0;
            using (var pix = (Pix2Bit)Pix.Create(Width, Height, depth)) {
                for (int i = 0; i < length; i++) {
                    uint val = (uint)(i % (1 << depth));
                    pix[i] = val;
                    var readVal = pix[i];
                    sum += readVal;
                }
            }
            // Assert.That(readVal, Is.EqualTo(val));
        }

        [Test]
        public void CanReadAndWrite4BitData()
        {
            const int depth = 4;
            const int length = Width * Height;

            uint sum = 0;
            using (var pix = (Pix4Bit)Pix.Create(Width, Height, depth)) {
                for (int i = 0; i < length; i++) {
                    uint val = (uint)(i % (1 << depth));
                    pix[i] = val;
                    var readVal = pix[i];
                    sum += readVal;
                }
            }
            // Assert.That(readVal, Is.EqualTo(val));
        }

        [Test]
        public void CanReadAndWrite8BitData()
        {
            const int depth = 8;
            const int length = Width * Height;

            uint sum = 0;
            using (var pix = (Pix8Bit)Pix.Create(Width, Height, depth)) {
                for (int i = 0; i < length; i++) {
                    byte val = (byte)(i % (1 << depth));
                    pix[i] = val;
                    var readVal = pix[i];
                    sum += readVal;
                }
            }
            // Assert.That(readVal, Is.EqualTo(val));
        }

        [Test]
        public void CanReadAndWrite16BitData()
        {
            const int depth = 16;
            const int length = Width * Height;

            uint sum = 0;
            using (var pix = (Pix16Bit)Pix.Create(Width, Height, depth)) {
                for (int i = 0; i < length; i++) {
                    ushort val = (ushort)(i % (1 << depth));
                    pix[i] = val;
                    var readVal = pix[i];
                    sum += readVal;
                }
            }
            // Assert.That(readVal, Is.EqualTo(val));
        }

        [Test]
        public void CanReadAndWrite32BitData()
        {
            const int depth = 32;
            const int length = Width * Height;

            uint sum = 0;
            using (var pix = (Pix32Bit)Pix.Create(Width, Height, depth)) {
                for (int i = 0; i < length; i++) {
                    uint val = (uint)(i % (1 << depth));
                    pix[i] = val;
                    var readVal = pix[i];
                    sum += readVal;
                }
            }
            // Assert.That(readVal, Is.EqualTo(val));
        }
    }
}
