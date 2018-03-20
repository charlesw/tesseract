//  Copyright (c) 2014 Andrey Akinshin
//  Project URL: https://github.com/AndreyAkinshin/InteropDotNet
//  Distributed under the MIT License: http://opensource.org/licenses/MIT
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using Tesseract.Internal;

namespace InteropDotNet
{
    public sealed class LibraryLoader
    {
        readonly ILibraryLoaderLogic logic;

        LibraryLoader(ILibraryLoaderLogic logic)
        {
            this.logic = logic;
        }

        private readonly object syncLock = new object();        
        private readonly Dictionary<string, IntPtr> loadedAssemblies = new Dictionary<string, IntPtr>();
        private string customSearchPath;

        public string CustomSearchPath
        {
            get { return customSearchPath; }
            set { customSearchPath = value; }
        }

        public IntPtr LoadLibrary(string fileName, string platformName = null)
        {            
            fileName = FixUpLibraryName(fileName);
            lock (syncLock)
            {
                if (!loadedAssemblies.ContainsKey(fileName))
                {
                    if (platformName == null)
                        platformName = SystemManager.GetPlatformName();
                    
                    Logger.TraceInformation("Current platform: " + platformName);
                                        
                    IntPtr dllHandle = CheckCustomSearchPath(fileName, platformName);
                    if (dllHandle == IntPtr.Zero)
                        dllHandle = CheckExecutingAssemblyDomain(fileName, platformName);
                    if (dllHandle == IntPtr.Zero)
                        dllHandle = CheckCurrentAppDomain(fileName, platformName);
                    if (dllHandle == IntPtr.Zero)
                        dllHandle = CheckCurrentAppDomainBin(fileName, platformName);
                    if (dllHandle == IntPtr.Zero)
                        dllHandle = CheckWorkingDirecotry(fileName, platformName);

                    if (dllHandle != IntPtr.Zero)
                        loadedAssemblies[fileName] = dllHandle;
                    else
                        throw new DllNotFoundException(string.Format("Failed to find library \"{0}\" for platform {1}.", fileName, platformName));
                }

                return loadedAssemblies[fileName];
            }
        }

        private IntPtr CheckCustomSearchPath(string fileName, string platformName)
        {
            var baseDirectory = CustomSearchPath;
            if (!String.IsNullOrEmpty(baseDirectory)) {
                Logger.TraceInformation("Checking custom search location '{0}' for '{1}' on platform {2}.", baseDirectory, fileName, platformName);
                return InternalLoadLibrary(baseDirectory, platformName, fileName);
            } else {
                Logger.TraceInformation("Custom search path is not defined, skipping.");
                return IntPtr.Zero;
            }

        }

        private IntPtr CheckExecutingAssemblyDomain(string fileName, string platformName)
        {
            var baseDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Logger.TraceInformation("Checking executing application domain location '{0}' for '{1}' on platform {2}.", baseDirectory, fileName, platformName);
            return InternalLoadLibrary(baseDirectory, platformName, fileName);
        }

        private IntPtr CheckCurrentAppDomain(string fileName, string platformName)
        {
            var baseDirectory = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory);
            Logger.TraceInformation("Checking current application domain location '{0}' for '{1}' on platform {2}.", baseDirectory, fileName, platformName);
            return InternalLoadLibrary(baseDirectory, platformName, fileName);
        }

        /// <summary>
        /// Special test for web applications.
        /// </summary>
        /// <remarks>
        /// Note that this makes a couple of assumptions these being:
        /// 
        /// <list type="bullet">
        ///     <item>That the current application domain's location for web applications corresponds to the web applications root directory.</item>
        ///     <item>That the tesseract\leptonica dlls reside in the corresponding x86 or x64 directories in the bin directory under the apps root directory.</item>
        /// </list>
        /// </remarks>
        /// <param name="fileName"></param>
        /// <param name="platformName"></param>
        /// <returns></returns>
        private IntPtr CheckCurrentAppDomainBin(string fileName, string platformName)
        {
            var baseDirectory = Path.Combine(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory), "bin");
            if (Directory.Exists(baseDirectory)) {
                Logger.TraceInformation("Checking current application domain's bin location '{0}' for '{1}' on platform {2}.", baseDirectory, fileName, platformName);
                return InternalLoadLibrary(baseDirectory, platformName, fileName);
            } else {
                Logger.TraceInformation("No bin directory exists under the current application domain's location, skipping.");
                return IntPtr.Zero;
            }
        }

        private IntPtr CheckWorkingDirecotry(string fileName, string platformName)
        {
            var baseDirectory = Path.GetFullPath(Environment.CurrentDirectory);
            Logger.TraceInformation("Checking working directory '{0}' for '{1}' on platform {2}.", baseDirectory, fileName, platformName);
            return InternalLoadLibrary(baseDirectory, platformName, fileName);
        }

        private IntPtr InternalLoadLibrary(string baseDirectory, string platformName, string fileName)
        {
            var fullPath = Path.Combine(baseDirectory, Path.Combine(platformName, fileName));
            return File.Exists(fullPath) ? logic.LoadLibrary(fullPath) : IntPtr.Zero;
        }

        public bool FreeLibrary(string fileName)
        {
            fileName = FixUpLibraryName(fileName);
            lock (syncLock)
            {
                if (!IsLibraryLoaded(fileName))
                {
                    Logger.TraceWarning("Failed to free library \"{0}\" because it is not loaded", fileName);
                    return false;
                }
                if (logic.FreeLibrary(loadedAssemblies[fileName]))
                {
                    loadedAssemblies.Remove(fileName);
                    return true;
                }
                return false;
            }
        }

        public IntPtr GetProcAddress(IntPtr dllHandle, string name)
        {
            return logic.GetProcAddress(dllHandle, name);
        }

        public bool IsLibraryLoaded(string fileName)
        {
            fileName = FixUpLibraryName(fileName);
            lock (syncLock)
                return loadedAssemblies.ContainsKey(fileName);
        }

        private string FixUpLibraryName(string fileName)
        {
            return logic.FixUpLibraryName(fileName);

        }

        #region Singleton

        private static LibraryLoader instance;

        public static LibraryLoader Instance
        {
            get
            {
                if (instance == null)
                {
                    var operatingSystem = SystemManager.GetOperatingSystem();
                    switch (operatingSystem)
                    {
                        case OperatingSystem.Windows:
                            Logger.TraceInformation("Current OS: Windows");
                            instance = new LibraryLoader(new WindowsLibraryLoaderLogic());
                            break;
                        case OperatingSystem.Unix:
                            Logger.TraceInformation("Current OS: Unix");
                            instance = new LibraryLoader(new UnixLibraryLoaderLogic());
                            break;
                        case OperatingSystem.MacOSX:
                            Logger.TraceInformation("Current OS: MacOsX");
                            instance = new LibraryLoader(new UnixLibraryLoaderLogic());
                            break;
                        default:
                            throw new Exception("Unsupported operation system");
                    }
                }
                return instance;
            }
        }

        #endregion
    }
}