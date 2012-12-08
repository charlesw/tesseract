
using System;
using NUnit.Framework;

namespace Tesseract.Tests
{
	[TestFixture]
	public class EngineTests
	{
		[Test]
		public void Initialise_ShouldStartEngine()
		{
			using(var engine = new Engine(@"./tessdata", "eng", EngineMode.Default)) {
				
				
			}
		}

        [Test]
        public void CanParseText()
        {
            using (var engine = new Engine(@"./tessdata", "eng", EngineMode.Default)) {
                using(var img = Pix.LoadFromFile("./phototest.tiff")) {
                    using(var iter = engine.Process(img)) {
                        var paraText = iter.GetText(PageIteratorLevel.Para);
                        var blockText = iter.GetText(PageIteratorLevel.Block);
                        var lineText = iter.GetText(PageIteratorLevel.TextLine);
                        var wordText = iter.GetText(PageIteratorLevel.Word);
                        var symbol = iter.GetText(PageIteratorLevel.Symbol);


                    }
                }
            }
        }
	}
}
