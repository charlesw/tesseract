using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Tesseract.Interop
{
    internal class DllLoader
    {
        private static readonly TraceSource trace = new TraceSource("Tesseract");

        /// <summary>
        /// Load a library DLL in advance of it's use
        /// </summary>
        /// <param name="dllName"></param>
        /// <returns></returns>
        internal static bool ForceLoadLibrary(string dllName)
        {
            var thisDllPath = new Uri(typeof(DllLoader).Assembly.CodeBase).LocalPath;
            var newDllPath = Path.Combine(Path.GetDirectoryName(thisDllPath), (IntPtr.Size == 8 ? "x64" : "x86"), dllName + (dllName.ToLower().EndsWith(".dll") ? "" : ".dll"));

            if (!File.Exists(newDllPath))
            {
                trace.TraceEvent(TraceEventType.Error, 0, "Requested DLL does not exist at {0}", newDllPath);
                return false;
            }

            trace.TraceEvent(TraceEventType.Verbose, 0, "Attempting to load DLL {0}", newDllPath);
            IntPtr ret = LoadLibraryEx(newDllPath, IntPtr.Zero, 0x00001000|0x00000100); // Careful of dependencies
            if (IntPtr.Zero == ret)
            {
                trace.TraceEvent(TraceEventType.Error, 0, "Failed to load library {0}, error {1}", newDllPath, Marshal.GetLastWin32Error());
                return false;
            }
            return true;
        }

        [DllImport("kernel32", BestFitMapping = false, CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Unicode, EntryPoint = "LoadLibraryExW", SetLastError = true)]
        static extern IntPtr LoadLibraryEx([MarshalAs(UnmanagedType.LPWStr)] string lpLibFileName, IntPtr hFile, uint dwFlags);

    }
}
