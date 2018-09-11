using System;
using System.Collections.Generic;
using System.IO;

namespace Tesseract
{
    /// <summary>
    /// Access TIFF images one at a time to limit memory utilization.
    /// </summary>
    public sealed class TiffReader
    {
        /// <summary>
        /// The file being read.
        /// </summary>
        private string Filename { get; set; }

        /// <summary>
        /// IFD offset cache.
        /// </summary>
        private List<int> Offsets { get; set; } = new List<int>();

        /// <summary>
        /// When true EOF was encountered and there are no more IFDs.
        /// </summary>
        private bool reachedEndOfFile = false;

        /// <summary>
        /// Initialize an instance of TiffReader for the specified file.
        /// </summary>
        /// <param name="filename">The path to the TIFF file to read.</param>
        public TiffReader(string filename)
        {
            if (filename == null || filename == string.Empty)
            {
                throw new ArgumentOutOfRangeException("filename");
            }

            Filename = filename;
            Offsets.Add(0);
        }

        /// <summary>
        /// Get an image corresponding to the TIFF's IFD number using the offset cache.  If the offset has not
        /// been previously visited then the request is rejected.
        /// </summary>
        /// <param name="i">The IFD number, origin zero.</param>
        /// <returns>null if the IFD isn't present, or the image.</returns>
        public Pix GetPix(int i)
        {
            int offset = 0;
            if (i < Offsets.Count)
            {
                offset = Offsets[i];
            }
            else
            {
                throw new IOException("Invalid sequence.");
            }

            IntPtr handle = Interop.LeptonicaApi.Native.pixReadFromMultipageTiff(Filename, ref offset);

            if (handle == IntPtr.Zero)
            {
                return null;
            }

            return Pix.Create(handle);
        }

        /// <summary>
        /// Get the next IFD entry from this TIFF.
        /// </summary>
        /// <returns>null if there are no more images.</returns>
        public Pix NextPix()
        {
            if (reachedEndOfFile)
            {
                return null;
            }

            IntPtr handle;
            if (Offsets.Count > 0)
            {
                int offset = Offsets[Offsets.Count - 1];
                handle = Interop.LeptonicaApi.Native.pixReadFromMultipageTiff(Filename, ref offset);

                if (handle == IntPtr.Zero)
                {
                    return null;
                }

                if (offset != 0)
                {
                    Offsets.Add(offset);
                }
                else
                {
                    reachedEndOfFile = true;
                }
            }
            else
            {
                throw new IOException("Invalid state.");
            }
            
            return Pix.Create(handle);
        }
    }
}
