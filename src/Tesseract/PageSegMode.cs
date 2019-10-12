using System;
using System.Collections.Generic;
using System.Text;

namespace Tesseract
{
    /// <summary>
    /// Represents the possible page layou analysis modes.
    /// </summary>
    public enum PageSegMode : int
    {
        /// <summary>
        /// Orientation and script detection (OSD) only.
        /// </summary>
        OsdOnly = 0,

        /// <summary>
        /// Automatic page sementation with orientantion and script detection (OSD).
        /// </summary>
        AutoOsd = 1,

        /// <summary>
        /// Automatic page segmentation, but no OSD, or OCR.
        /// </summary>
        AutoOnly = 2,

        /// <summary>
        /// Fully automatic page segmentation, but no OSD.
        /// </summary>
        Auto = 3,

        /// <summary>
        /// Assume a single column of text of variable sizes.
        /// </summary>
        SingleColumn = 4,

        /// <summary>
        /// Assume a single uniform block of vertically aligned text.
        /// </summary>
        SingleBlockVertText = 5,

        /// <summary>
        /// Assume a single uniform block of text.
        /// </summary>
        SingleBlock = 6,

        /// <summary>
        /// Treat the image as a single text line.
        /// </summary>
        SingleLine = 7,

        /// <summary>
        /// Treat the image as a single word.
        /// </summary>
        SingleWord = 8,

        /// <summary>
        /// Treat the image as a single word in a circle.
        /// </summary>
        CircleWord = 9,

        /// <summary>
        /// Treat the image as a single character.
        /// </summary>
        SingleChar = 10,

        /// <summary>
        SparseText = 11,

        /// <summary>
        /// Sparse text with orientation and script detection.
        /// </summary>
        SparseTextOsd = 12,
        
        /// <summary>
        /// Treat the image as a single text line, bypassing hacks that are
        /// specific to Tesseract.
        /// </summary>
        RawLine = 13,

        /// <summary>        
        /// Number of enum entries.
        /// </summary>
        Count
    }
}
