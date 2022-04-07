using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tesseract.Tests.ResultIteratorTests
{
    [TestFixture]
    public class FontAttributesTests : TesseractTestBase
    {
        private TesseractEngine Engine { get; set; }
        private Pix TestImage { get; set; }

        [SetUp]
        public void Init()
        {
            Engine = CreateEngine(mode: EngineMode.TesseractOnly);
            TestImage = LoadTestPix("Ocr/Fonts.tif");
        }

        [TearDown]
        public void Dispose()
        {
            if (TestImage != null) {
                TestImage.Dispose();
                TestImage = null;
            }

            if (Engine != null) {
                Engine.Dispose();
                Engine = null;
            }
        }

        #region Tests
        [Test]
        public void GetWordFontAttributesWorks()
        {
            using (var page = Engine.Process(TestImage))
            using (var iter = page.GetIterator()) {
                // font attributes come in this order in the test image:
                // bold, italic, monospace, serif, smallcaps
                //
                // there is no test for underline because in 3.04 IsUnderlined is
                // hard-coded to "false".  See: https://github.com/tesseract-ocr/tesseract/blob/3.04/ccmain/ltrresultiterator.cpp#182
                // Note: GetWordFontAttributes returns null if font failed to be resolved (https://github.com/charlesw/tesseract/issues/607)

                var fontAttrs = iter.GetWordFontAttributes();
                Assert.That(fontAttrs.FontInfo.IsBold, Is.True);
                Assert.That(iter.GetWordRecognitionLanguage(), Is.EqualTo("eng"));
                //Assert.That(iter.GetWordIsFromDictionary(), Is.True);
                iter.Next(PageIteratorLevel.TextLine, PageIteratorLevel.Word);

                fontAttrs = iter.GetWordFontAttributes();
                Assert.That(fontAttrs.FontInfo.IsItalic, Is.True);
                iter.Next(PageIteratorLevel.TextLine, PageIteratorLevel.Word);

                fontAttrs = iter.GetWordFontAttributes();
                Assert.That(fontAttrs.FontInfo.IsFixedPitch, Is.True);
                iter.Next(PageIteratorLevel.TextLine, PageIteratorLevel.Word);

                fontAttrs = iter.GetWordFontAttributes();
                Assert.That(fontAttrs.FontInfo.IsSerif, Is.True);
                iter.Next(PageIteratorLevel.TextLine, PageIteratorLevel.Word);

                fontAttrs = iter.GetWordFontAttributes();
                Assert.That(fontAttrs.IsSmallCaps, Is.True);
                iter.Next(PageIteratorLevel.TextLine, PageIteratorLevel.Word);

                Assert.That(iter.GetWordIsNumeric(), Is.True);

                iter.Next(PageIteratorLevel.TextLine, PageIteratorLevel.Word);
                iter.Next(PageIteratorLevel.Word, PageIteratorLevel.Symbol);

                Assert.That(iter.GetSymbolIsSuperscript(), Is.True);

                iter.Next(PageIteratorLevel.TextLine, PageIteratorLevel.Word);
                iter.Next(PageIteratorLevel.Word, PageIteratorLevel.Symbol);

                Assert.That(iter.GetSymbolIsSubscript(), Is.True);
            }
        }
        #endregion Tests
    }
}
