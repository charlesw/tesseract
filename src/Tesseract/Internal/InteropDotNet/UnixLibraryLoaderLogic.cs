//  Copyright (c) 2014 Andrey Akinshin
//  Project URL: https://github.com/AndreyAkinshin/InteropDotNet
//  Distributed under the MIT License: http://opensource.org/licenses/MIT
using System;
using System.Runtime.InteropServices;

using Tesseract.Internal;

namespace InteropDotNet
{
    class UnixLibraryLoaderLogic : ILibraryLoaderLogic
    {
        public IntPtr LoadLibrary(string fileName)
        {
            var libraryHandle = IntPtr.Zero;

            try
            {
                Logger.TraceInformation("Trying to load native library \"{0}\"...", fileName);
                libraryHandle = UnixLoadLibrary(fileName, RTLD_NOW);
                if (libraryHandle != IntPtr.Zero)
                    Logger.TraceInformation("Successfully loaded native library \"{0}\", handle = {1}.", fileName, libraryHandle);
                else
                    Logger.TraceError("Failed to load native library \"{0}\".\r\nCheck windows event log.", fileName);
            }
            catch (Exception e)
            {
                var lastError = UnixGetLastError();
                Logger.TraceError("Failed to load native library \"{0}\".\r\nLast Error:{1}\r\nCheck inner exception and\\or windows event log.\r\nInner Exception: {2}", fileName, lastError, e.ToString());
            }

            return libraryHandle;
        }

        public bool FreeLibrary(IntPtr libraryHandle)
        {
            return UnixFreeLibrary(libraryHandle) != 0;
        }

        public IntPtr GetProcAddress(IntPtr libraryHandle, string functionName)
        {
            UnixGetLastError(); // Clearing previous errors
            Logger.TraceInformation("Trying to load native function \"{0}\" from the library with handle {1}...",
                functionName, libraryHandle);
            var functionHandle = UnixGetProcAddress(libraryHandle, functionName);
            var errorPointer = UnixGetLastError();
            if (errorPointer != IntPtr.Zero)
                throw new Exception("dlsym: " + Marshal.PtrToStringAnsi(errorPointer));
            if (functionHandle != IntPtr.Zero && errorPointer == IntPtr.Zero)
                Logger.TraceInformation("Successfully loaded native function \"{0}\", function handle = {1}.",
                    functionName, functionHandle);
            else
                Logger.TraceError("Failed to load native function \"{0}\", function handle = {1}, error pointer = {2}",
                    functionName, functionHandle, errorPointer);
            return functionHandle;
        }

        public string FixUpLibraryName(string fileName)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                if (!fileName.EndsWith(".so", StringComparison.OrdinalIgnoreCase))
                    fileName += ".so";
                if (!fileName.StartsWith("lib", StringComparison.OrdinalIgnoreCase))
                    fileName = "lib" + fileName;
            }
            return fileName;
        }

        const int RTLD_NOW = 2;

        [DllImport("libdl.so", EntryPoint = "dlopen")]
        private static extern IntPtr UnixLoadLibrary(String fileName, int flags);

        [DllImport("libdl.so", EntryPoint = "dlclose", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern int UnixFreeLibrary(IntPtr handle);

        [DllImport("libdl.so", EntryPoint = "dlsym", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr UnixGetProcAddress(IntPtr handle, String symbol);

        [DllImport("libdl.so", EntryPoint = "dlerror", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr UnixGetLastError();
    }
}