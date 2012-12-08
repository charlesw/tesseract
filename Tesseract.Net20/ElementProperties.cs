using System;
using System.Collections.Generic;
using System.Text;

namespace Tesseract
{
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

        public Orientation Orientation
        {
            get { return orientation; }
        }

        public TextLineOrder TextLineOrder
        {
            get { return textLineOrder; }
        }

        public WritingDirection WritingDirection
        {
            get { return writingDirection; }
        }

        public float DeskewAngle
        {
            get { return deskewAngle; }
        }
    }
}
