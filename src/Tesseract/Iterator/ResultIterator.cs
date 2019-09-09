using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Tesseract.Iterator
{
    internal abstract class ResultIterator : DisposableBase
    {
        public abstract PageIteratorLevel ElementLevel { get; }
        internal readonly HandleRef _Handle;
        protected readonly object[] _CreateParam;

        private ResultIterator()
            => _CreateParam = new object[] { this };
        protected ResultIterator(HandleRef handleRef) : this()
            => _Handle = handleRef;
        protected ResultIterator(TesseractEngine engine) : this()
            => _Handle = new HandleRef(this, Interop.TessApi.Native.BaseApiGetIterator(engine.Handle));
    }
}
