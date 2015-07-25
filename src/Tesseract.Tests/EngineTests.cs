using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace Tesseract.Tests
{
    [TestFixture]
    public class EngineTests
    {
        [Test]
        public void CanParseUznFile()
        {
            using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default)) {
                var inputFilename = TestFilePath(@"Ocr\uzn-test.png");
                using (var img = Pix.LoadFromFile(inputFilename)) {
                    using (var page = engine.Process(img, inputFilename, PageSegMode.SingleLine)) {
                        var text = page.GetText();

                        const string expectedText =
                            "This is another test\n\n";

                        Assert.That(text, Is.EqualTo(expectedText));
                    }
                }
            }
        }

        [Test]
        public void CanProcessBitmap()
        {
            using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default)) {
                using (var img = new Bitmap("./phototest.tif")) {
                    using (var page = engine.Process(img)) {
                        var text = page.GetText();

                        const string expectedText =
                            "This is a lot of 12 point text to test the\nocr code and see if it works on all types\nof file format.\n\nThe quick brown dog jumped over the\nlazy fox. The quick brown dog jumped\nover the lazy fox. The quick brown dog\njumped over the lazy fox. The quick\nbrown dog jumped over the lazy fox.\n\n";

                        Assert.That(text, Is.EqualTo(expectedText));
                    }
                }
            }
        }

        [Test]
        public void CanProcessDifferentRegionsInSameImage()
        {
            using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default)) {
                using (var img = Pix.LoadFromFile("./phototest.tif")) {
                    using (var page = engine.Process(img, Rect.FromCoords(0, 0, img.Width, 188))) {
                        var region1Text = page.GetText();

                        const string expectedTextRegion1 =
                            "This is a lot of 12 point text to test the\nocr code and see if it works on all types\nof file format.\n\n";

                        Assert.That(region1Text, Is.EqualTo(expectedTextRegion1));

                        page.RegionOfInterest = Rect.FromCoords(0, 188, img.Width, img.Height);

                        var region2Text = page.GetText();
                        const string expectedTextRegion2 =
                            "The quick brown dog jumped over the\nlazy fox. The quick brown dog jumped\nover the lazy fox. The quick brown dog\njumped over the lazy fox. The quick\nbrown dog jumped over the lazy fox.\n\n";

                        Assert.That(region2Text, Is.EqualTo(expectedTextRegion2));
                    }
                }
            }
        }

        [Test]
        public void CanProcessEmptyPxUsingResultIterator()
        {
            string actualResult;
            using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default)) {
                using (var img = Pix.LoadFromFile("./data/ocr/empty.png")) {
                    using (var page = engine.Process(img)) {
                        actualResult = WriteResultsToString(page);
                    }
                }
            }

            Assert.That(actualResult, Is.EqualTo(
NormaliseNewLine(@"</word></line>
</para>
</block>
")));
        }

        [Test]
        public void CanProcessMultipageTif()
        {
            using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default)) {
                using (var pixA = PixArray.LoadMultiPageTiffFromFile("./Data/processing/multi-page.tif")) {
                    int i = 1;
                    foreach (var pix in pixA) {
                        using (var page = engine.Process(pix)) {
                            var text = page.GetText().Trim();

                            string expectedText = String.Format("Page {0}", i);
                            Assert.That(text, Is.EqualTo(expectedText));
                        }
                        i++;
                    }
                }
            }
        }

        [Test]
        public void CanProcessMultiplePixs()
        {
            using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default)) {
                for (int i = 0; i < 3; i++) {
                    using (var img = Pix.LoadFromFile("./phototest.tif")) {
                        using (var page = engine.Process(img)) {
                            var text = page.GetText();

                            const string expectedText =
                                "This is a lot of 12 point text to test the\nocr code and see if it works on all types\nof file format.\n\nThe quick brown dog jumped over the\nlazy fox. The quick brown dog jumped\nover the lazy fox. The quick brown dog\njumped over the lazy fox. The quick\nbrown dog jumped over the lazy fox.\n\n";

                            Assert.That(text, Is.EqualTo(expectedText));
                        }
                    }
                }
            }
        }

        [Test]
        public void CanProcessPix()
        {
            using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default)) {
                using (var img = Pix.LoadFromFile("./phototest.tif")) {
                    using (var page = engine.Process(img)) {
                        var text = page.GetText();

                        const string expectedText =
                            "This is a lot of 12 point text to test the\nocr code and see if it works on all types\nof file format.\n\nThe quick brown dog jumped over the\nlazy fox. The quick brown dog jumped\nover the lazy fox. The quick brown dog\njumped over the lazy fox. The quick\nbrown dog jumped over the lazy fox.\n\n";

                        Assert.That(text, Is.EqualTo(expectedText));
                    }
                }
            }
        }

        // Test for [Issue #166](https://github.com/charlesw/tesseract/issues/166)
        [Test]
        public void CanProcessScaledBitmap()
        {
            using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default)) {
                using (var img = Bitmap.FromFile("./phototest.tif")) {
                    using (var scaledImg = new Bitmap(img, new Size(img.Width * 2, img.Height * 2))) {
                        using (var page = engine.Process(scaledImg)) {
                            var text = page.GetText();

                            const string expectedText =
                                "This is a lot of 12 point text to test the\nocr code and see if it works on all types\nof file format.\n\nThe quick brown dog jumped over the\nlazy fox. The quick brown dog jumped\nover the lazy fox. The quick brown dog\njumped over the lazy fox. The quick\nbrown dog jumped over the lazy fox.\n\n";

                            Assert.That(text, Is.EqualTo(expectedText));
                        }
                    }
                }
            }
        }

        [Test]
        public void CanGenerateHOCROutput()
        {
            string actualResult; 
            using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default)) {
                using (var img = Pix.LoadFromFile("./phototest.tif")) {
                    using (var page = engine.Process(img)) {
                        actualResult = NormaliseNewLine(page.GetHOCRText(1));
                    }
                }
            }


            const string ExpectedResultPath = "./Results/EngineTests.CanGenerateHOCROutput.txt";
            if (File.Exists(ExpectedResultPath)) {
                var expectedResult = NormaliseNewLine(File.ReadAllText(ExpectedResultPath));
                if (expectedResult != actualResult) {
                    var actualResultPath = String.Format("./Results/EngineTests.CanGenerateHOCROutput_{0:yyyyMMddTHHmmss}.txt", DateTime.UtcNow);
                    File.WriteAllText(actualResultPath, actualResult);
                    Assert.Fail("Expected results to be {0} but was {1}", ExpectedResultPath, actualResultPath);
                }
            } else {
                var actualResultPath = String.Format("./Results/EngineTests.CanGenerateHOCROutput_{0:yyyyMMddTHHmmss}.txt", DateTime.UtcNow);
                File.WriteAllText(actualResultPath, actualResult);
                Assert.Fail("Expected result did not exist, actual results saved to {0}", actualResultPath);
            }
        }

        [Test]
        public void CanProcessPixUsingResultIterator()
        {
            string actualResult;
            using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default)) {
                using (var img = Pix.LoadFromFile("./phototest.tif")) {
                    using (var page = engine.Process(img)) {
                        actualResult = WriteResultsToString(page);
                    }
                }
            }

            const string ExpectedResultPath = "./Results/EngineTests.CanProcessPixUsingResultIterator.txt";
            var expectedResult = NormaliseNewLine(File.ReadAllText(ExpectedResultPath));
            if (expectedResult != actualResult) {
                var actualResultPath = String.Format("./Results/EngineTests.CanProcessPixUsingResultIterator_{0:yyyyMMddTHHmmss}.txt", DateTime.UtcNow);
                File.WriteAllText(actualResultPath, actualResult);
                Assert.Fail("Expected results to be {0} but was {1}", ExpectedResultPath, actualResultPath);
            }
        }

        [Test]
        public void Initialise_CanLoadConfigFile()
        {
            var tessDataPath = Path.Combine(Environment.CurrentDirectory, @"tessdata\");
            using (var engine = new TesseractEngine(tessDataPath, "eng", EngineMode.Default, "bazzar")) {
                // verify that the config file was loaded
                string user_patterns_suffix;
                if (engine.TryGetStringVariable("user_words_suffix", out user_patterns_suffix)) {
                    Assert.That(user_patterns_suffix, Is.EqualTo("user-words"));
                } else {
                    Assert.Fail("Failed to retrieve value for 'user_words_suffix'.");
                }

                using (var img = Pix.LoadFromFile("./phototest.tif")) {
                    using (var page = engine.Process(img)) {
                        var text = page.GetText();

                        const string expectedText =
                            "This is a Iot of 12 point text to test the\nocr code and see if it works on all types\nof file format.\n\nThe quick brown dog jumped over the\nIazy fox. The quick brown dog jumped\nover the Iazy fox. The quick brown dog\njumped over the Iazy fox. The quick\nbrown dog jumped over the Iazy fox.\n\n";

                        Assert.That(text, Is.EqualTo(expectedText));
                    }
                }
            }
        }

        [Test, Ignore("Missing russian language data")]
        public void Initialise_Rus_ShouldStartEngine()
        {
            using (var engine = new TesseractEngine(@"./tessdata", "rus", EngineMode.Default)) {
            }
        }

        [Test]
        public void Initialise_ShouldStartEngine(
            [ValueSource("DataPaths")] string datapath)
        {
            using (var engine = new TesseractEngine(datapath, "eng", EngineMode.Default)) {
            }
        }

        [Test]
        public void Initialise_ShouldThrowErrorIfDatapathNotCorrect()
        {
            Assert.That(() => {
                using (var engine = new TesseractEngine(@"./IDontExist", "eng", EngineMode.Default)) {
                }
            }, Throws.InstanceOf(typeof(TesseractException)));
        }

        [Test]
        public void Initialise_WithTessDataPrefixSet()
        {
            Environment.SetEnvironmentVariable("TESSDATA_PREFIX", Environment.CurrentDirectory);
            using (var engine = new TesseractEngine(null, "eng", EngineMode.Default)) {
            }
        }

        [Test]
        public void WritesOutThresholdedImageWhenOCRing()
        {
            var expectedFilePath = Path.Combine(Environment.CurrentDirectory, "tessinput.tif");

            if (File.Exists(expectedFilePath)) {
                File.Delete(expectedFilePath);
            }

            using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default)) {
                engine.SetVariable("tessedit_write_images", true);

                using (var img = Pix.LoadFromFile("./phototest.tif")) {
                    using (var page = engine.Process(img)) {
                        var region1Text = page.GetText();

                        Assert.That(File.Exists(expectedFilePath));
                    }
                }
            }
        }

        private IEnumerable<string> DataPaths()
        {
            return new string[] {
				null,
				@".",
				@".\",
				@"./",
				@"./tessdata",
				@"./tessdata/",
				@".\tessdata\",
				Path.Combine(Environment.CurrentDirectory, @"tessdata"),
				Path.Combine(Environment.CurrentDirectory, @"tessdata\"),
				Path.Combine(Environment.CurrentDirectory, @"tessdata/"),
				Environment.CurrentDirectory
			};
        }

        private string WriteResultsToString(Page page)
        {
            var output = new StringBuilder();
            using (var iter = page.GetIterator()) {
                iter.Begin();
                do {
                    do {
                        do {
                            do {
                                do {
                                    if (iter.IsAtBeginningOf(PageIteratorLevel.Block)) {
                                        var confidence = iter.GetConfidence(PageIteratorLevel.Block) / 100;
                                        Rect bounds;
                                        if (iter.TryGetBoundingBox(PageIteratorLevel.Block, out bounds)) {
                                            output.AppendFormat("<block confidence=\"{0:P}\" bounds=\"{1}, {2}, {3}, {4}\">", confidence, bounds.X1, bounds.Y1, bounds.X2, bounds.Y2);
                                        } else {
                                            output.AppendFormat("<block confidence=\"{0:P}\">", confidence);
                                        }
                                        output.AppendLine();
                                    }
                                    if (iter.IsAtBeginningOf(PageIteratorLevel.Para)) {
                                        var confidence = iter.GetConfidence(PageIteratorLevel.Para) / 100;
                                        Rect bounds;
                                        if (iter.TryGetBoundingBox(PageIteratorLevel.Para, out bounds)) {
                                            output.AppendFormat("<para confidence=\"{0:P}\" bounds=\"{1}, {2}, {3}, {4}\">", confidence, bounds.X1, bounds.Y1, bounds.X2, bounds.Y2);
                                        } else {
                                            output.AppendFormat("<para confidence=\"{0:P}\">", confidence);
                                        }
                                        output.AppendLine();
                                    }
                                    if (iter.IsAtBeginningOf(PageIteratorLevel.TextLine)) {
                                        var confidence = iter.GetConfidence(PageIteratorLevel.TextLine) / 100;
                                        Rect bounds;
                                        if (iter.TryGetBoundingBox(PageIteratorLevel.TextLine, out bounds)) {
                                            output.AppendFormat("<line confidence=\"{0:P}\" bounds=\"{1}, {2}, {3}, {4}\">", confidence, bounds.X1, bounds.Y1, bounds.X2, bounds.Y2);
                                        } else {
                                            output.AppendFormat("<line confidence=\"{0:P}\">", confidence);
                                        }
                                    }
                                    if (iter.IsAtBeginningOf(PageIteratorLevel.Word)) {
                                        var confidence = iter.GetConfidence(PageIteratorLevel.Word) / 100;
                                        Rect bounds;
                                        if (iter.TryGetBoundingBox(PageIteratorLevel.Word, out bounds)) {
                                            output.AppendFormat("<word confidence=\"{0:P}\" bounds=\"{1}, {2}, {3}, {4}\">", confidence, bounds.X1, bounds.Y1, bounds.X2, bounds.Y2);
                                        } else {
                                            output.AppendFormat("<word confidence=\"{0:P}\">", confidence);
                                        }
                                    }
                                    output.Append(iter.GetText(PageIteratorLevel.Symbol));
                                    if (iter.IsAtFinalOf(PageIteratorLevel.Word, PageIteratorLevel.Symbol)) {
                                        output.Append("</word>");
                                    }
                                } while (iter.Next(PageIteratorLevel.Word, PageIteratorLevel.Symbol));

                                if (iter.IsAtFinalOf(PageIteratorLevel.TextLine, PageIteratorLevel.Word)) {
                                    output.AppendLine("</line>");
                                }
                            } while (iter.Next(PageIteratorLevel.TextLine, PageIteratorLevel.Word));
                            if (iter.IsAtFinalOf(PageIteratorLevel.Para, PageIteratorLevel.TextLine)) {
                                output.AppendLine("</para>");
                            }
                        } while (iter.Next(PageIteratorLevel.Para, PageIteratorLevel.TextLine));
                    } while (iter.Next(PageIteratorLevel.Block, PageIteratorLevel.Para));
                    output.AppendLine("</block>");
                } while (iter.Next(PageIteratorLevel.Block));
            }
            return NormaliseNewLine(output.ToString());
        }

        #region Variable set\get

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void CanSetBooleanVariable(bool variableValue)
        {
            const string VariableName = "classify_enable_learning";
            using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default)) {
                var variableWasSet = engine.SetVariable(VariableName, variableValue);
                Assert.That(variableWasSet, Is.True, "Failed to set variable '{0}'.", VariableName);
                bool result;
                if (engine.TryGetBoolVariable(VariableName, out result)) {
                    Assert.That(result, Is.EqualTo(variableValue));
                } else {
                    Assert.Fail("Failed to retrieve value for '{0}'.", VariableName);
                }
            }
        }

        /// <summary>
        /// As per Bug #52 setting 'classify_bln_numeric_mode' variable to '1' causes the engine to fail on processing.
        /// </summary>
        [Test,
         Ignore("Broken in Tesseract 3.02")]
        public void CanSetClassifyBlnNumericModeVariable()
        {
            using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default)) {
                engine.SetVariable("classify_bln_numeric_mode", 1);

                using (var img = Pix.LoadFromFile("./Data/processing/numbers.png")) {
                    using (var page = engine.Process(img)) {
                        var text = page.GetText();

                        const string expectedText = "1234567890\n\n";

                        Assert.That(text, Is.EqualTo(expectedText));
                    }
                }
            }
        }

        [Test]
        [TestCase("edges_boxarea", 0.875)]
        [TestCase("edges_boxarea", 0.9)]
        [TestCase("edges_boxarea", -0.9)]
        public void CanSetDoubleVariable(string variableName, double variableValue)
        {
            using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default)) {
                var variableWasSet = engine.SetVariable(variableName, variableValue);
                Assert.That(variableWasSet, Is.True, "Failed to set variable '{0}'.", variableName);
                double result;
                if (engine.TryGetDoubleVariable(variableName, out result)) {
                    Assert.That(result, Is.EqualTo(variableValue));
                } else {
                    Assert.Fail("Failed to retrieve value for '{0}'.", variableName);
                }
            }
        }

        [Test]
        [TestCase("edges_children_count_limit", 45)]
        [TestCase("edges_children_count_limit", 20)]
        [TestCase("textord_testregion_left", 20)]
        [TestCase("textord_testregion_left", -20)]
        public void CanSetIntegerVariable(string variableName, int variableValue)
        {
            using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default)) {
                var variableWasSet = engine.SetVariable(variableName, variableValue);
                Assert.That(variableWasSet, Is.True, "Failed to set variable '{0}'.", variableName);
                int result;
                if (engine.TryGetIntVariable(variableName, out result)) {
                    Assert.That(result, Is.EqualTo(variableValue));
                } else {
                    Assert.Fail("Failed to retrieve value for '{0}'.", variableName);
                }
            }
        }

        [Test]
        [TestCase("tessedit_char_whitelist", "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ")]
        [TestCase("tessedit_char_whitelist", "")]
        [TestCase("tessedit_char_whitelist", "Test")]
        [TestCase("tessedit_char_whitelist", "chinese 漢字")] // Issue 68
        public void CanSetStringVariable(string variableName, string variableValue)
        {
            using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default)) {
                var variableWasSet = engine.SetVariable(variableName, variableValue);
                Assert.That(variableWasSet, Is.True, "Failed to set variable '{0}'.", variableName);
                string result;
                if (engine.TryGetStringVariable(variableName, out result)) {
                    Assert.That(result, Is.EqualTo(variableValue));
                } else {
                    Assert.Fail("Failed to retrieve value for '{0}'.", variableName);
                }
            }
        }

        #endregion Variable set\get

        #region File Helpers

        private string TestFilePath(string path)
        {
            var basePath = Path.Combine(Environment.CurrentDirectory, "Data");
            return Path.Combine(basePath, path);
        }

        /// <summary>
        /// Normalise new line characters to unix (\n) so they are all the same.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private string NormaliseNewLine(string text)
        {
            return text
                .Replace("\r\n", "\n")
                .Replace("\r", "\n");
        }

        #endregion File Helpers
    }
}