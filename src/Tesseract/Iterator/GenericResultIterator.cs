using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Tesseract.Iterator
{
    internal class GenericResultIterator<T> : ResultIterator, IEnumerator<T> where T : ResultBase
    {
        private static PageIteratorLevel _GetLevel()
        {
            FieldInfo levelField = typeof(T).GetField("Level");
            Debug.Assert(levelField != null, "'" + typeof(T).Name + "' must have a 'PageIteratorLevel Level' static field, or constant field, to be used as 'T' type in GenericResultIterator<T>");
            return (PageIteratorLevel)levelField.GetValue(null);
        }
        protected static readonly PageIteratorLevel _ElementLevel = _GetLevel();
        public override PageIteratorLevel ElementLevel => _ElementLevel;
        private static readonly ConstructorInfo _CreateInstance = typeof(T).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[] { typeof(Iterator.ResultIterator) }, null);
        protected T CreateInstance() => (T)_CreateInstance.Invoke(_CreateParam);

        private bool _isReset = true;

        public T Current { get; protected set; }
        object IEnumerator.Current => Current;

        protected GenericResultIterator(HandleRef handle)
            : base(handle) { }
        public GenericResultIterator(TesseractEngine engine)
            : base(engine) { Reset(); }

        public virtual bool MoveNext()
        {
            VerifyNotDisposed();
            if (_Handle.Handle != IntPtr.Zero)
            {
                if (_isReset)
                {
                    _isReset = !_isReset;
                }
                else
                {
                    if (Interop.TessApi.Native.PageIteratorNext(_Handle, ElementLevel) == 0)
                        return false;
                }

                Current = CreateInstance();
                return true;
            }
            return false;
        }

        public virtual void Reset()
        {
            VerifyNotDisposed();
            _isReset = true;
            if (_Handle.Handle != IntPtr.Zero)
            {
                Interop.TessApi.Native.PageIteratorBegin(_Handle);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (_Handle.Handle != IntPtr.Zero)
            {
                Interop.TessApi.Native.PageIteratorDelete(_Handle);
            }
        }
    }
}