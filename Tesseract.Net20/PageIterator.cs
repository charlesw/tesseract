using System;
using System.Collections.Generic;
using System.Text;

namespace Tesseract
{
    /// <summary>
    /// Represents an object that can iterate over tesseract's page structure.
    /// </summary>
    /// <remarks>
    /// The iterator points to tesseract's internal page structure and is only valid while the Engine instance that created it exists
    /// and has not been subjected to a call to Recognize since the iterator was created.
    /// </remarks>
    public class PageIterator : DisposableBase
    {
        protected readonly IntPtr handle;

        internal PageIterator(IntPtr handle)
        {
            this.handle = handle;
        }

        /// <summary>
        /// Moves the iterator to the start of the page.
        /// </summary>
        public void Begin()
        {
            Interop.TessApi.PageIteratorBegin(handle);
        }

        /// <summary>
        /// Moves to the start of the next element at the given level.
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <param name="level"></param>
        /// <returns></returns>
        public bool Next(PageIteratorLevel level)
        {
            return Interop.TessApi.PageIteratorNext(handle, level) != 0;
        }

        /// <summary>
        /// Moves the iterator to the next <paramref name="element"/> iff the iterator is not currently pointing to the last <paramref name="element"/> in the specified <paramref name="level"/> (i.e. the last word in the paragraph).
        /// </summary>
        /// <param name="level">The iterator level.</param>
        /// <param name="element">The page level.</param>
        /// <returns><c>True</c> iff there is another <paramref name="element"/> to advance too and the current element is not the last element at the given level; otherwise returns <c>False</c>.</returns>
        public bool Next(PageIteratorLevel level, PageIteratorLevel element)
        {
            var isAtFinalElement = IsAtFinalOf(level, element);
            if (!isAtFinalElement) {
                return Next(element);
            } else {
                return false;
            }
        }

        /// <summary>
        /// Returns <c>True</c> if the iterator is at the first element at the given level.
        /// </summary>
        /// <remarks>
        /// A possible use is to determin if a call to next(word) moved to the start of a new paragraph.
        /// </remarks>
        /// <param name="level"></param>
        /// <returns></returns>
        public bool IsAtBeginningOf(PageIteratorLevel level)
        {
            return Interop.TessApi.PageIteratorIsAtBeginningOf(handle, level) != 0;
        }

        /// <summary>
        /// Returns <c>True</c> if the iterator is possitioned at the last element at the given level.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        public bool IsAtFinalOf(PageIteratorLevel level, PageIteratorLevel element)
        {
            return Interop.TessApi.PageIteratorIsAtFinalElement(handle, level, element) != 0;
        }

        public PolyBlockType BlockType
        {
            get { return Interop.TessApi.PageIteratorBlockType(handle); }
        }

        public Pix GetBinaryImage(PageIteratorLevel level)
        {
            return Pix.Create(Interop.TessApi.PageIteratorGetBinaryImage(handle, level));
        }

        public Pix GetImage(PageIteratorLevel level, int padding, out int x, out int y)
        {
            return Pix.Create(Interop.TessApi.PageIteratorGetImage(handle, level, padding, out x, out y));
        }

        /// <summary>
        /// Gets the bounding rectangle of the current element at the given level. 
        /// </summary>
        /// <param name="level"></param>
        /// <param name="bounds"></param>
        /// <returns></returns>
        public bool TryGetBoundingBox(PageIteratorLevel level, out Rect bounds)
        {
            int x1, y1, x2, y2;
            if (Interop.TessApi.PageIteratorBoundingBox(handle, level, out x1, out y1, out x2, out y2) != 0) {
                bounds = Rect.FromCoords(x1, y1, x2, y2);
                return true;
            } else {
                bounds = Rect.Empty;
                return false;
            }
        }

        /// <summary>
        /// Gets the baseline of the current element at the given level.
        /// </summary>
        /// <remarks>
        /// The baseline is the line that passes through (x1, y1) and (x2, y2).
        /// WARNING: with vertical text, baselines may be vertical! Returns false if there is no baseline at the current position.</remarks>
        /// <param name="level"></param>
        /// <param name="bounds"></param>
        /// <returns></returns>
        public bool TryGetBaseline(PageIteratorLevel level, out Rect bounds)
        {
            int x1, y1, x2, y2;
            if (Interop.TessApi.PageIteratorBaseline(handle, level, out x1, out y1, out x2, out y2) != 0) {
                bounds = Rect.FromCoords(x1, y1, x2, y2);
                return true;
            } else {
                bounds = Rect.Empty;
                return false;
            }
        }

        /// <summary>
        /// Gets the element orientation information that the iterator currently points too.
        /// </summary>
        public ElementProperties GetProperties()
        {
            Orientation orientation;
            WritingDirection writing_direction;
            TextLineOrder textLineOrder;
            float deskew_angle;
            Interop.TessApi.PageIteratorOrientation(handle, out orientation, out writing_direction, out textLineOrder, out deskew_angle);

            return new ElementProperties(orientation, textLineOrder, writing_direction, deskew_angle);
        }


        protected override void Dispose(bool disposing)
        {
            Interop.TessApi.PageIteratorDelete(handle);
        }
    }
}
