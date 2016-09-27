using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Tesseract.Renderers
{
    public class PdfRenderer:ResultRenderer
    {
        public string TessdataPath;        
        
        public PdfRenderer(string outputbase, string tessdataPath)
        {
            TessdataPath = tessdataPath;
            OutputBase = outputbase;

            VerifyNotDisposed();

            Handle = new HandleRef(this, Interop.TessApi.Native.PDFRendererCreate(outputbase, Marshal.StringToHGlobalAnsi(TessdataPath)));
        }
    }
}
