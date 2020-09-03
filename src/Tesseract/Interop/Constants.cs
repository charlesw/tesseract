﻿using System;

namespace Tesseract.Interop
{
    /// <summary>
    /// Description of Constants.
    /// </summary>
    internal static class Constants
    {
        public const string LeptonicaDllName = "org.sw.demo.danbloomberg.leptonica-1.80.0.dll";
        public const string TesseractDllName = "tesseract50.dll";
        
        // tesseract uses an int to represent true false values.
        public const int TRUE = 1;
        public const int FALSE = 0;
    }
}