using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Tesseract
{
    public abstract class ResultBase
    {
        internal readonly Iterator.ResultIterator Iterator;
        public readonly float Confidence;
        public readonly string Text;

        public readonly Pix BinaryImage;

        /// <summary>
        /// Gets the bounding rectangle of the element. 
        /// </summary>
        public readonly Rect BoundingBox;

        /// <summary>
        /// Gets the baseline of the element.
        /// </summary>
        /// <remarks>
        /// The baseline is the line that passes through (x1, y1) and (x2, y2).
        /// WARNING: with vertical text, baselines may be vertical! Returns false if there is no baseline at the current position.</remarks>
        public readonly Rect Baseline;

        /// <summary>
        /// Gets the element orientation information that the iterator currently points too.
        /// </summary>
        public readonly ElementProperties Properties;

        internal ResultBase(Iterator.ResultIterator iterator)
        {
            Iterator = iterator;
            if (Iterator._Handle.Handle != IntPtr.Zero)
            {
                Confidence = Interop.TessApi.Native.ResultIteratorGetConfidence(Iterator._Handle, iterator.ElementLevel);
                Text = Interop.TessApi.ResultIteratorGetUTF8Text(Iterator._Handle, iterator.ElementLevel);
                BinaryImage = Pix.Create(Interop.TessApi.Native.PageIteratorGetBinaryImage(Iterator._Handle, iterator.ElementLevel));
                int x1 = -1, y1 = -1, x2 = -1, y2 = -1;
                if (Interop.TessApi.Native.PageIteratorBoundingBox(Iterator._Handle, iterator.ElementLevel, out x1, out y1, out x2, out y2) != 0)
                    BoundingBox = Rect.FromCoords(x1, y1, x2, y2);
                if (Interop.TessApi.Native.PageIteratorBaseline(Iterator._Handle, iterator.ElementLevel, out x1, out y1, out x2, out y2) != 0)
                    Baseline = Rect.FromCoords(x1, y1, x2, y2);
            }
            else
            {
                Confidence = 0;
                Text = string.Empty;
                BinaryImage = null;
                BoundingBox = Rect.Empty;
                Baseline = BoundingBox;
            }
        }
    }
}