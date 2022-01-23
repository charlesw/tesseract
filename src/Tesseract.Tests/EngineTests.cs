using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
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
        public void CanGetVersion()
        {
            using (var engine = CreateEngine())
            {
                Assert.That(engine.Version, Does.StartWith("5.0.0"));
            }
        }

        [Test]
        public void CanParseMultipageTif()
        {
            using (var engine = CreateEngine()) {
                // load all pages at once
                using (var pixA = PixArray.LoadMultiPageTiffFromFile(TestFilePath("./processing/multi-page.tif"))) {
                    int i = 0;
                    foreach (var pix in pixA) {
                        using (var page = engine.Process(pix)) {
                            var text = page.GetText().Trim();

                            string expectedText = String.Format("Page {0}", ++i);
                            Assert.That(text, Is.EqualTo(expectedText));
                        }
                    }
                }
            }
        }

        [Test]
        public void CanParseMultipageTifOneByOne()
        {
            using (var engine = CreateEngine())
            {
                int offset = 0;
                int i = 0;
                do
                {
                    // read pages one at a time
                    using (var img = Pix.pixReadFromMultipageTiff(TestFilePath("./processing/multi-page.tif"), ref offset))
                    {
                        using (var page = engine.Process(img))
                        {
                            var text = page.GetText().Trim();
                            string expectedText = String.Format("Page {0}", ++i);
                            Assert.That(text, Is.EqualTo(expectedText));
                        }
                    }         
                } while (offset != 0);
            }
        }

        [Test]
        [TestCase(PageSegMode.SingleBlock, "This is a lot of 12 point text to test the\nocr code and see if it works on all types\nof file format.")]
        [TestCase(PageSegMode.SingleColumn, "This is a lot of 12 point text to test the")]
        [TestCase(PageSegMode.SingleLine, "This is a lot of 12 point text to test the")]
        [TestCase(PageSegMode.SingleWord, "This")]
        [TestCase(PageSegMode.SingleChar, "T")]
        [TestCase(PageSegMode.SingleBlockVertText, "A line of text", Ignore = "#490")]
        public void CanParseText_UsingMode(PageSegMode mode, String expectedText)
        {

            using (var engine = CreateEngine(mode:EngineMode.TesseractAndLstm)) {
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
                            "This is a lot of 12 point text to test the\nocr code and see if it works on all types\nof file format.\n\nThe quick brown dog jumped over the\nlazy fox. The quick brown dog jumped\nover the lazy fox. The quick brown dog\njumped over the lazy fox. The quick\nbrown dog jumped over the lazy fox.\n";

                        Assert.That(text, Is.EqualTo(expectedText));
                    }
                }
            }
        }

        [Test, Ignore("See #594")]
        public void CanParseUznFile()
        {
            using (var engine = CreateEngine()) {
                var inputFilename = TestFilePath(@"Ocr/uzn-test.png");
                using (var img = Pix.LoadFromFile(inputFilename)) {
                    using (var page = engine.Process(img, inputFilename, PageSegMode.SingleLine)) {
                        var text = page.GetText();

                        const string expectedText =
                            "This is another test\n";

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
                var testImgFilename = TestFilePath(@"Ocr/phototest.tif");
                using (var img = new Bitmap(testImgFilename)) {
                    using (var page = engine.Process(img)) {
                        var text = page.GetText();

                        const string expectedText =
                            "This is a lot of 12 point text to test the\nocr code and see if it works on all types\nof file format.\n\nThe quick brown dog jumped over the\nlazy fox. The quick brown dog jumped\nover the lazy fox. The quick brown dog\njumped over the lazy fox. The quick\nbrown dog jumped over the lazy fox.\n";

                        Assert.That(text, Is.EqualTo(expectedText));
                    }
                }
            }
        }
#endif
        [Test, Ignore("#489")]
        public void CanProcessSpecifiedRegionInImage()
        {
            using (var engine = CreateEngine(mode:EngineMode.LstmOnly))
            {
                using (var img = LoadTestPix(TestImagePath))
                {
                    using (var page = engine.Process(img, Rect.FromCoords(0, 0, img.Width, 188)))
                    {
                        var region1Text = page.GetText();

                        const string expectedTextRegion1 =
                            "This is a lot of 12 point text to test the\nocr code and see if it works on all types\nof file format.\n";

                        Assert.That(region1Text, Is.EqualTo(expectedTextRegion1));
                    }
                }
            }
        }

        [Test, Ignore("#489")]
        public void CanProcessDifferentRegionsInSameImage()
        {
            using (var engine = CreateEngine()) {
                using (var img = LoadTestPix(TestImagePath)) {
                    using (var page = engine.Process(img, Rect.FromCoords(0, 0, img.Width, 188))) {
                        var region1Text = page.GetText();

                        const string expectedTextRegion1 =
                            "This is a lot of 12 point text to test the\ncor code and see if it works on all types\nof file format.\n";

                        Assert.That(region1Text, Is.EqualTo(expectedTextRegion1));

                        page.RegionOfInterest = Rect.FromCoords(0, 188, img.Width, img.Height);

                        var region2Text = page.GetText();
                        const string expectedTextRegion2 =
                            "The quick brown dog jumped over the\nlazy fox. The quick brown dog jumped\nover the lazy fox. The quick brown dog\njumped over the lazy fox. The quick\nbrown dog jumped over the lazy fox.\n";

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
                using (var img = LoadTestPix("Ocr/empty.png")) {
                    using (var page = engine.Process(img)) {
                        actualResult = PageSerializer.Serialize(page, false);
                    }
                }
            }

            Assert.That(actualResult, Is.EqualTo(
TestUtils.NormaliseNewLine(@"</word></line>
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
                                "This is a lot of 12 point text to test the\nocr code and see if it works on all types\nof file format.\n\nThe quick brown dog jumped over the\nlazy fox. The quick brown dog jumped\nover the lazy fox. The quick brown dog\njumped over the lazy fox. The quick\nbrown dog jumped over the lazy fox.\n";

                            Assert.That(text, Is.EqualTo(expectedText));
                        }
                    }
                }
            }
        }

        [Test]
        public void CanProcessPixUsingResultIterator()
        {
            const string ResultPath = @"EngineTests/CanProcessPixUsingResultIterator.txt";
            var actualResultPath = TestResultRunFile(ResultPath);

            using (var engine = CreateEngine()) {
                using (var img = LoadTestPix(TestImagePath)) {
                    using (var page = engine.Process(img)) {
                        var pageString = PageSerializer.Serialize(page, false);
                        File.WriteAllText(actualResultPath, pageString);
                    }
                }
            }

            CheckResult(ResultPath);
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
                                "This is a lot of 12 point text to test the\nocr code and see if it works on all types\nof file format.\n\nThe quick brown dog jumped over the\nlazy fox. The quick brown dog jumped\nover the lazy fox. The quick brown dog\njumped over the lazy fox. The quick\nbrown dog jumped over the lazy fox.";

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
            var resultFilename = String.Format("EngineTests/CanGenerateHOCROutput_{0}.txt", useXHtml);

            using (var engine = CreateEngine()) {
                using (var img = LoadTestPix(TestImagePath)) {
                    using (var page = engine.Process(img)) {
                        var actualResult = TestUtils.NormaliseNewLine(page.GetHOCRText(1, useXHtml));
                        File.WriteAllText(TestResultRunFile(resultFilename), actualResult);
                    }
                }
            }

            CheckResult(resultFilename);
        }

        [Test]
        public void CanGenerateAltoOutput()
        {
            var resultFilename = String.Format("EngineTests/CanGenerateAltoOutput.txt");

            using (var engine = CreateEngine())
            {
                using (var img = LoadTestPix(TestImagePath))
                {
                    using (var page = engine.Process(img))
                    {
                        var actualResult = TestUtils.NormaliseNewLine(page.GetAltoText(1));
                        File.WriteAllText(TestResultRunFile(resultFilename), actualResult);
                    }
                }
            }

            CheckResult(resultFilename);
        }

        [Test]
        public void CanGenerateTsvOutput()
        {
            var resultFilename = String.Format("EngineTests/CanGenerateTsvOutput.txt");

            using (var engine = CreateEngine())
            {
                using (var img = LoadTestPix(TestImagePath))
                {
                    using (var page = engine.Process(img))
                    {
                        var actualResult = TestUtils.NormaliseNewLine(page.GetTsvText(1));
                        File.WriteAllText(TestResultRunFile(resultFilename), actualResult);
                    }
                }
            }

            CheckResult(resultFilename);
        }

        [Test]
        public void CanGenerateBoxOutput()
        {
            var resultFilename = String.Format("EngineTests/CanGenerateBoxOutput.txt");
            using (var engine = CreateEngine())
            {
                using (var img = LoadTestPix(TestImagePath))
                {
                    using (var page = engine.Process(img))
                    {
                        var actualResult = TestUtils.NormaliseNewLine(page.GetBoxText(1));
                        File.WriteAllText(TestResultRunFile(resultFilename), actualResult);
                    }
                }
            }

            CheckResult(resultFilename);
        }

        [Test]
        public void CanGenerateLSTMBoxOutput()
        {
            var resultFilename = String.Format("EngineTests/CanGenerateLSTMBoxOutput.txt");

            using (var engine = CreateEngine())
            {
                using (var img = LoadTestPix(TestImagePath))
                {
                    using (var page = engine.Process(img))
                    {
                        var actualResult = TestUtils.NormaliseNewLine(page.GetLSTMBoxText(1));
                        File.WriteAllText(TestResultRunFile(resultFilename), actualResult);
                    }
                }
            }

            CheckResult(resultFilename);
        }

        [Test]
        public void CanGenerateWordStrBoxOutput()
        {
            var resultFilename = "EngineTests/CanGenerateWordStrBoxOutput.txt";

            using (var engine = CreateEngine())
            {
                using (var img = LoadTestPix(TestImagePath))
                {
                    using (var page = engine.Process(img))
                    {
                        var actualResult = TestUtils.NormaliseNewLine(page.GetWordStrBoxText(1));
                        File.WriteAllText(TestResultRunFile(resultFilename), actualResult);
                    }
                }
            }

            CheckResult(resultFilename);
        }

        [Test]
        public void CanGenerateUNLVOutput()
        {
            var resultFilename = "EngineTests/CanGenerateUNLVOutput.txt";

            using (var engine = CreateEngine())
            {
                using (var img = LoadTestPix(TestImagePath))
                {
                    using (var page = engine.Process(img))
                    {
                        var actualResult = TestUtils.NormaliseNewLine(page.GetUNLVText());
                        File.WriteAllText(TestResultRunFile(resultFilename), actualResult);
                    }
                }
            }

            CheckResult(resultFilename);
        }

        [Test]
        public void CanProcessPixUsingResultIteratorAndChoiceIterator()
        {
            const string resultFilename = @"EngineTests/CanProcessPixUsingResultIteratorAndChoiceIterator.txt";

            using (var engine = CreateEngine())
            {
                using (var img = LoadTestPix(TestImagePath))
                {
                    using (var page = engine.Process(img))
                    {
                        var pageString = PageSerializer.Serialize(page, true);
                        File.WriteAllText(TestResultRunFile(resultFilename), pageString);
                    }
                }
            }

            CheckResult(resultFilename);
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
                            "This is a lot of 12 point text to test the\nocr code and see if it works on all types\nof file format.\n\nThe quick brown dog jumped over the\nlazy fox. The quick brown dog jumped\nover the lazy fox. The quick brown dog\njumped over the lazy fox. The quick\nbrown dog jumped over the lazy fox.\n";
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
                AbsolutePath(@"./tessdata"),
                AbsolutePath(@"./tessdata/"),
                AbsolutePath(@".\tessdata\")
            };
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

                        const string expectedText = "1234567890\n";

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

        #endregion Variable set/get

        #region Variable print

        [Test]
        public void CanPrintVariables()
        {
            const string ResultFilename = @"EngineTests/CanPrintVariables.txt";
            using (var engine = CreateEngine()) {
                var actualResultsFilename = TestResultRunFile(ResultFilename);
                Assert.That(engine.TryPrintVariablesToFile(actualResultsFilename), Is.True);

                // Load the expected results and verify that they match
                CheckResult(ResultFilename);
            }
        }

        #endregion
    }
}