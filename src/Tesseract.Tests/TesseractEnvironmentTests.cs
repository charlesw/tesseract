using System.Collections.Generic;
using System.IO;
using System.Linq;
using InteropDotNet;
using NUnit.Framework;
using Tesseract.Interop;

namespace Tesseract.Tests
{
    [TestFixture]
    public class TesseractEnvironmentTests
    {
        private static IEnumerable<string> GetNativeLibraries()
        {
            string platformName = SystemManager.GetPlatformName();
            var namesWithoutExt = new List<string> {Constants.TesseractDllName, Constants.LeptonicaDllName};
            var shortNames = namesWithoutExt.Select(nameWithoutExt => string.Concat(nameWithoutExt, ".dll"));
            var relativeNames = shortNames.Select(shortName => Path.Combine(platformName, shortName));
            return relativeNames;
        }

        /// <summary>
        /// We can check the error codes like this http://stackoverflow.com/a/3202085/608971
        /// but it won't work on Linux.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool IsFileLocked(string filePath)
        {
            try
            {
                using (File.Open(filePath, FileMode.Open)) {}
                return false;
            }
            catch (IOException e)
            {
                if (e is DriveNotFoundException)
                    return false;
                if (e is DirectoryNotFoundException)
                    return false;
                if (e is FileNotFoundException)
                    return false;

                return true;
            }
        }

        [Test]
        public void Unload_ShouldFreeNativeLibraries()
        {
            // arrange
            // we create the tesseract engine wrapper which should load the native libraries
            using (new TesseractEngine("./tessdata", "eng", EngineMode.Default)) { }

            // act
            TesseractEnvironment.Unload();

            // assert
            List<string> lockedLibraries = new List<string>();
            foreach (var nativeLibrary in GetNativeLibraries())
            {
                if (IsFileLocked(nativeLibrary))
                {
                    lockedLibraries.Add(nativeLibrary);
                }
            }

            // this assert also shows us the contents of the list
            Assert.That(lockedLibraries, Is.EquivalentTo(new List<string>()));
        }

        [Test]
        public void Unload_ShouldReloadNativeLibrariesOnNextUse()
        {
            // Arrange:
            // 1. we create the tesseract engine wrapper which should load the native libraries
            using (new TesseractEngine("./tessdata", "eng", EngineMode.Default)) { }

            // 2. Now we unload the native libraries
            TesseractEnvironment.Unload();

            // Act: Create another tesseract engine instance which should reload the native libraries
            using (new TesseractEngine("./tessdata", "eng", EngineMode.Default)) { }
     
        }
    }
}
