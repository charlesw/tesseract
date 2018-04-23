using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Tesseract.Iterator
{
    internal class GenericResultSubIterator<T> : GenericResultIterator<T>, IEnumerator<T> where T : ResultBase
    {
        private readonly PageIteratorLevel ParentLevel;

        private bool isReset = true;
        private bool isNextable = true;


        public GenericResultSubIterator(ResultIterator mainIterator)
            : base(mainIterator._Handle)
        {
            ParentLevel = mainIterator.ElementLevel;
        }

        public override bool MoveNext()
        {
            if ((_Handle.Handle != IntPtr.Zero) && isNextable)
            {
                if (isReset)
                {
                    isReset = false;
                }
                else
                {
                    if (Interop.TessApi.Native.PageIteratorNext(_Handle, ElementLevel) == 0)
                        return false;
                }

                Current = CreateInstance();
                isNextable = Interop.TessApi.Native.PageIteratorIsAtFinalElement(_Handle, ParentLevel, ElementLevel) == 0;
                return true;
            }
            return false;
        }

        public override void Reset()
        {
            throw new NotImplementedException();
        }

        protected override void Dispose(bool disposing) { }
    }
}
