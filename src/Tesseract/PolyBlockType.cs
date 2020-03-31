using System;
using System.Collections.Generic;
using System.Text;

namespace Tesseract
{
    public enum PolyBlockType : int
    {
        /// <summary>
        /// The type is not known yet, keep as first element.
        /// </summary>
        Unknown,
        /// <summary>
        /// The text is inside a column.
        /// </summary>
        FlowingText,
        /// <summary>
        /// The text spans more than one column.
        /// </summary>
        HeadingText,
        /// <summary>
        /// The text is in a cross-column pull-out region.
        /// </summary>
        PullOutText,
        /// <summary>
        /// The partion belongs to an equation region..
        /// </summary>
        Equation,
        /// <summary>
        /// The partion has an inline equation.
        /// </summary>
        InlineEquation,
        /// <summary>
        /// The partion belongs to a Table region.
        /// </summary>
        Table,
        /// <summary>
        /// Text line runs vertically.
        /// </summary>
        VerticalText,
        /// <summary>
        /// Text that belongs to an image.
        /// </summary>
        CaptionText,
        /// <summary>
        /// Image that lives inside a column.
        /// </summary>
        FlowingImage, 
        /// <summary>
        /// Image that spans more than one column.
        /// </summary>
        HeadingImage,
        /// <summary>
        /// Image that is in a cross-column pull-out region.
        /// </summary>
        PullOutImage,
        HorizontalLine, 
        VerticalLine,
        /// <summary>
        /// Lies outside any column.
        /// </summary>
        Noise,
        Count
    }
}
