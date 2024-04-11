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

        private static readonly string FileExtension = SystemManager.GetOperatingSystem() == OperatingSystem.MacOSX ? ".dylib" : ".so";

        public string FixUpLibraryName(string fileName)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                if (!fileName.EndsWith(FileExtension, StringComparison.OrdinalIgnoreCase))
                    fileName += FileExtension;
                if (!fileName.StartsWith("lib", StringComparison.OrdinalIgnoreCase))
                    fileName = "lib" + fileName;
            }
            return fileName;
        }

        const int RTLD_NOW = 2;

        private static bool? _useSystemLibrary2;

        private static IntPtr UnixLoadLibrary(String fileName, int flags)
        {
            if (!_useSystemLibrary2.HasValue)
            {
                try
                {
                    var result = dlopen2(fileName, flags);
                    _useSystemLibrary2 = true;
                    return result;
                }
                catch (DllNotFoundException)
                {
                    _useSystemLibrary2 = false;
                    return dlopen1(fileName, flags);
                }
            }

            return _useSystemLibrary2.Value ? dlopen2(fileName, flags) : dlopen1(fileName, flags);
        }

        private static int UnixFreeLibrary(IntPtr handle)
        {
            if (!_useSystemLibrary2.HasValue)
                throw new Exception($"Expected ${nameof(UnixLoadLibrary)} to have been called.");
            return _useSystemLibrary2.Value ? dlclose2(handle) : dlclose1(handle);
        }

        private static IntPtr UnixGetProcAddress(IntPtr handle, String symbol)
        {
            if (!_useSystemLibrary2.HasValue)
                throw new Exception($"Expected ${nameof(UnixLoadLibrary)} to have been called.");
            return _useSystemLibrary2.Value ? dlsym2(handle, symbol) : dlsym1(handle, symbol);
        }

        private static IntPtr UnixGetLastError()
        {
            if (!_useSystemLibrary2.HasValue)
                throw new Exception($"Expected ${nameof(UnixLoadLibrary)} to have been called.");
            return _useSystemLibrary2.Value ? dlerror2() : dlerror1();
        }

        [DllImport("libdl", EntryPoint = "dlopen")]
        private static extern IntPtr dlopen1(String fileName, int flags);

        [DllImport("libdl", EntryPoint = "dlclose", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern int dlclose1(IntPtr handle);

        [DllImport("libdl", EntryPoint = "dlsym", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr dlsym1(IntPtr handle, String symbol);

        [DllImport("libdl", EntryPoint = "dlerror", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr dlerror1();

        [DllImport("libdl.so.2", EntryPoint = "dlopen")]
        private static extern IntPtr dlopen2(String fileName, int flags);

        [DllImport("libdl.so.2", EntryPoint = "dlclose", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern int dlclose2(IntPtr handle);

        [DllImport("libdl.so.2", EntryPoint = "dlsym", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr dlsym2(IntPtr handle, String symbol);

        [DllImport("libdl.so.2", EntryPoint = "dlerror", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr dlerror2();
    }
}