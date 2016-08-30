using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tesseract.Tests
{
    [TestFixture]
    public class ResultRendererTests : TesseractTestBase
    {
        #region Test setup and teardown

        TesseractEngine _engine;

        [SetUp]
        public void Inititialse()
        {
            _engine = CreateEngine();
        }

        [TearDown]
        public void Dispose()
        {
            if (_engine != null) {
                _engine.Dispose();
                _engine = null;
            }
        }

        #endregion

        [Test]
        public void CanRenderResultsIntoTextFile()
        {
            var resultPath = TestResultRunFile(@"ResultRenderers\Text\phototest");
            using (var renderer = ResultRenderer.CreateTextRenderer(resultPath)) {
                var examplePixPath = this.TestFilePath("Ocr/phototest.tif");
                ProcessFile(renderer, examplePixPath);
            }
        }

        [Test]
        public void CanRenderResultsIntoPdfFile()
        {
            var resultPath = TestResultRunFile(@"ResultRenderers\PDF\phototest");
            using (var renderer = ResultRenderer.CreatePdfRenderer(resultPath, DataPath)) {
                var examplePixPath = this.TestFilePath("Ocr/phototest.tif");
                ProcessFile(renderer, examplePixPath);
            }
        }

        [Test]
        public void CanRenderMultiplePageDocumentToPdfFile()
        {
            var resultPath = TestResultRunFile(@"ResultRenderers\PDF\multi-page");
            using (var renderer = ResultRenderer.CreatePdfRenderer(resultPath, DataPath)) {
                var examplePixPath = this.TestFilePath("processing/multi-page.tif");
                ProcessMultipageTiff(renderer, examplePixPath);
            }
        }

        [Test]
        public void CanRenderResultsIntoHOcrFile()
        {
            var resultPath = TestResultRunFile(@"ResultRenderers\HOCR\phototest");
            using (var renderer = ResultRenderer.CreateHOcrRenderer(resultPath)) {
                var examplePixPath = this.TestFilePath("Ocr/phototest.tif");
                ProcessFile(renderer, examplePixPath);
            }
        }

        [Test]
        public void CanRenderResultsIntoUnlvFile()
        {
            var resultPath = TestResultRunFile(@"ResultRenderers\UNLV\phototest");
            using (var renderer = ResultRenderer.CreateUnlvRenderer(resultPath)) {
                var examplePixPath = this.TestFilePath("Ocr/phototest.tif");
                ProcessFile(renderer, examplePixPath);
            }
        }

        [Test]
        public void CanRenderResultsIntoBoxFile()
        {
            var resultPath = TestResultRunFile(@"ResultRenderers\Box\phototest");
            using (var renderer = ResultRenderer.CreateBoxRenderer(resultPath)) {
                var examplePixPath = this.TestFilePath("Ocr/phototest.tif");
                ProcessFile(renderer, examplePixPath);
            }
        }

        private void ProcessMultipageTiff(IResultRenderer renderer, string filename)
        {
            var imageName = Path.GetFileNameWithoutExtension(filename);
            using (var pixA = PixArray.LoadMultiPageTiffFromFile(filename)) {

                int expectedPageNumber = -1;
                using (renderer.BeginDocument(imageName)) {
                    Assert.AreEqual(renderer.PageNumber, expectedPageNumber);
                    foreach (var pix in pixA) {
                        using (var page = _engine.Process(pix, imageName)) {
                            renderer.AddPage(page);

                            Assert.AreEqual(renderer.PageNumber, ++expectedPageNumber);
                        }
                    }
                }

                Assert.AreEqual(renderer.PageNumber, expectedPageNumber);
            }
        }

        private void ProcessFile(IResultRenderer renderer, string filename)
        {
            var imageName = Path.GetFileNameWithoutExtension(filename);
            using (var pix = Pix.LoadFromFile(filename)) {
                using (renderer.BeginDocument(imageName)) {
                    Assert.AreEqual(renderer.PageNumber, -1);
                    using (var page = _engine.Process(pix, imageName)) {
                        renderer.AddPage(page);

                        Assert.AreEqual(renderer.PageNumber, 0);
                    }
                }

                Assert.AreEqual(renderer.PageNumber, 0);
            }
        }
    }
}
