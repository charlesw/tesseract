using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace Tesseract.Tests
{
    [TestFixture]
    public class AnalyseResultTests
    {
        private const string ResultsDirectory = @"Results\Analysis\";

        #region Setup\TearDown

        private TesseractEngine engine;

        [TearDown]
        public void Dispose()
        {
            if (engine != null) {
                engine.Dispose();
                engine = null;
            }
        }

        [SetUp]
        public void Init()
        {
            if (!Directory.Exists(ResultsDirectory)) Directory.CreateDirectory(ResultsDirectory);

            engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default);
        }

        #endregion Setup\TearDown

        #region Tests

        [Test]
        [TestCase(null)]
        [TestCase(RotateFlipType.Rotate90FlipNone)]
        [TestCase(RotateFlipType.Rotate180FlipNone)]
        public void AnalyseLayout_RotatedImage(RotateFlipType? rotation)
        {
            using (var img = new Bitmap(@".\phototest.tif")) {
                if (rotation.HasValue) img.RotateFlip(rotation.Value);
                engine.DefaultPageSegMode = PageSegMode.AutoOsd;
                using (var page = engine.Process(img)) {
                    using (var pageLayout = page.GetIterator()) {
                        pageLayout.Begin();
                        do {
                            var result = pageLayout.GetProperties();
                            // Note: The orientation always seem to be 'PageUp' in Tesseract 3.02 according to this test.
                            Assert.That(result.Orientation, Is.EqualTo(Orientation.PageUp));
                            if (rotation == RotateFlipType.Rotate180FlipNone) {
                                // This isn't correct...
                                Assert.That(result.WritingDirection, Is.EqualTo(WritingDirection.LeftToRight));
                                Assert.That(result.TextLineOrder, Is.EqualTo(TextLineOrder.TopToBottom));
                            } else if (rotation == RotateFlipType.Rotate90FlipNone) {
                                Assert.That(result.WritingDirection, Is.EqualTo(WritingDirection.TopToBottom));
                                Assert.That(result.TextLineOrder, Is.EqualTo(TextLineOrder.RightToLeft));
                            } else if (rotation == null) {
                                Assert.That(result.WritingDirection, Is.EqualTo(WritingDirection.LeftToRight));
                                Assert.That(result.TextLineOrder, Is.EqualTo(TextLineOrder.TopToBottom));
                            }
                            // Not sure...
                        } while (pageLayout.Next(PageIteratorLevel.Block));
                    }
                }
            }
        }

        [Test]
        public void CanDetectOrientationForMode(
            [Values(PageSegMode.Auto,
                PageSegMode.AutoOnly,
                PageSegMode.AutoOsd,
                PageSegMode.CircleWord,
                PageSegMode.OsdOnly,
                PageSegMode.SingleBlock,
                PageSegMode.SingleBlockVertText,
                PageSegMode.SingleChar,
                PageSegMode.SingleColumn,
                PageSegMode.SingleLine,
                PageSegMode.SingleWord)]
            PageSegMode pageSegMode)
        {
            using (var img = Pix.LoadFromFile(@".\phototest.tif")) {
                using (var rotatedPix = img.Rotate((float)Math.PI)) {
                    using (var page = engine.Process(rotatedPix, pageSegMode)) {
                        Orientation orientation;
                        float confidence;
                        page.DetectBestOrientation(out orientation, out confidence);

                        Assert.That(orientation, Is.EqualTo(Orientation.PageDown));
                    }
                }
            }
        }

        [Test]
        [TestCase(0)]
        [TestCase(90f)]
        [TestCase(180f)]
        [TestCase(270f)]
        public void DetectOrientation_RotatedImage(float rotation)
        {
            using (var img = Pix.LoadFromFile(@".\phototest.tif")) {
                using (var rotatedPix = img.Rotate(rotation / 360 * (float)Math.PI * 2)) {
                    //var destFilename = String.Format("RotatedPix_{0}.tif", rotation);
                    //rotatedPix.Save(Path.Combine(ResultsDirectory, destFilename), ImageFormat.Tiff);

                    using (var page = engine.Process(rotatedPix, PageSegMode.OsdOnly)) {
                        Orientation expectedOrientation;
                        float expectedDeskew;
                        ExpectedOrientation(rotation, out expectedOrientation, out expectedDeskew);

                        Orientation orientation;
                        float confidence;
                        page.DetectBestOrientation(out orientation, out confidence);

                        Assert.That(orientation, Is.EqualTo(expectedOrientation));
                    }
                }
            }
        }

        [Test]
        public void GetImage(
            [Values(PageIteratorLevel.Block, PageIteratorLevel.Para, PageIteratorLevel.TextLine, PageIteratorLevel.Word, PageIteratorLevel.Symbol)] PageIteratorLevel level, 
            [Values(0, 3)] int padding)
        {
            using (var img = new Bitmap(@".\phototest.tif")) {
                using (var page = engine.Process(img)) {
                    using (var pageLayout = page.GetIterator()) {
                        pageLayout.Begin();
                        // get symbol
                        int x, y;
                        using (var elementImg = pageLayout.GetImage(level, padding, out x, out y)) {
                            //var destFilename = String.Format("ResultIterator_Image_{0}_{1}_at_({2},{3}).png", level, padding, x, y);
                            //elementImg.Save(Path.Combine(ResultsDirectory, destFilename), ImageFormat.Png);
                        }
                    }
                }
            }
        }

        private void ExpectedOrientation(float rotation, out Orientation orientation, out float deskew)
        {
            rotation = rotation % 360f;
            rotation = rotation < 0 ? rotation + 360 : rotation;

            if (rotation >= 315 || rotation < 45) {
                orientation = Orientation.PageUp;
                deskew = -rotation;
            } else if (rotation >= 45 && rotation < 135) {
                orientation = Orientation.PageRight;
                deskew = 90 - rotation;
            } else if (rotation >= 135 && rotation < 225) {
                orientation = Orientation.PageDown;
                deskew = 180 - rotation;
            } else if (rotation >= 225 && rotation < 315) {
                orientation = Orientation.PageLeft;
                deskew = 270 - rotation;
            } else {
                throw new ArgumentOutOfRangeException("rotation");
            }
        }

        #endregion Tests
    }
}