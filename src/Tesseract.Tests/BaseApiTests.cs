using NUnit.Framework;
using System;
using System.Diagnostics;

namespace Tesseract.Tests
{
    [TestFixture]
    public class BaseApiTests
    {
        [Test]
        public void GetVersion_Is410()
        {
            var version = Interop.TessApi.BaseApiGetVersion();
            Assert.That(version, Does.StartWith("4.1.0"));
        }
    }
}