using System;
using System.Runtime.InteropServices;

namespace Tesseract
{
    /// <summary>
    /// Class to iterate over the classifier choices for a single symbol.
    /// </summary>
    public sealed class ChoiceIterator : DisposableBase
    {
        private readonly HandleRef _handleRef;
        
        internal ChoiceIterator(IntPtr handle)
        {
            this._handleRef = new HandleRef(this, handle);
        }

        /// <summary>
        /// Moves to the next choice for the symbol and returns false if there are none left.
        /// </summary>
        /// <returns>true|false</returns>
        public bool Next()
        {
            VerifyNotDisposed();
            if (_handleRef.Handle == IntPtr.Zero)
                return false;
            return Interop.TessApi.Native.ChoiceIteratorNext(_handleRef) != 0;
        }

        /// <summary>
        /// Returns the confidence of the current choice.        
        /// </summary>
        /// <remarks>
        /// The number should be interpreted as a percent probability. (0.0f-100.0f)
        /// </remarks>
        /// <returns>float</returns>
        public float GetConfidence()
        {
            VerifyNotDisposed();
            if (_handleRef.Handle == IntPtr.Zero)
                return 0f;

            return Interop.TessApi.Native.ChoiceIteratorGetConfidence(_handleRef);
        }

        /// <summary>
        /// Returns the text string for the current choice.
        /// </summary>
        /// <returns>string</returns>
        public string GetText()
        {
            VerifyNotDisposed();
            if (_handleRef.Handle == IntPtr.Zero)            
                return String.Empty;
            
            return Interop.TessApi.ChoiceIteratorGetUTF8Text(_handleRef);
        }

        protected override void Dispose(bool disposing)
        {
            if (_handleRef.Handle != IntPtr.Zero)
            {
                Interop.TessApi.Native.ChoiceIteratorDelete(_handleRef);
            }
        }
    }
}