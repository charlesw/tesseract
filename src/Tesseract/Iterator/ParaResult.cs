using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Tesseract
{
    public class ParaResult : ResultBase, IEnumerable<TextLineResult>
    {
        public const PageIteratorLevel Level = PageIteratorLevel.Para;
        internal ParaResult(Iterator.ResultIterator iterator)
            : base(iterator) { }

        public IEnumerator<TextLineResult> GetEnumerator() => new Iterator.GenericResultSubIterator<TextLineResult>(Iterator);
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerator<TextLineResult>)this);
    }
}
