using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Tesseract.Renderers
{
    public class HocrRenderer:ResultRenderer
    {
        public HocrRenderer(string outputbase)
        {
            OutputBase = outputbase;

            VerifyNotDisposed();

            Handle = new HandleRef(this, Interop.TessApi.Native.HOcrRendererCreate(outputbase));
        }
    }
}
