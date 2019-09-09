using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Tesseract
{
    public class TextLineResult : ResultBase, IEnumerable<WordResult>
    {
        public IEnumerator<WordResult> Words => GetEnumerator();
        public IEnumerator<SymbolResult> Symbols => new Iterator.GenericResultSubIterator<SymbolResult>(Iterator);

        public const PageIteratorLevel Level = PageIteratorLevel.TextLine;
        internal TextLineResult(Iterator.ResultIterator iterator)
            : base(iterator) { }

        public IEnumerator<WordResult> GetEnumerator() => new Iterator.GenericResultSubIterator<WordResult>(Iterator);
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerator<WordResult>)this);
    }
}