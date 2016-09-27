using System;
using System.Runtime.InteropServices;

namespace Tesseract
{
    /// <summary>
    /// Class to create result renderer.
    /// </summary>
    public abstract class ResultRenderer : DisposableBase
    {
        internal HandleRef Handle;

        public string OutputBase;       

        /// <summary>
        /// Process the pages of the image using the selected renderer
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="imagePath"></param>
        /// <param name="retryConfig"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public bool ProcessPages(TesseractEngine engine, string imagePath, string retryConfig = null, int timeout = 0)
        {
            bool result = Interop.TessApi.Native.BaseAPIProcessPages(engine.Handle, imagePath, null, 0, Handle);
            Next();

            return result;
        }
        

        /// <summary>
        /// Returns the next renderer or NULL.
        /// </summary>
        /// <returns></returns>
        public IntPtr Next()
        {
            VerifyNotDisposed();
            if (Handle.Handle == IntPtr.Zero)
                return IntPtr.Zero;

            Handle = new HandleRef(this,Interop.TessApi.Native.ResultRendererNext(Handle));

            return Handle.Handle;
        }

        /// <summary>
        /// Inserts the next renderer.
        /// </summary>
        /// <returns>string</returns>
        public void Insert(ResultRenderer next)
        {
            VerifyNotDisposed();
            if (Handle.Handle == IntPtr.Zero)            
                return;

            Interop.TessApi.Native.ResultRendererInsert(Handle, next.Handle);
        }

        protected override void Dispose(bool disposing)
        {
            if (Handle.Handle != IntPtr.Zero)
            {
                Interop.TessApi.Native.DeleteResultRenderer(new HandleRef(this,Handle.Handle));
            }
        }
    }
}