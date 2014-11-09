using InteropDotNet;

namespace Tesseract
{
    /// <summary>
    /// Manages the tesseract OCR engine environment.
    /// </summary>
    public static class TesseractEnvironment
    {
        /// <summary>
        /// Unloads native libraries.
        /// </summary>
        public static void Unload()
        {
            LibraryLoader.Instance.FreeAllLibraries();
        }
    }
}
