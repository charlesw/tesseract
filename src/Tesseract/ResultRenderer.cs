using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Tesseract.Internal;

namespace Tesseract
{
    /// <summary>
    /// Represents a native result renderer (e.g. text, pdf, etc).
    /// </summary>
    /// <remarks>
    /// Note that the ResultRenderer is explictly responsible for managing the
    /// renderer hierarchy. This gets around a number of difficult issues such
    /// as keeping track of what the next renderer is and how to manage the memory.
    /// </remarks>
    public class ResultRenderer : DisposableBase, IResultRenderer
    {
        #region Factory Methods

        /// <summary>
        /// Creates a <see cref="IResultRenderer">result renderer</see> that render that generates a searchable
        /// pdf file from tesseract's output.
        /// </summary>
        /// <param name="outputFilename">The filename of the pdf file to be generated without the file extension.</param>
        /// <param name="fontDirectory">The directory containing the pdf font data, normally same as your tessdata directory.</param>
        /// <param name="next">The next renderer.</param>
        /// <returns></returns>
        public static IResultRenderer CreatePdfRenderer(string outputFilename, string fontDirectory)
        {
            var rendererHandle = Interop.TessApi.Native.PDFRendererCreate(outputFilename, fontDirectory);

            return new ResultRenderer(rendererHandle);
        }

        /// <summary>
        /// Creates a <see cref="IResultRenderer">result renderer</see> that render that generates UTF-8 encoded text
        /// file from tesseract's output.
        /// </summary>
        /// <param name="outputFilename">The path to the text file to be generated without the file extension.</param>
        /// <returns></returns>
        public static IResultRenderer CreateTextRenderer(string outputFilename)
        {
            var rendererHandle = Interop.TessApi.Native.TextRendererCreate(outputFilename);

            return new ResultRenderer(rendererHandle);
        }


        /// <summary>
        /// Creates a <see cref="IResultRenderer">result renderer</see> that render that generates a HOCR
        /// file from tesseract's output.
        /// </summary>
        /// <param name="outputFilename">The path to the hocr file to be generated without the file extension.</param>
        /// <param name="fontInfo"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public static IResultRenderer CreateHOcrRenderer(string outputFilename, bool fontInfo = false)
        {
            var rendererHandle = Interop.TessApi.Native.HOcrRendererCreate2(outputFilename, fontInfo ? 1 : 0);

            return new ResultRenderer(rendererHandle);
        }


        /// <summary>
        /// Creates a <see cref="IResultRenderer">result renderer</see> that render that generates a unlv
        /// file from tesseract's output.
        /// </summary>
        /// <param name="outputFilename">The path to the unlv file to be created without the file extension.</param>
        /// <returns></returns>
        public static IResultRenderer CreateUnlvRenderer(string outputFilename)
        {
            var rendererHandle = Interop.TessApi.Native.UnlvRendererCreate(outputFilename);

            return new ResultRenderer(rendererHandle);
        }
        
        /// <summary>
        /// Creates a <see cref="IResultRenderer">result renderer</see> that render that generates a box text file from tesseract's output.
        /// </summary>
        /// <param name="outputFilename">The path to the box file to be created without the file extension.</param>
        /// <returns></returns>
        public static IResultRenderer CreateBoxRenderer(string outputFilename)
        {
            var rendererHandle = Interop.TessApi.Native.BoxTextRendererCreate(outputFilename);

            return new ResultRenderer(rendererHandle);
        }
       
        #endregion

        /// <summary>
        /// Ensures the renderer's EndDocument when disposed off.
        /// </summary>
        private class EndDocumentOnDispose : IDisposable
        {
            readonly ResultRenderer _renderer;

            public EndDocumentOnDispose(ResultRenderer renderer)
            {
                _renderer = renderer;
            }

            public void Dispose()
            {
                // End the renderer
                Interop.TessApi.Native.ResultRendererEndDocument(_renderer._handle);
            }
        }
        
        private HandleRef _handle;

        protected ResultRenderer(IntPtr handle)
        {
            Guard.Require("handle", handle != IntPtr.Zero, "handle must be initialised.");

            _handle = new HandleRef(this, handle);
        }
                
        /// <summary>
        /// Add the page to the current document.
        /// </summary>
        /// <param name="page"></param>
        public void AddPage(Page page)
        {
            Guard.RequireNotNull("page", page);
            VerifyNotDisposed();

            // TODO: Force page to do a recognise run to ensure the underlying base api is full of state note if
            // your implementing your own renderer you won't need to do this since all the page operations will do it
            // implicitly if required. This is why I've only made Page.Recognise internal not public.
            page.Recognize();

            Interop.TessApi.Native.ResultRendererAddImage(_handle, page.Engine.Handle);   
        }

        /// <summary>
        /// Notifies the renderer to start a new document, ensure any previous 'documents' have been disposed off.
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public IDisposable BeginDocument(string title)
        {
            Guard.RequireNotNull("title", title);
            VerifyNotDisposed();

            Interop.TessApi.Native.ResultRendererBeginDocument(_handle, title);

            return new EndDocumentOnDispose(this);
        }
        
        public int PageNumber
        {
            get
            {
                VerifyNotDisposed();

                return Interop.TessApi.Native.ResultRendererImageNum(_handle);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (_handle.Handle != IntPtr.Zero) {
                Interop.TessApi.Native.DeleteResultRenderer(_handle);
                _handle = new HandleRef(this, IntPtr.Zero);
            }
        }
    }
}
