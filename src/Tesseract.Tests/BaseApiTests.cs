using NUnit.Framework;
using System;
using System.Diagnostics;

namespace Tesseract.Tests
{
    [TestFixture]
    public class BaseApiTests
    {
        [Test,
         Ignore("GetVersion doesn't work for x64, probably compilation related.")]
        public void GetVersion_Is400()
        {
            var version = Interop.TessApi.Native.GetVersion();
            Assert.That(version, Is.StringStarting("4.00"));
        }
    }
}