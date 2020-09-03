using NUnit.Framework;

namespace Tesseract.Tests
{
    [TestFixture]
    public class BaseApiTests : TesseractTestBase
    {
        [Test]
        public void GetVersion_Is500()
        {
            using (var engine = CreateEngine())
            {
                var version = engine.Version;
                Assert.That(version, Does.StartWith("5.0.0"));
            }
        }
    }
}