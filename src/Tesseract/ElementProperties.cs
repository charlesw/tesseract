using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Tesseract
{
    /// <summary>
    /// Represents properties that describe a text block's orientation.
    /// </summary>
    public struct ElementProperties
    {
        /// <summary>
        /// Gets the <see cref="Orientation" /> for corresponding text block.
        /// </summary>
        public readonly Orientation Orientation;

        /// <summary>
        /// Gets the <see cref="TextLineOrder" /> for corresponding text block.
        /// </summary>
        public readonly TextLineOrder TextLineOrder;

        /// <summary>
        /// Gets the <see cref="WritingDirection" /> for corresponding text block.
        /// </summary>
        public readonly WritingDirection WritingDirection;

        /// <summary>
        /// Gets the angle the page would need to be rotated to deskew the text block.
        /// </summary>
        public readonly float DeskewAngle;

        public ElementProperties(HandleRef handle)
        {
            if (handle.Handle == IntPtr.Zero)
            {
                Orientation = Orientation.PageUp;
                TextLineOrder = TextLineOrder.TopToBottom;
                WritingDirection = WritingDirection.LeftToRight;
                DeskewAngle = 0;
            }
            else
            {
                Interop.TessApi.Native.PageIteratorOrientation(handle, out Orientation, out WritingDirection, out TextLineOrder, out DeskewAngle);
            }
        }
    }
}