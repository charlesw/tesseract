using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Tesseract
{
    public class ParaResult : ResultBase, IEnumerable<TextLineResult>
    {
        public IEnumerator<TextLineResult> TextLines => GetEnumerator();
        public IEnumerator<WordResult> Words => new Iterator.GenericResultSubIterator<WordResult>(Iterator);
        public IEnumerator<SymbolResult> Symbols => new Iterator.GenericResultSubIterator<SymbolResult>(Iterator);

        public const PageIteratorLevel Level = PageIteratorLevel.Para;
        internal ParaResult(Iterator.ResultIterator iterator)
            : base(iterator) { }

        public IEnumerator<TextLineResult> GetEnumerator() => new Iterator.GenericResultSubIterator<TextLineResult>(Iterator);
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerator<TextLineResult>)this);
    }
}
