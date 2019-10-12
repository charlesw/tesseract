
using System;

namespace Tesseract
{
	
	public enum EngineMode : int
	{
        /// <summary>
        /// Only the legacy tesseract OCR engine is used.
        /// </summary>
        TesseractOnly = 0,

        /// <summary>
        /// Only the new LSTM-based OCR engine is used.
        /// </summary>
        LstmOnly,

        /// <summary>
        /// Both the legacy and new LSTM based OCR engine is used.
        /// </summary>
        TesseractAndLstm,

        /// <summary>
        /// The default OCR engine is used (currently LSTM-ased OCR engine).
        /// </summary>
        Default
	}
}
