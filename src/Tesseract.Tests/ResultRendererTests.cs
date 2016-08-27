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
            using (var renderer = ResultRender.CreateTextRenderer(resultPath)) {
                var examplePixPath = this.TestFilePath("Ocr/phototest.tif");
                ProcessFile(renderer, examplePixPath);
            }
        }

        [Test]
        public void CanRenderResultsIntoPdfFile()
        {
            var resultPath = TestResultRunFile(@"ResultRenderers\PDF\phototest");
            using (var renderer = ResultRender.CreatePdfRenderer(resultPath, DataPath)) {
                var examplePixPath = this.TestFilePath("Ocr/phototest.tif");
                ProcessFile(renderer, examplePixPath);
            }
        }

        [Test]
        public void CanRenderResultsIntoHOcrFile()
        {
            var resultPath = TestResultRunFile(@"ResultRenderers\HOCR\phototest");
            using (var renderer = ResultRender.CreateHOcrRenderer(resultPath)) {
                var examplePixPath = this.TestFilePath("Ocr/phototest.tif");
                ProcessFile(renderer, examplePixPath);
            }
        }

        [Test]
        public void CanRenderResultsIntoUnlvFile()
        {
            var resultPath = TestResultRunFile(@"ResultRenderers\UNLV\phototest");
            using (var renderer = ResultRender.CreateUnlvRenderer(resultPath)) {
                var examplePixPath = this.TestFilePath("Ocr/phototest.tif");
                ProcessFile(renderer, examplePixPath);
            }
        }

        [Test]
        public void CanRenderResultsIntoBoxFile()
        {
            var resultPath = TestResultRunFile(@"ResultRenderers\Box\phototest");
            using (var renderer = ResultRender.CreateBoxRenderer(resultPath)) {
                var examplePixPath = this.TestFilePath("Ocr/phototest.tif");
                ProcessFile(renderer, examplePixPath);
            }
        }


        private void ProcessFile(IResultRenderer renderer, string filename)
        {
            var imageName = Path.GetFileNameWithoutExtension(filename);
            using (var pix = Pix.LoadFromFile(filename)) {
                using (renderer.BeginDocument(imageName)) {
                    using (var page = _engine.Process(pix, imageName)) {
                        renderer.AddPage(page);
                    }
                }
            }
        }
    }
}
