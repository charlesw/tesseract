
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
			using(var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default)) {
				
				
			}
		}

        [Test]
        public void CanParseText()
        {
            using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default)) {
                using(var img = Pix.LoadFromFile("./phototest.tif")) {
                    using(var page = engine.Process(img)) {
                        var text = page.GetText();

                        const string expectedText =
                            "This is a lot of 12 point text to test the\nocr code and see if it works on all types\nof file format.\n\nThe quick brown dog jumped over the\nlazy fox. The quick brown dog jumped\nover the lazy fox. The quick brown dog\njumped over the lazy fox. The quick\nbrown dog jumped over the lazy fox.\n\n";

                        Assert.That(text, Is.EqualTo(expectedText));
                    }
                }
            }
        }
	}
}
