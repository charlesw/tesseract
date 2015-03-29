using InteropDotNet;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tesseract
{
    public static class TesseractEnviornment
    {
        /// <summary>
        /// Gets or sets a search path that will be checked first when attempting to load the Tesseract and Leptonica dlls.
        /// </summary>
        /// <remarks>
        /// This search path should not include the platform component as this will automatically be appended to the string based on the detected platform.
        /// </remarks>
        public static string CustomSearchPath
        {
            get { return LibraryLoader.Instance.CustomSearchPath; }
            set { LibraryLoader.Instance.CustomSearchPath = value; }
        }
    }
}
