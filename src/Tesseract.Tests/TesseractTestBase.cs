using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tesseract.Tests
{
    public abstract class TesseractTestBase
    {
        protected TesseractEngine CreateEngine(string lang = "eng", EngineMode mode = EngineMode.Default)
        {
            var datapath = DataPath;
            return new TesseractEngine(datapath, lang, mode);
        }

        protected string DataPath
        {
            get {  return Path.Combine(Environment.CurrentDirectory, "tessdata"); }
        }

        #region File Helpers

        protected string TestFilePath(string path)
        {
            var basePath = Path.Combine(TestContext.CurrentContext.WorkDirectory, "Data");

            return Path.Combine(basePath, path);
        }

        protected string TestResultPath(string path)
        {
            var basePath = Path.Combine(TestContext.CurrentContext.WorkDirectory, "Results");

            return Path.Combine(basePath, path);
        }

        protected string TestResultRunDirectory(string path)
        {
            var runPath = Path.Combine(
                TestContext.CurrentContext.WorkDirectory, 
                String.Format("Runs/{0:yyyyMMddTHHmmss}", TestRun.Current.StartedAt)
            );
            var testResultRunDirectory = Path.Combine(runPath, path);        
            Directory.CreateDirectory(testResultRunDirectory);

            return testResultRunDirectory;
        }
        
        protected string TestResultRunFile(string path)
        {
            var testRunDirectory = TestResultRunDirectory(Path.GetDirectoryName(path));
            var testFileName = Path.GetFileName(path);

            return Path.Combine(testRunDirectory, testFileName);
        }

        protected Pix LoadTestPix(string filename)
        {
            var testFilename = TestFilePath(filename);
            return Pix.LoadFromFile(testFilename);
        }

        /// <summary>
        /// Normalise new line characters to unix (\n) so they are all the same.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        protected string NormaliseNewLine(string text)
        {
            return text
                .Replace("\r\n", "\n")
                .Replace("\r", "\n");
        }

        #endregion File Helpers
    }
}
