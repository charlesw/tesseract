using System;
using System.Runtime.InteropServices;
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
    public abstract class ResultRenderer : DisposableBase, IResultRenderer
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
            return new PdfResultRenderer(outputFilename, fontDirectory);
        }

        /// <summary>
        /// Creates a <see cref="IResultRenderer">result renderer</see> that render that generates UTF-8 encoded text
        /// file from tesseract's output.
        /// </summary>
        /// <param name="outputFilename">The path to the text file to be generated without the file extension.</param>
        /// <returns></returns>
        public static IResultRenderer CreateTextRenderer(string outputFilename)
        {
            return new TextResultRenderer(outputFilename);
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
            return new HOcrResultRenderer(outputFilename, fontInfo);
        }

        /// <summary>
        /// Creates a <see cref="IResultRenderer">result renderer</see> that render that generates a unlv
        /// file from tesseract's output.
        /// </summary>
        /// <param name="outputFilename">The path to the unlv file to be created without the file extension.</param>
        /// <returns></returns>
        public static IResultRenderer CreateUnlvRenderer(string outputFilename)
        {
            return new UnlvResultRenderer(outputFilename);
        }

        /// <summary>
        /// Creates a <see cref="IResultRenderer">result renderer</see> that render that generates a box text file from tesseract's output.
        /// </summary>
        /// <param name="outputFilename">The path to the box file to be created without the file extension.</param>
        /// <returns></returns>
        public static IResultRenderer CreateBoxRenderer(string outputFilename)
        {
            return new BoxResultRenderer(outputFilename);
        }

        #endregion Factory Methods

        /// <summary>
        /// Ensures the renderer's EndDocument when disposed off.
        /// </summary>
        private class EndDocumentOnDispose : IDisposable
        {
            private readonly ResultRenderer _renderer;

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

        protected ResultRenderer()
        {
            _handle = new HandleRef(this, IntPtr.Zero);
        }

        /// <summary>
        /// Initialise the render to use the specified native result renderer.
        /// </summary>
        /// <param name="handle"></param>
        protected void Initialise(IntPtr handle)
        {
            Guard.Require("handle", handle != IntPtr.Zero, "handle must be initialised.");
            Guard.Verify(_handle.Handle == IntPtr.Zero, "Rensult renderer has already been initialised.");

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

            Interop.TessApi.Native.ResultRendererAddImage(Handle, page.Engine.Handle);
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

            Interop.TessApi.Native.ResultRendererBeginDocument(Handle, title);

            return new EndDocumentOnDispose(this);
        }

        protected HandleRef Handle
        {
            get { return _handle; }
        }

        public int PageNumber
        {
            get
            {
                VerifyNotDisposed();

                return Interop.TessApi.Native.ResultRendererImageNum(Handle);
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

    public sealed class TextResultRenderer : ResultRenderer
    {
        public TextResultRenderer(string outputFilename)
        {
            var rendererHandle = Interop.TessApi.Native.TextRendererCreate(outputFilename);
            Initialise(rendererHandle);
        }
    }

    public sealed class HOcrResultRenderer : ResultRenderer
    {
        public HOcrResultRenderer(string outputFilename, bool fontInfo = false)
        {
            var rendererHandle = Interop.TessApi.Native.HOcrRendererCreate2(outputFilename, fontInfo ? 1 : 0);
            Initialise(rendererHandle);
        }
    }

    public sealed class UnlvResultRenderer : ResultRenderer
    {
        public UnlvResultRenderer(string outputFilename)
        {
            var rendererHandle = Interop.TessApi.Native.UnlvRendererCreate(outputFilename);
            Initialise(rendererHandle);
        }
    }

    public sealed class BoxResultRenderer : ResultRenderer
    {
        public BoxResultRenderer(string outputFilename)
        {
            var rendererHandle = Interop.TessApi.Native.BoxTextRendererCreate(outputFilename);
            Initialise(rendererHandle);
        }
    }

    public sealed class PdfResultRenderer : ResultRenderer
    {
        private IntPtr _fontDirectoryHandle;

        public PdfResultRenderer(string outputFilename, string fontDirectory)
        {
            var fontDirectoryHandle = Marshal.StringToHGlobalAnsi(fontDirectory);
            var rendererHandle = Interop.TessApi.Native.PDFRendererCreate(outputFilename, fontDirectoryHandle);

            Initialise(rendererHandle);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            // dispose of font
            if (_fontDirectoryHandle != IntPtr.Zero) {
                Marshal.FreeHGlobal(_fontDirectoryHandle);
                _fontDirectoryHandle = IntPtr.Zero;
            }
        }
    }
}