using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Tesseract.Tests
{
    [TestFixture]
    public class AnalyseResultTests
    {
        #region Setup\TearDown
        private TesseractEngine engine;

        [SetUp]
        public void Init()
        {
            engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default);
        }

        [TearDown]
        public void Dispose()
        {
            if (engine != null) {
                engine.Dispose();
            }
        }

        #endregion

        #region Tests

        [Test]
        [TestCase(null)]
        [TestCase(RotateFlipType.Rotate90FlipNone)]
        [TestCase(RotateFlipType.Rotate180FlipNone)]
        public void AnalyseLayout_RotatedImage(RotateFlipType? rotation)
        {
            using(var img = new Bitmap(@".\phototest.tif")) {
        		if(rotation.HasValue) img.RotateFlip(rotation.Value);
        		engine.DefaultPageSegMode = PageSegMode.AutoOsd;
                using(var page = engine.Process(img)) {
        			using (var pageLayout = page.GetIterator()) {
        				pageLayout.Begin();
        				do {
        					var result = pageLayout.GetProperties();
        					// Note: The orientation always seem to be 'PageUp' in Tesseract 3.02 according to this test.
        					Assert.That(result.Orientation, Is.EqualTo(Orientation.PageUp));
        					if(rotation == RotateFlipType.Rotate180FlipNone) {
        						// This isn't correct...
	        					Assert.That(result.WritingDirection, Is.EqualTo(WritingDirection.LeftToRight));        					
	        					Assert.That(result.TextLineOrder, Is.EqualTo(TextLineOrder.TopToBottom));    
        					} else if(rotation == RotateFlipType.Rotate90FlipNone) {
	        					Assert.That(result.WritingDirection, Is.EqualTo(WritingDirection.TopToBottom));        					
	        					Assert.That(result.TextLineOrder, Is.EqualTo(TextLineOrder.RightToLeft));     
        					} else if(rotation == null) {
	        					Assert.That(result.WritingDirection, Is.EqualTo(WritingDirection.LeftToRight));        					
	        					Assert.That(result.TextLineOrder, Is.EqualTo(TextLineOrder.TopToBottom));        					
        					}
        					// Not sure...
        				} while(pageLayout.Next(PageIteratorLevel.Block));
                    }
                }
            }
        }

        #endregion
    }
}
