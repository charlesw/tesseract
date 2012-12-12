
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
                using(var img = Pix.LoadFromFile("./phototest.tiff")) {
                    using(var page = engine.Process(img)) {
                        var text = page.GetText();

                        Assert.That(text, Is.EqualTo(""));


                    }
                }
            }
        }
	}
}
