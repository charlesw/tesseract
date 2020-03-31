﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Tesseract.Interop;

namespace Tesseract.Tests
{
    [TestFixture]
    public class EngineTests : TesseractTestBase
    {
        private const string TestImagePath = "Ocr/phototest.tif";

        [Test]
        public void CanParseMultipageTif()
        {
            using (var engine = CreateEngine()) {
                using (var pixA = PixArray.LoadMultiPageTiffFromFile(TestFilePath("./processing/multi-page.tif"))) {
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
        [TestCase(PageSegMode.SingleBlock, "This is a lot of 12 point text to test the\ncor code and see if it works on all types\nof file format.")]
        [TestCase(PageSegMode.SingleColumn, "This is a lot of 12 point text to test the")]
        [TestCase(PageSegMode.SingleLine, "This is a lot of 12 point text to test the")]
        [TestCase(PageSegMode.SingleWord, "This")]
        [TestCase(PageSegMode.SingleChar, "T")]
        [TestCase(PageSegMode.SingleBlockVertText, "A line of text")]
        public void CanParseText_UsingMode(PageSegMode mode, String expectedText)
        {

            using (var engine = CreateEngine()) {
                var demoFilename = String.Format("./Ocr/PSM_{0}.png", mode);
                using (var pix = LoadTestPix(demoFilename)) {
                    using (var page = engine.Process(pix, mode)) {
                        var text = page.GetText().Trim();

                        Assert.That(text, Is.EqualTo(expectedText));

                    }
                }    
            }
        }

        [Test]
        public void CanParseText()
        {
            using (var engine = CreateEngine()) {
                using (var img = LoadTestPix(TestImagePath)) {
                    using (var page = engine.Process(img)) {
                        var text = page.GetText();

                        const string expectedText =
                            "This is a lot of 12 point text to test the\ncor code and see if it works on all types\nof file format.\n\nThe quick brown dog jumped over the\nlazy fox. The quick brown dog jumped\nover the lazy fox. The quick brown dog\njumped over the lazy fox. The quick\nbrown dog jumped over the lazy fox.\n\n";

                        Assert.That(text, Is.EqualTo(expectedText));
                    }
                }
            }
        }

        [Test]
        public void CanParseUznFile()
        {
            using (var engine = CreateEngine()) {
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

#if NETFULL
        [Test]
        public void CanProcessBitmap()
        {
            using (var engine = CreateEngine()) {
                var testImgFilename = TestFilePath(@"Ocr\phototest.tif");
                using (var img = new Bitmap(testImgFilename)) {
                    using (var page = engine.Process(img)) {
                        var text = page.GetText();

                        const string expectedText =
                            "This is a lot of 12 point text to test the\ncor code and see if it works on all types\nof file format.\n\nThe quick brown dog jumped over the\nlazy fox. The quick brown dog jumped\nover the lazy fox. The quick brown dog\njumped over the lazy fox. The quick\nbrown dog jumped over the lazy fox.\n\n";

                        Assert.That(text, Is.EqualTo(expectedText));
                    }
                }
            }
        }
#endif

        [Test]
        public void CanProcessDifferentRegionsInSameImage()
        {
            using (var engine = CreateEngine()) {
                using (var img = LoadTestPix(TestImagePath)) {
                    using (var page = engine.Process(img, Rect.FromCoords(0, 0, img.Width, 188))) {
                        var region1Text = page.GetText();

                        const string expectedTextRegion1 =
                            "This is a lot of 12 point text to test the\ncor code and see if it works on all types\nof file format.\n\n";

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
        public void CanGetSegmentedRegions()
        {
            int expectedCount = 8; // number of text lines in test image

            using (var engine = CreateEngine())
            {
                var imgPath = TestFilePath(TestImagePath);
                using (var img = Pix.LoadFromFile(imgPath))
                {
                    using (var page = engine.Process(img)) {
                        List<Rectangle> boxes = page.GetSegmentedRegions(PageIteratorLevel.TextLine);

                        for (int i = 0; i < boxes.Count; i++) {
                            Rectangle box = boxes[i];
                            Console.WriteLine(String.Format("Box[{0}]: x={1}, y={2}, w={3}, h={4}", i, box.X, box.Y, box.Width, box.Height));
                        }

                        Assert.AreEqual(boxes.Count, expectedCount);
                    }
                }
            }
        }

        [Test]
        public void CanProcessEmptyPxUsingResultIterator()
        {
            string actualResult;
            using (var engine = CreateEngine()) {
                using (var img = LoadTestPix("ocr/empty.png")) {
                    using (var page = engine.Process(img)) {
                        actualResult = WriteResultsToString(page, false);
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
        public void CanProcessMultiplePixs()
        {
            using (var engine = CreateEngine()) {
                for (int i = 0; i < 3; i++) {
                    using (var img = LoadTestPix(TestImagePath)) {
                        using (var page = engine.Process(img)) {
                            var text = page.GetText();

                            const string expectedText =
                                "This is a lot of 12 point text to test the\ncor code and see if it works on all types\nof file format.\n\nThe quick brown dog jumped over the\nlazy fox. The quick brown dog jumped\nover the lazy fox. The quick brown dog\njumped over the lazy fox. The quick\nbrown dog jumped over the lazy fox.\n\n";

                            Assert.That(text, Is.EqualTo(expectedText));
                        }
                    }
                }
            }
        }

        [Test]
        public void CanProcessPixUsingResultIterator()
        {
            const string ResultPath = @"EngineTests\CanProcessPixUsingResultIterator.txt";
            
            string actualResult;
            using (var engine = CreateEngine()) {
                using (var img = LoadTestPix(TestImagePath)) {
                    using (var page = engine.Process(img)) {
                        actualResult = NormaliseNewLine(WriteResultsToString(page, false));
                    }
                }
            }

            var expectedResultPath = TestResultPath(ResultPath); 
            var expectedResult = NormaliseNewLine(File.ReadAllText(expectedResultPath));
            if (expectedResult != actualResult) {
                var actualResultPath = TestResultRunFile(ResultPath);
                File.WriteAllText(actualResultPath, actualResult);
                Assert.Fail("Expected results to be \"{0}\" but was \"{1}\".", expectedResultPath, actualResultPath);
            }
        }

#if NETFULL
        // Test for [Issue #166](https://github.com/charlesw/tesseract/issues/166)
        [Test]
        public void CanProcessScaledBitmap()
        {
            using (var engine = CreateEngine()) {
                var imagePath = TestFilePath(TestImagePath);
                using (var img = Bitmap.FromFile(imagePath)) {
                    using (var scaledImg = new Bitmap(img, new Size(img.Width * 2, img.Height * 2))) {
                        using (var page = engine.Process(scaledImg)) {
                            var text = page.GetText().Trim();

                            const string expectedText =
                                "This is a lot of 12 point text to test the\ncor code and see if it works on all types\nof file format.\n\nThe quick brown dog jumped over the\nlazy fox. The quick brown dog jumped\nover the lazy fox. The quick brown dog\njumped over the lazy fox. The quick\nbrown dog jumped over the lazy fox.";

                            Assert.That(text, Is.EqualTo(expectedText));
                        }
                    }
                }
            }
        }
#endif

        [Test]
        public void CanGenerateHOCROutput(
            [Values(true, false)] Boolean useXHtml)
        {
            string actualResult; 
            using (var engine = CreateEngine()) {
                using (var img = LoadTestPix(TestImagePath)) {
                    using (var page = engine.Process(img)) {
                        actualResult = NormaliseNewLine(page.GetHOCRText(1, useXHtml));
                    }
                }
            }

            var resultFilename = String.Format("./EngineTests/CanGenerateHOCROutput_{0}.txt", useXHtml);
            string expectedFilename = TestResultPath(resultFilename);
            if (File.Exists(expectedFilename)) {
                var expectedResult = NormaliseNewLine(File.ReadAllText(expectedFilename));
                if (expectedResult != actualResult) {
                    var actualFilename = TestResultRunFile(resultFilename);
                    File.WriteAllText(actualFilename, actualResult);
                    Assert.Fail("Expected results to be {0} but was {1}", expectedFilename, actualFilename);
                }
            } else {
                var actualFilename = TestResultRunFile(resultFilename);
                File.WriteAllText(actualFilename, actualResult);
                Assert.Fail("Expected result did not exist, actual results saved to {0}", actualFilename);
            }
        }

        [Test]
        public void CanProcessPixUsingResultIteratorAndChoiceIterator()
        {
            string actualResult;
            using (var engine = CreateEngine())
            {
                using (var img = LoadTestPix(TestImagePath))
                {
                    using (var page = engine.Process(img))
                    {
                        actualResult = WriteResultsToString(page, true);
                    }
                }
            }

            const string resultFilename = @"EngineTests\CanProcessPixUsingResultIteratorAndChoiceIterator.txt";
            string expectedResultFilename = TestResultPath(resultFilename);         
            var expectedResult = NormaliseNewLine(File.ReadAllText(expectedResultFilename));
                        
            if (expectedResult != actualResult)
            {
                var actualResultPath = TestResultRunFile(resultFilename);
                File.WriteAllText(actualResultPath, actualResult);
                Assert.Fail("Expected results to be {0} but was {1}", expectedResultFilename, actualResultPath);
            }            
        }

        [Test]
        public void Initialise_CanLoadConfigFile()
        {
            using (var engine = new TesseractEngine(DataPath, "eng", EngineMode.Default, "bazzar")) {
                // verify that the config file was loaded
                string user_patterns_suffix;
                if (engine.TryGetStringVariable("user_words_suffix", out user_patterns_suffix)) {
                    Assert.That(user_patterns_suffix, Is.EqualTo("user-words"));
                } else {
                    Assert.Fail("Failed to retrieve value for 'user_words_suffix'.");
                }

                using (var img = LoadTestPix(TestImagePath)) {
                    using (var page = engine.Process(img)) {
                        var text = page.GetText();

                        const string expectedText =
                            "This is a Iot of 12 point text to test the\nocr code and see if it works on all types\nof file format.\n\nThe quick brown dog jumped over the\nlazy fox. The quick brown dog jumped\nover the lazy fox. The quick brown dog\njumped over the lazy fox. The quick\nbrown dog jumped over the lazy fox.\n\n";

                        Assert.That(text, Is.EqualTo(expectedText));
                    }
                }
            }
        }

        [Test]
        public void Initialise_CanPassInitVariables()
        {
            var initVars = new Dictionary<string, object>() {
                { "load_system_dawg", false }
            };
            using (var engine = new TesseractEngine(DataPath, "eng", EngineMode.Default, Enumerable.Empty<string>(), initVars, false)) {
                bool loadSystemDawg;
                if (!engine.TryGetBoolVariable("load_system_dawg", out loadSystemDawg)) {
                    Assert.Fail("Failed to get 'load_system_dawg'.");
                }
                Assert.That(loadSystemDawg, Is.False);
            }
        }

        [Test, Ignore("Missing russian language data")]
        public void Initialise_Rus_ShouldStartEngine()
        {
            using (var engine = new TesseractEngine(DataPath, "rus", EngineMode.Default)) {
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
                using (var engine = new TesseractEngine(AbsolutePath(@"./IDontExist"), "eng", EngineMode.Default)) {
                }
            }, Throws.InstanceOf(typeof(TesseractException)));
        }

        private static IEnumerable<string> DataPaths()
        {
            return new string[] {
                AbsolutePath(@"."),
                AbsolutePath(@".\"),
                AbsolutePath(@"./"),
                AbsolutePath(@"./tessdata"),
                AbsolutePath(@"./tessdata/"),
                AbsolutePath(@".\tessdata\")
            };
        }

        private string WriteResultsToString(Page page, bool outputChoices)
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
                                            output.AppendFormat(CultureInfo.InvariantCulture, "<block confidence=\"{0:P}\" bounds=\"{1}, {2}, {3}, {4}\">", confidence, bounds.X1, bounds.Y1, bounds.X2, bounds.Y2);
                                        } else {
                                            output.AppendFormat(CultureInfo.InvariantCulture, "<block confidence=\"{0:P}\">", confidence);
                                        }
                                        output.AppendLine();
                                    }
                                    if (iter.IsAtBeginningOf(PageIteratorLevel.Para)) {
                                        var confidence = iter.GetConfidence(PageIteratorLevel.Para) / 100;
                                        Rect bounds;
                                        if (iter.TryGetBoundingBox(PageIteratorLevel.Para, out bounds)) {
                                            output.AppendFormat(CultureInfo.InvariantCulture, "<para confidence=\"{0:P}\" bounds=\"{1}, {2}, {3}, {4}\">", confidence, bounds.X1, bounds.Y1, bounds.X2, bounds.Y2);
                                        } else {
                                            output.AppendFormat(CultureInfo.InvariantCulture, "<para confidence=\"{0:P}\">", confidence);
                                        }
                                        output.AppendLine();
                                    }
                                    if (iter.IsAtBeginningOf(PageIteratorLevel.TextLine)) {
                                        var confidence = iter.GetConfidence(PageIteratorLevel.TextLine) / 100;
                                        Rect bounds;
                                        if (iter.TryGetBoundingBox(PageIteratorLevel.TextLine, out bounds)) {
                                            output.AppendFormat(CultureInfo.InvariantCulture, "<line confidence=\"{0:P}\" bounds=\"{1}, {2}, {3}, {4}\">", confidence, bounds.X1, bounds.Y1, bounds.X2, bounds.Y2);
                                        } else {
                                            output.AppendFormat(CultureInfo.InvariantCulture, "<line confidence=\"{0:P}\">", confidence);
                                        }
                                    }
                                    if (iter.IsAtBeginningOf(PageIteratorLevel.Word)) {
                                        var confidence = iter.GetConfidence(PageIteratorLevel.Word) / 100;
                                        Rect bounds;
                                        if (iter.TryGetBoundingBox(PageIteratorLevel.Word, out bounds)) {
                                            output.AppendFormat(CultureInfo.InvariantCulture, "<word confidence=\"{0:P}\" bounds=\"{1}, {2}, {3}, {4}\">", confidence, bounds.X1, bounds.Y1, bounds.X2, bounds.Y2);
                                        } else {
                                            output.AppendFormat(CultureInfo.InvariantCulture, "<word confidence=\"{0:P}\">", confidence);
                                        }
                                    }

                                    // symbol and choices
                                    if (outputChoices)
                                    {
                                        using (var choiceIter = iter.GetChoiceIterator())
                                        {
                                            var symbolConfidence = iter.GetConfidence(PageIteratorLevel.Symbol)/100;
                                            if (choiceIter != null)
                                            {
                                                output.AppendFormat(CultureInfo.InvariantCulture, "<symbol text=\"{0}\" confidence=\"{1:P}\">", iter.GetText(PageIteratorLevel.Symbol), symbolConfidence);
                                                output.Append("<choices>");
                                                do
                                                {
                                                    var choiceConfidence = choiceIter.GetConfidence()/100;
                                                    output.AppendFormat(CultureInfo.InvariantCulture, "<choice text=\"{0}\" confidence\"{1:P}\"/>", choiceIter.GetText(), choiceConfidence);

                                                } while (choiceIter.Next());
                                                output.Append("</choices>");
                                                output.Append("</symbol>");
                                            }
                                            else
                                            {
                                                output.AppendFormat(CultureInfo.InvariantCulture, "<symbol text=\"{0}\" confidence=\"{1:P}\"/>", iter.GetText(PageIteratorLevel.Symbol), symbolConfidence);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        output.Append(iter.GetText(PageIteratorLevel.Symbol));   
                                    }                                    
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
            using (var engine = CreateEngine()) {
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
        [Test]
        public void CanSetClassifyBlnNumericModeVariable()
        {
            using (var engine = CreateEngine()) {
                engine.SetVariable("classify_bln_numeric_mode", 1);

                using (var img = Pix.LoadFromFile(TestFilePath("./processing/numbers.png"))) {
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
            using (var engine = CreateEngine()) {
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
            using (var engine = CreateEngine()) {
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
            using (var engine = CreateEngine()) {
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

        [Test]
        public void CanGetStringVariableThatDoesNotExist()
        {
            using (var engine = CreateEngine()) {
                String result;
                Boolean success = engine.TryGetStringVariable("illegal-variable", out result);
                Assert.That(success, Is.False);
                Assert.That(result, Is.Null);
            }
        }

        #endregion Variable set\get

        #region Variable print

        [Test]
        public void CanPrintVariables()
        {
            const string ResultFilename = @"EngineTests\CanPrintVariables.txt";
            using (var engine = CreateEngine()) {
                var actualResultsFilename = TestResultRunFile(ResultFilename);
                Assert.That(engine.TryPrintVariablesToFile(actualResultsFilename), Is.True);
                var actualResult = NormaliseNewLine(File.ReadAllText(actualResultsFilename));

                // Load the expected results and verify that they match
                var expectedResultFilename = TestResultPath(ResultFilename);
                var expectedResult = NormaliseNewLine(File.ReadAllText(expectedResultFilename));
                if (expectedResult != actualResult) {
                    Assert.Fail("Expected results to be \"{0}\" but was \"{1}\".", expectedResultFilename, actualResultsFilename);
                }
            }
        }

        #endregion
    }
}