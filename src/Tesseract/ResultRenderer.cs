using System;
using System.Runtime.InteropServices;

namespace Tesseract
{
    /// <summary>
    /// Class to create result renderer.
    /// </summary>
    public sealed class ResultRenderer : DisposableBase
    {
        private readonly HandleRef _handleRef;

        internal ResultRenderer(IntPtr handle)
        {
            this._handleRef = new HandleRef(this, handle);
        }

        /// <summary>
        /// Renders tesseract output into a plain UTF-8 text string.
        /// </summary>
        /// <param name="outputbase"></param>
        /// <returns></returns>
        public ResultRenderer TextRendererCreate(String outputbase)
        {
            VerifyNotDisposed();
            if (_handleRef.Handle == IntPtr.Zero)
                return null;

            return Interop.TessApi.Native.TextRendererCreate(outputbase);
        }

        /// <summary>
        /// Renders tesseract output into an hocr text string.
        /// </summary>
        /// <param name="outputbase"></param>
        /// <returns></returns>
        public ResultRenderer HOcrRendererCreate(String outputbase)
        {
            VerifyNotDisposed();
            if (_handleRef.Handle == IntPtr.Zero)
                return null;

            return Interop.TessApi.Native.HOcrRendererCreate(outputbase);
        }

        /// <summary>
        /// Renders tesseract output into an hocr text string.
        /// </summary>
        /// <param name="outputbase"></param>
        /// <param name="font_info"></param>
        /// <returns></returns>
        public ResultRenderer HOcrRendererCreate2(String outputbase, bool font_info)
        {
            VerifyNotDisposed();
            if (_handleRef.Handle == IntPtr.Zero)
                return null;

            return Interop.TessApi.Native.HOcrRendererCreate2(outputbase, font_info ? 1 : 0);
        }

        /// <summary>
        /// Renders tesseract output into searchable PDF.
        /// </summary>
        /// <param name="outputbase"></param>
        /// <param name="datadir"></param>
        /// <returns></returns>
        public ResultRenderer PDFRendererCreate(string outputbase, string datadir)
        {
            VerifyNotDisposed();
            if (_handleRef.Handle == IntPtr.Zero)
                return null;

            return Interop.TessApi.Native.PDFRendererCreate(outputbase, datadir);
        }

        /// <summary>
        /// Renders tesseract output into a plain UTF-8 text string in UNLV format.
        /// </summary>
        /// <param name="outputbase"></param>
        /// <returns></returns>
        public ResultRenderer UnlvRendererCreate(String outputbase)
        {
            VerifyNotDisposed();
            if (_handleRef.Handle == IntPtr.Zero)
                return null;

            return Interop.TessApi.Native.UnlvRendererCreate(outputbase);
        }

        /// <summary>
        /// Renders tesseract output into a plain UTF-8 text string in Box format.
        /// </summary>
        /// <param name="outputbase"></param>
        /// <returns></returns>
        public ResultRenderer BoxTextRendererCreate(String outputbase)
        {
            VerifyNotDisposed();
            if (_handleRef.Handle == IntPtr.Zero)
                return null;

            return Interop.TessApi.Native.BoxTextRendererCreate(outputbase);
        }

        /// <summary>
        /// Returns the next renderer or NULL.
        /// </summary>
        /// <returns></returns>
        public ResultRenderer Next()
        {
            VerifyNotDisposed();
            if (_handleRef.Handle == IntPtr.Zero)
                return null;
            return Interop.TessApi.Native.ResultRendererNext(this);
        }

        /// <summary>
        /// Inserts the next renderer.
        /// </summary>
        /// <returns>string</returns>
        public void Insert(ResultRenderer next)
        {
            VerifyNotDisposed();
            if (_handleRef.Handle == IntPtr.Zero)            
                return;

            Interop.TessApi.Native.ResultRendererInsert(this, next);
        }

        protected override void Dispose(bool disposing)
        {
            if (_handleRef.Handle != IntPtr.Zero)
            {
                Interop.TessApi.Native.DeleteResultRenderer(this);
            }
        }
    }
}