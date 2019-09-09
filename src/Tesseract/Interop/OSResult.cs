using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Tesseract.Interop
{
    [StructLayout(LayoutKind.Sequential)]
    public struct OSBestResult
    {
        public Orientation OrientationId;
        public int ScriptId;
        public float SConfidence;
        public float OConfidence;
    }

    /// <summary>
    ///
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct OSResult
    {
        public void Init()
        {
            fixed (float* orientationsPtr = _orientations) {
                fixed (float* scriptsNAPtr = _scriptsNA) {
                    for (int i = 0; i < 4; ++i) {
                        for (int j = 0; j < kMaxNumberOfScripts; ++j) {
                            scriptsNAPtr[i * kMaxNumberOfScripts + j] = 0f;
                        }
                        orientationsPtr[i] = 0f;
                    }
                }
            }
            _unicharset = IntPtr.Zero;
            _bestResult = new OSBestResult();
        }

        // Max number of scripts in ICU + "NULL" + Japanese and Korean + Fraktur
        private const int kMaxNumberOfScripts = 116 + 1 + 2 + 1;

        private fixed float _orientations[4];

        // Script confidence scores for each of 4 possible orientations.
        private fixed float _scriptsNA[4 * kMaxNumberOfScripts];

        private IntPtr _unicharset;
        private OSBestResult _bestResult;

        public void GetBestOrientation(out Orientation orientation, out float confidence)
        {
            Debug.Assert(((int)_bestResult.OrientationId >= 0) && ((int)_bestResult.OrientationId <= 3),
                "Best orientation must be between 0 and 3 but was " + _bestResult.OrientationId + ".");

            orientation = _bestResult.OrientationId;
            confidence = _bestResult.OConfidence;
        }
    }
}