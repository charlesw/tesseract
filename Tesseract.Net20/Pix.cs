using System;
using System.Collections.Generic;
using System.IO;

namespace Tesseract
{
    public unsafe sealed class Pix : DisposableBase
    {
        private static readonly List<int> AllowedDepths = new List<int> { 1, 2, 4, 8, 16, 32 };

        private IntPtr handle;
        private readonly int width;
        private readonly int height;
        private readonly int depth;

        public static Pix Create(int width, int height, int depth)
        {
            if (!AllowedDepths.Contains(depth))
                throw new ArgumentException("Depth must be 1, 2, 4, 8, 16, or 32 bits.", "depth");

            if (width <= 0) throw new ArgumentException("Width must be greater than zero", "width");
            if (height <= 0) throw new ArgumentException("Height must be greater than zero", "height");

            var handle = Interop.LeptonicaApi.pixCreate(width, height, depth);
            if (handle == IntPtr.Zero) throw new InvalidOperationException("Failed to create pix, this normally occurs because the requested image size is too large, please check Standard Error Output.");

            return Create(handle);
        }

        public static Pix Create(IntPtr handle)
        {
            if (handle == IntPtr.Zero) throw new ArgumentException("Pix handle must not be zero (null).", "handle");

            return new Pix(handle);
        }

        public static Pix LoadFromFile(string filename)
        {
            var pixHandle = Interop.LeptonicaApi.pixRead(filename);
            if (pixHandle == IntPtr.Zero) {
                throw new IOException(String.Format("Failed to load image '{0}'.", filename));
            }
            return Create(pixHandle);
        }


        /// <summary>
        /// Creates a new pix instance using an existing handle to a pix structure.
        /// </summary>
        /// <remarks>
        /// Note that the resulting instance takes ownership of the data structure.
        /// </remarks>
        /// <param name="handle"></param>
        private Pix(IntPtr handle)
        {
            if (handle == IntPtr.Zero) throw new ArgumentNullException("handle");
            this.handle = handle;
            this.width = Interop.LeptonicaApi.pixGetWidth(handle);
            this.height = Interop.LeptonicaApi.pixGetHeight(handle);
            this.depth = Interop.LeptonicaApi.pixGetHeight(handle);
        }


        public void Save(string filename, ImageFormat format = ImageFormat.Default)
        {
            if (Interop.LeptonicaApi.pixWrite(filename, handle, format) != 0) {
                throw new IOException(String.Format("Failed to save image '{0}'.", filename));
            }
        }

        public int Width
        {
            get { return width; }
        }

        public int Height
        {
            get { return height; }
        }

        public int Depth
        {
            get { return depth; }
        }

        public PixData GetData()
        {
            return new PixData(this);
        }

        public IntPtr Handle
        {
            get { return handle; }
        }

        protected override void Dispose(bool disposing)
        {
            Interop.LeptonicaApi.pixDestroy(ref handle);
        }
    }
}
