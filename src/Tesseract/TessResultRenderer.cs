using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Tesseract
{
    public abstract class TessResultRenderer : IDisposable
    {
        public IntPtr Handle;
        
        public void Dispose()
        {
            if(Handle != IntPtr.Zero)
            {
                Handle = IntPtr.Zero;
            }
        }

        public HandleRef GetRef()
        {
            return new HandleRef(this, Handle);
        }

        public bool BeginDocument(string title)
        {
            return Interop.TessApi.Native.ResultRendererBeginDocument(GetRef(), title);
        }
        
        public bool EndDocument()
        {
            return Interop.TessApi.Native.ResultRendererEndDocument(GetRef());
        }
    }
}
