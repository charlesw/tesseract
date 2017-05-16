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
        OsdOnly,

        /// <summary>
        /// Automatic page sementation with orientantion and script detection (OSD).
        /// </summary>
        AutoOsd,

        /// <summary>
        /// Automatic page segmentation, but no OSD, or OCR.
        /// </summary>
        AutoOnly,

        /// <summary>
        /// Fully automatic page segmentation, but no OSD.
        /// </summary>
        Auto,

        /// <summary>
        /// Assume a single column of text of variable sizes.
        /// </summary>
        SingleColumn,

        /// <summary>
        /// Assume a single uniform block of vertically aligned text.
        /// </summary>
        SingleBlockVertText,

        /// <summary>
        /// Assume a single uniform block of text.
        /// </summary>
        SingleBlock,

        /// <summary>
        /// Treat the image as a single text line.
        /// </summary>
        SingleLine,

        /// <summary>
        /// Treat the image as a single word.
        /// </summary>
        SingleWord,

        /// <summary>
        /// Treat the image as a single word in a circle.
        /// </summary>
        CircleWord,

        /// <summary>
        /// Treat the image as a single character.
        /// </summary>
        SingleChar,

        /// <summary>
        /// Find as much text as possible in no particular order.
        /// </summary>
        SparseText,

        /// <summary>
        /// Sparse text with orientation and script detection.
        /// </summary>
        SparseTextOsd,

        /// <summary>
        /// Treat the image as a single text line, bypassing hacks that are Tesseract-specific.
        /// </summary>
        RawLine,

        /// <summary>        
        /// Number of enum entries.
        /// </summary>
        Count
    }
}
