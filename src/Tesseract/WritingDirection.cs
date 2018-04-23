using System;
using System.Collections.Generic;
using System.Text;

namespace Tesseract
{
    /// <summary>
    /// The grapheme cluster within a line of text are laid out logically in this direction,
    /// judged when looking at the text line rotated so that Orientation is "page up".
    /// </summary>
    public enum WritingDirection : int
    {
        /// <summary>
        /// The text line from the left hand side to the right hand side when the page is rotated so it's orientation is <see cref="Orientation.PageUp" />.
        /// </summary>
        LeftToRight = 0,

        /// <summary>
        /// The text line from the right hand side to the left hand side when the page is rotated so it's orientation is <see cref="Orientation.PageUp" />.
        /// </summary>
        RightToLeft = 1,

        /// <summary>
        /// The text line from the top to the bottom of the page when the page is rotated so it's orientation is <see cref="Orientation.PageUp" />.
        /// </summary>
        TopToBottom = 2
    }
}