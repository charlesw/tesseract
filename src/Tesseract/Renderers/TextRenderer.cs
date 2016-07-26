using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Tesseract.Renderers
{
    public class TextRenderer:ResultRenderer
    {      
        public TextRenderer(string outputbase)
        {
            OutputBase = outputbase;

            VerifyNotDisposed();

            Handle = new HandleRef(this, Interop.TessApi.Native.TextRendererCreate(outputbase));
        }
    }
}
