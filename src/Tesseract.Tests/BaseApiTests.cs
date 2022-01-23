using NUnit.Framework;
using System;
using System.Diagnostics;

namespace Tesseract.Tests
{
    [TestFixture]
    public class BaseApiTests
    {
        [Test]
        public void CanGetVersion()
        {
            var version = Interop.TessApi.BaseApiGetVersion();
            Assert.That(version, Does.StartWith("5.0.0"));
        }
    }
}