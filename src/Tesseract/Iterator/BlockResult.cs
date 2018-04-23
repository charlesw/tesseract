using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Tesseract
{
    public class BlockResult : ResultBase, IEnumerable<ParaResult>
    {
        public const PageIteratorLevel Level = PageIteratorLevel.Block;
        public readonly PolyBlockType BlockType;

        internal BlockResult(Iterator.ResultIterator iterator) : base(iterator)
        {
            if (Iterator._Handle.Handle == IntPtr.Zero)
                BlockType = PolyBlockType.Unknown;
            else
                BlockType = Interop.TessApi.Native.PageIteratorBlockType(Iterator._Handle);
        }

        public IEnumerator<ParaResult> GetEnumerator() => new Iterator.GenericResultSubIterator<ParaResult>(Iterator);
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerator<ParaResult>)this);
    }
}