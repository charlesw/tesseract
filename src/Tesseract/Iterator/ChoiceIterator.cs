using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Tesseract.Iterator
{
    /// <summary>
    /// Class to iterate over the classifier choices for a single symbol.
    /// </summary>
    internal sealed class ChoiceIterator : DisposableBase, IEnumerator<Choice>, IEnumerable<Choice>
    {
        private HandleRef _handleRef;
        private readonly HandleRef _resultIterator;

        public Choice Current { get; private set; }
        object IEnumerator.Current => Current;

        internal ChoiceIterator(HandleRef resultIterator)
        {
            _resultIterator = resultIterator;
            Reset();
        }

        protected override void Dispose(bool disposing)
        {
            if (_handleRef.Handle != IntPtr.Zero)
            {
                Interop.TessApi.Native.ChoiceIteratorDelete(_handleRef);
                _handleRef = new HandleRef();
            }
        }

        public bool MoveNext()
        {
            VerifyNotDisposed();
            if ((_handleRef.Handle != IntPtr.Zero) &&
                (Interop.TessApi.Native.ChoiceIteratorNext(_handleRef) != 0))
            {
                Current = new Choice(
                    Interop.TessApi.ChoiceIteratorGetUTF8Text(_handleRef),
                    Interop.TessApi.Native.ChoiceIteratorGetConfidence(_handleRef));
                return true;
            }

            return false;
        }

        public void Reset()
        {
            if (_resultIterator.Handle != IntPtr.Zero)
            {
                IntPtr choiceIteratorHandle = Interop.TessApi.Native.ResultIteratorGetChoiceIterator(_resultIterator);
                if (choiceIteratorHandle != IntPtr.Zero)
                    _handleRef = new HandleRef(this, choiceIteratorHandle);
            }
        }

        public IEnumerator<Choice> GetEnumerator() => this;
        IEnumerator IEnumerable.GetEnumerator() => this;
    }
}