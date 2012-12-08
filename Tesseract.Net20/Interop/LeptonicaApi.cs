using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Tesseract.Interop
{
    public unsafe static class LeptonicaApi
    {

        static LeptonicaApi()
        {
            var type = typeof(LeptonicaApi);
            // This may have already been loaded by tesseract but that's fine (EmbeddedDllLoader won't try and load the dll again).
            EmbeddedDllLoader.Instance.LoadEmbeddedDll(type.Assembly, type.Namespace, "liblept168.dll");

        }
        [DllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixCreate")]
        public static unsafe extern IntPtr Create(int width, int height, int depth);
        
        [DllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixDestroy")]
        public static extern void Destroy(ref IntPtr handle);

        [DllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixGetWidth")]
        public static extern int GetWidth(IntPtr handle);

        [DllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixGetHeight")]
        public static extern int GetHeight(IntPtr handle);

        [DllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixGetDepth")]
        public static extern int GetDepth(IntPtr handle);

        [DllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixGetData")]
        public static extern uint* GetData(IntPtr handle);

        [DllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixRead")]
        public static extern IntPtr LoadFromFile(string filename);

        [DllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixRead")]
        public static extern int Save(string filename, IntPtr handle, ImageFormat format);
    }
}
