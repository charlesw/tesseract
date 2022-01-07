using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tesseract.Tests.ResultIteratorTests
{
    [TestFixture]
    public class OfAnEmptyPixTests : TesseractTestBase
    {
        private TesseractEngine Engine { get; set; }
        private Pix EmptyPix { get; set; }

        [SetUp]
        public void Init()
        {
            Engine = CreateEngine();
            EmptyPix = LoadTestPix("Ocr/blank.tif");
        }

        [TearDown]
        public void Dispose()
        {
            if (EmptyPix != null) {
                EmptyPix.Dispose();
                EmptyPix = null;
            }

            if (Engine != null) {
                Engine.Dispose();
                Engine = null;
            }
        }

        #region Tests

        [Theory]
        public void GetTextReturnNullForEachLevel(PageIteratorLevel level)
        {
            using (var page = Engine.Process(EmptyPix)) {
                using (var iter = page.GetIterator()) {
                    Assert.That(iter.GetText(level), Is.Null);
                }
            }
        }

        #endregion Tests
    }
}