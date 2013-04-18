using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Tesseract.Interop
{
    /// <summary>
    /// Handles loading embedded dlls into memory, based on http://stackoverflow.com/questions/666799/embedding-unmanaged-dll-into-a-managed-c-sharp-dll.
    /// </summary>
    public class EmbeddedDllLoader
    {
        private static readonly string InvalidDirectoryNameChars = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());

        #region Singleton pattern

        private static readonly EmbeddedDllLoader instance = new EmbeddedDllLoader();

        public static EmbeddedDllLoader Instance { get { return instance; } }


        #endregion

        private readonly object syncLock = new object();
#if Net40 || Net45
        private HashSet<string> loadedAssemblies = new HashSet<string>();
#else
        private List<string> loadedAssemblies = new List<string>();
#endif
        private string directoryName;

        private EmbeddedDllLoader()
        {
            var process =  System.Diagnostics.Process.GetCurrentProcess().MainModule;
            var processName =  Path.GetFileNameWithoutExtension(process.FileName);
            
            // Make sure to avoid accidentally generating an invalid path
            var processComponent = String.Format("{0}_{1}", processName, process.FileVersionInfo.FileVersion);
            var cleanedProcessComponent = processComponent;
            foreach (var invalidChar in InvalidDirectoryNameChars)
            {
                cleanedProcessComponent = cleanedProcessComponent.Replace(invalidChar.ToString(), string.Empty);
            }

            directoryName = Path.Combine(Path.GetTempPath(), cleanedProcessComponent);

            Directory.CreateDirectory(directoryName);
        }

        public void LoadEmbeddedDll(Assembly assembly, string @namespace, string dllName)
        {
            var resourceName = @namespace + "." + dllName;
            lock (syncLock) {
                if (!loadedAssemblies.Contains(dllName)) {
                    var dllPath = Path.Combine(directoryName, dllName);

                    // make sure the file hasn't yet been copied over.
                    if (!File.Exists(dllName)) {
                        using (var resourceStream = assembly.GetManifestResourceStream(resourceName)) {
                            try {
                                using (var fileStream = File.Create(dllPath)) {
                                    const int bufferSize = 4096;
                                    byte[] buf = new byte[bufferSize];
                                    int bytesRead = resourceStream.Read(buf, 0, bufferSize);
                                    while (bytesRead >= 1) {
                                        fileStream.Write(buf, 0, bytesRead);
                                        bytesRead = resourceStream.Read(buf, 0, bufferSize);
                                    }
                                }
                            } catch (IOException e) {
                                // TODO: Introduce a file locking based mechanisim to prevent this.
                                Trace.TraceWarning("Failed to copy '{0}' to '{1}'\r\nDetail:{2}.", dllName, directoryName, e.ToString());
                            }
                        }
                    }


                    IntPtr h = LoadLibrary(dllPath);
                    if(h == IntPtr.Zero) throw new InvalidOperationException(String.Format("Failed to load '{0}'.", dllPath));

                    loadedAssemblies.Add(dllName);
                }
            }
        }

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr LoadLibrary(string dllPath);
    }
}
