using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace Tesseract.Tests
{
    [TestFixture]
    public class AnalyseResultTests : TesseractTestBase
    {
        private const string ResultsDirectory = @"Results\Analysis\";
        private const string ExampleImagePath = @"Ocr\phototest.tif";

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
        [TestCase(90f)]
        [TestCase(180f)]
        public void AnalyseLayout_RotatedImage(float? angle)
        {
            var exampleImagePath = this.TestFilePath("Ocr/phototest.tif");
            using (var img = LoadTestImage(ExampleImagePath)) {
                using (var rotatedImage = angle.HasValue ? img.Rotate(MathHelper.ToRadians(angle.Value)) : img.Clone()) {
                    rotatedImage.Save(TestResultRunFile(String.Format(@"AnalyseResult\AnalyseLayout_RotateImage_{0}.png", angle)));


                    engine.DefaultPageSegMode = PageSegMode.AutoOsd;
                    using (var page = engine.Process(rotatedImage)) {
                        using (var pageLayout = page.GetIterator()) {
                            pageLayout.Begin();
                            do {
                                var result = pageLayout.GetProperties();
                                // Note: The orientation always seem to be 'PageUp' in Tesseract 3.04 according to this test.
                                Assert.That(result.Orientation, Is.EqualTo(Orientation.PageUp));

                                if(angle.HasValue) {
                                    if (angle == 180f) {
                                        // This isn't correct...
                                        Assert.That(result.WritingDirection, Is.EqualTo(WritingDirection.LeftToRight));
                                        Assert.That(result.TextLineOrder, Is.EqualTo(TextLineOrder.TopToBottom));
                                    } else if (angle == 90f) {
                                        Assert.That(result.WritingDirection, Is.EqualTo(WritingDirection.TopToBottom));
                                        Assert.That(result.TextLineOrder, Is.EqualTo(TextLineOrder.RightToLeft));
                                    } else {
                                        Assert.Fail("Angle not supported.");
                                    }
                                } else {
                                    Assert.That(result.WritingDirection, Is.EqualTo(WritingDirection.LeftToRight));
                                    Assert.That(result.TextLineOrder, Is.EqualTo(TextLineOrder.TopToBottom));
                                }                               
                            } while (pageLayout.Next(PageIteratorLevel.Block));
                        }
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
            using (var img = LoadTestImage(ExampleImagePath)) {
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
            using (var img = LoadTestImage(ExampleImagePath)) {
                using (var rotatedPix = img.Rotate(rotation / 360 * (float)Math.PI * 2)) {
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
            using (var img = LoadTestImage(ExampleImagePath)) {
                using (var page = engine.Process(img)) {
                    using (var pageLayout = page.GetIterator()) {
                        pageLayout.Begin();
                        // get symbol
                        int x, y;
                        using (var elementImg = pageLayout.GetImage(level, padding, out x, out y)) {
                            var elementImgFilename = String.Format(@"AnalyseResult\GetImage\ResultIterator_Image_{0}_{1}_at_({2},{3}).png", level, padding, x, y);

                            // TODO: Ensure generated pix is equal to expected pix, only saving it if it's not.
                            var destFilename = TestResultRunFile(elementImgFilename);
                            elementImg.Save(destFilename, ImageFormat.Png);                           
                        }
                    }
                }
            }
        }

        #endregion Tests

        #region Helpers


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

        private Pix LoadTestImage(string path)
        {
            var fullExampleImagePath = this.TestFilePath(path);
            return Pix.LoadFromFile(fullExampleImagePath);
        }

        #endregion
    }
}