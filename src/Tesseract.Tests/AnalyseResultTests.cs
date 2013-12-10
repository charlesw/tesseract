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

        public void AnalyseBitmap()
        {
            using(var img = new Bitmap(@".\phototest.tif")) {
                using(var page = engine.Process(img)) {
                    using (var pageLayout = page.AnalyseLayout()) {

                    }
                }
            }
        }

        #endregion
    }
}
