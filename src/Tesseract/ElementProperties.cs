using System;
using System.Collections.Generic;
using System.Text;

namespace Tesseract
{
	/// <summary>
	/// Represents properties that describe a text block's orientation.
	/// </summary>
    public struct ElementProperties
    {
        private Orientation orientation;
        private TextLineOrder textLineOrder;
        private WritingDirection writingDirection;
        private float deskewAngle;

        public ElementProperties(Orientation orientation, TextLineOrder textLineOrder, WritingDirection writingDirection, float deskewAngle)
        {
            this.orientation = orientation;
            this.textLineOrder = textLineOrder;
            this.writingDirection = writingDirection;
            this.deskewAngle = deskewAngle;
        }

        /// <summary>
        /// Gets the <see cref="Orientation" /> for corresponding text block.
        /// </summary>
        public Orientation Orientation
        {
            get { return orientation; }
        }

        /// <summary>
        /// Gets the <see cref="TextLineOrder" /> for corresponding text block.
        /// </summary>
        public TextLineOrder TextLineOrder
        {
            get { return textLineOrder; }
        }

        /// <summary>
        /// Gets the <see cref="WritingDirection" /> for corresponding text block.
        /// </summary>
        public WritingDirection WritingDirection
        {
            get { return writingDirection; }
        }

        /// <summary>
        /// Gets the angle the page would need to be rotated to deskew the text block.
        /// </summary>
        public float DeskewAngle
        {
            get { return deskewAngle; }
        }
    }
}
