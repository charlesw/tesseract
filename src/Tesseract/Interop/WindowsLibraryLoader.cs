﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;

namespace Tesseract.Interop
{
    /// <summary>
    /// Handles loading embedded dlls into memory, based on http://stackoverflow.com/questions/666799/embedding-unmanaged-dll-into-a-managed-c-sharp-dll.
    /// </summary>
    public class WindowsLibraryLoader
    {
        private class ProcessArchitectureInfo
        {
            public ProcessArchitectureInfo()
            {
                Warnings = new List<string>();
            }

            public string Architecture { get; set; }
            public List<string> Warnings { get; set; }

            public bool HasWarnings
            {
                get { return Warnings.Count > 0; }
            }

            public string WarningText()
            {
#if Net20
                return String.Join("\r\n", Warnings.ToArray());
#else
                return String.Join("\r\n", Warnings);
#endif
            }
        }

        #region Singleton pattern

        private static readonly WindowsLibraryLoader instance = new WindowsLibraryLoader();

        public static WindowsLibraryLoader Instance { get { return instance; } }


        #endregion

        private readonly object syncLock = new object();

        /// <summary>
        /// The default base directory name to copy the assemblies too.
        /// </summary>
        private const string DefaultTempDirectory = "TesseractOcr";
        private const string PROCESSOR_ARCHITECTURE = "PROCESSOR_ARCHITECTURE";
        private const string DllFileExtension = ".dll";

#if Net40 || Net45
        private HashSet<string> loadedAssemblies = new HashSet<string>();
#else
        private List<string> loadedAssemblies = new List<string>();
#endif
        // Map processor 
        private readonly Dictionary<string, string> processorArchitecturePlatforms;
        
        // Used as a sanity check for the returned processor architecture to double check the returned value.
        private readonly Dictionary<string, int> processorArchitectureAddressWidthPlatforms;
        private WindowsLibraryLoader()
        {
            processorArchitecturePlatforms = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            processorArchitecturePlatforms.Add("x86", "x86");
            processorArchitecturePlatforms.Add("AMD64", "x64");
            processorArchitecturePlatforms.Add("IA64", "Itanium");
            processorArchitecturePlatforms.Add("ARM", "WinCE");

            processorArchitectureAddressWidthPlatforms = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            processorArchitectureAddressWidthPlatforms.Add("x86", 4);
            processorArchitectureAddressWidthPlatforms.Add("AMD64", 8);
            processorArchitectureAddressWidthPlatforms.Add("IA64", 8);
            processorArchitectureAddressWidthPlatforms.Add("ARM", 4);
        }

        public bool IsLibraryLoaded(string dllName)
        {
            lock (syncLock) {
                return loadedAssemblies.Contains(dllName);
            }
        }

        public bool IsCurrentPlatformSupported()
        {
            return Environment.OSVersion.Platform == PlatformID.Win32NT ||
                Environment.OSVersion.Platform == PlatformID.Win32Windows;
        }
      
        public void LoadLibrary(string dllName)
        {
            if (IsCurrentPlatformSupported()) {
                try {
                    lock (syncLock) {
                        if (!loadedAssemblies.Contains(dllName)) {
                            var processArch = GetProcessArchitecture();
                            IntPtr dllHandle;
                            string baseDirectory;

                            // Try loading from executing assembly domain
                            var executingAssembly = Assembly.GetExecutingAssembly();
                            baseDirectory = Path.GetDirectoryName(executingAssembly.Location);
                            dllHandle = LoadLibraryInternal(dllName, baseDirectory, processArch);
                            if (dllHandle != IntPtr.Zero) return;

                            // Fallback to current app domain
                            baseDirectory = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory);
                            dllHandle = LoadLibraryInternal(dllName, baseDirectory, processArch);
                            if (dllHandle != IntPtr.Zero) return;

                            // Finally try the working directory
                            baseDirectory = Path.GetFullPath(Environment.CurrentDirectory);
                            dllHandle = LoadLibraryInternal(dllName, baseDirectory, processArch);
                            if (dllHandle != IntPtr.Zero) return;
                            
                            // ASP.NET hack, requires an active context
                            #if !ClientProfile
                            if(HttpContext.Current != null) {
                            	var server = HttpContext.Current.Server;
                            	baseDirectory = Path.GetFullPath(server.MapPath("bin"));
	                            dllHandle = LoadLibraryInternal(dllName, baseDirectory, processArch);
	                            if (dllHandle != IntPtr.Zero) return;	                            
                            }
                            #endif
                            StringBuilder errorMessage = new StringBuilder();
                            errorMessage.AppendFormat("Failed to find dll \"{0}\", for processor architecture {1}.", dllName, processArch.Architecture);
                            if (processArch.HasWarnings) {
                                // include process detection warnings
                                errorMessage.AppendFormat("\r\nWarnings: \r\n{0}", processArch.WarningText());
                            }
                            throw new LoadLibraryException(errorMessage.ToString());
                        }
                    }
                } catch (LoadLibraryException e) {
                    Trace.TraceError(e.Message);
                }
            }
        }

        /// <summary>
        /// Get's the current process architecture while keeping track of any assumptions or possible errors.
        /// </summary>
        /// <returns></returns>
        private ProcessArchitectureInfo GetProcessArchitecture()
        {
            ProcessArchitectureInfo processInfo = new ProcessArchitectureInfo();

            // BUGBUG: Will this always be reliable?
            string processArchitecture = Environment.GetEnvironmentVariable(PROCESSOR_ARCHITECTURE);
            if (!String.IsNullOrEmpty(processArchitecture)) {
                // Sanity check
                processInfo.Architecture = processArchitecture;
            } else {
                processInfo.Architecture = "x86";
                processInfo.Warnings.Add("Failed to detect processor architecture, falling back to x86.");
            }


            var addressWidth = processorArchitectureAddressWidthPlatforms[processInfo.Architecture];
            if (addressWidth != IntPtr.Size) {
                processInfo.Warnings.Add(String.Format("Expected the detected processing architecture of {0} to have an address width of {1} Bytes but was {2} Bytes.", processInfo.Architecture, addressWidth, IntPtr.Size));
            }

            return processInfo;
        }

        private IntPtr LoadLibraryInternal(string dllName, string baseDirectory, ProcessArchitectureInfo processArchInfo)
        {
            IntPtr libraryHandle = IntPtr.Zero;
            var platformName = GetPlatformName(processArchInfo.Architecture);
            var expectedDllDirectory = Path.Combine(baseDirectory, platformName);
            var fileName = FixUpDllFileName(Path.Combine(expectedDllDirectory, dllName));

            if (File.Exists(fileName)) {
                // Attempt to load dll
                try {
                    // Show where we're trying to load the file from
                    Trace.TraceInformation(String.Format(CultureInfo.CurrentCulture,
                          "Trying to load native library \"{0}\"...",
                          fileName));

                    libraryHandle = Win32LoadLibrary(fileName);
                    if (libraryHandle != IntPtr.Zero) {
                        // library has been loaded
                        Trace.TraceInformation(String.Format(CultureInfo.CurrentCulture,
                          "Successfully loaded native library \"{0}\".",
                          fileName));
                        loadedAssemblies.Add(dllName);
                    } else {
                        Trace.TraceError(String.Format("Failed to load native library \"{0}\".\r\nCheck windows event log.", fileName));
                    }
                } catch (Exception e) {
                    var lastError = Marshal.GetLastWin32Error();
                    Trace.TraceError(String.Format("Failed to load native library \"{0}\".\r\nLast Error:{1}\r\nCheck inner exception and\\or windows event log.\r\nInner Exception: {2}", fileName, lastError, e.ToString()));
                }
            } else {
                Trace.TraceWarning(String.Format(CultureInfo.CurrentCulture,
                          "The native library \"{0}\" does not exist.",
                          fileName));
            }

            return libraryHandle;
        }

        /// <summary>
        /// Determines if the dynamic link library file name requires a suffix
        /// and adds it if necessary.
        /// </summary>
        private string FixUpDllFileName(string fileName)
        {
            if (!String.IsNullOrEmpty(fileName)) {
                PlatformID platformId = Environment.OSVersion.Platform;

                if ((platformId == PlatformID.Win32S) ||
                    (platformId == PlatformID.Win32Windows) ||
                    (platformId == PlatformID.Win32NT) ||
                    (platformId == PlatformID.WinCE)) {
                    if (!fileName.EndsWith(DllFileExtension,
                            StringComparison.OrdinalIgnoreCase)) {
                        return fileName + DllFileExtension;
                    }
                }
            }

            return fileName;
        }

        /// <summary>
        /// Given the processor architecture, returns the name of the platform.
        /// </summary>
        private string GetPlatformName(string processorArchitecture)
        {
            if (String.IsNullOrEmpty(processorArchitecture))
                return null;

            string platformName;
            if (processorArchitecturePlatforms.TryGetValue(processorArchitecture, out platformName)) {
                return platformName;
            }

            return null;
        }

        [DllImport("kernel32", EntryPoint = "LoadLibrary", CallingConvention = CallingConvention.Winapi,
            SetLastError = true, CharSet = CharSet.Auto, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        private static extern IntPtr Win32LoadLibrary(string dllPath);
    }
}
