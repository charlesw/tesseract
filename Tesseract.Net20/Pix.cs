using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace Tesseract
{
    /// <summary>
    /// Wraps leptonica's pix structure.
    /// </summary>
    public interface IPix : IDisposable
    {
        IntPtr Handle { get; }
        int Width { get; }
        int Height { get; }
        int Depth { get; }

        void Save(string filename, ImageFormat format = ImageFormat.Default);
    }

    public unsafe abstract class Pix : DisposableBase, IPix
    {
        private static readonly List<int> AllowedDepths = new List<int> { 1, 2, 4, 8, 16, 32 };

        protected IntPtr handle;
        protected readonly int width;
        protected readonly int height;
        protected readonly int depth;
        protected readonly int pixelCount;
        protected readonly uint* data;

        public static IPix Create(int width, int height, int depth)
        {
            if(!AllowedDepths.Contains(depth))
                throw new ArgumentException("Depth must be 1, 2, 4, 8, 16, or 32 bits.", "depth");

            if (width <= 0) throw new ArgumentException("Width must be greater than zero", "width");
            if (height <= 0) throw new ArgumentException("Height must be greater than zero", "height");

            var handle = Interop.LeptonicaApi.Create(width, height, depth);
            if (handle == IntPtr.Zero) throw new InvalidOperationException("Failed to create pix, this normally occurs because the requested image size is too large, please check Standard Error Output.");

            return Create(handle);
        }

        public static IPix Create(IntPtr handle)
        {
            if (handle == IntPtr.Zero) throw new ArgumentException("Pix handle must not be zero (null).", "handle");

            var depth = Interop.LeptonicaApi.GetDepth(handle);
            if (depth == 1) {
                return new Pix1Bit(handle);
            } else if (depth == 2) {
                return new Pix2Bit(handle);
            } else if(depth == 4) {
                return new Pix4Bit(handle);
            } else if(depth == 8) {
                return new Pix8Bit(handle);
            } else if (depth == 16) {
                return new Pix16Bit(handle);
            } else if (depth == 32) {
                return new Pix32Bit(handle);
            } else {
                throw new ArgumentException("Depth must be 1, 2, 4, 8, 16, or 32 bits.");
            }
        }

        public static IPix LoadFromFile(string filename)
        {
            var pixHandle = Interop.LeptonicaApi.LoadFromFile(filename);
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
        public Pix(IntPtr handle)
        {
            if (handle == IntPtr.Zero) throw new ArgumentNullException("handle");
            this.handle = handle;
            this.width = Interop.LeptonicaApi.GetWidth(handle);
            this.height = Interop.LeptonicaApi.GetHeight(handle);
            this.depth = Interop.LeptonicaApi.GetDepth(handle);
            this.data = Interop.LeptonicaApi.GetData(handle);
            this.pixelCount = width * height;
        }

        
        public void Save(string filename, ImageFormat format = ImageFormat.Default)
        {
            if (Interop.LeptonicaApi.Save(filename, handle, format) != 0) {
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

        public IntPtr Handle
        {
            get { return handle; }
        }

        protected override void Dispose(bool disposing)
        {
            Interop.LeptonicaApi.Destroy(ref handle);
        }
    }

    public unsafe sealed class Pix1Bit : Pix
    {
        public Pix1Bit(IntPtr handle)
            : base(handle)
        {
            if (Depth != 1) throw new TesseractException("Pix must have a depth of 1 bit.");
        }

        // data access routines
        public uint this[int index]
        {
#if Net45
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
            get
            {
                return (*(this.data + ((index) >> 5)) >> (31 - ((index) & 31))) & 1;
            }
#if Net45
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
            set
            {
                uint* wordPtr = data + ((index) >> 5);
                *wordPtr &= ~(0x80000000 >> ((index) & 31));
                *wordPtr |= (value << (31 - ((index) & 31)));
            }
        }
    }

    public unsafe sealed class Pix2Bit : Pix
    {
        public Pix2Bit(IntPtr handle)
            : base(handle)
        {
            if (Depth != 2) throw new TesseractException("Pix must have a depth of 2 bits.");
        }

        // data access routines
        public uint this[int index]
        {
#if Net45
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
            get
            {
                return (*(data + ((index) >> 4)) >> (2 * (15 - ((index) & 15)))) & 3;
            }
#if Net45
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
            set
            {
                uint* wordPtr = data + ((index) >> 4);
                *wordPtr &= ~(0xc0000000 >> (2 * ((index) & 15)));
                *wordPtr |= (((value) & 3) << (30 - 2 * ((index) & 15)));
            }
        }
    }

    public unsafe sealed class Pix4Bit : Pix
    {
        public Pix4Bit(IntPtr handle)
            : base(handle)
        {
            if (Depth != 4) throw new TesseractException("Pix must have a depth of 4 bits.");
        }

        // data access routines
        public uint this[int index]
        {
#if Net45
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
            get
            {
                return (*(data + ((index) >> 3)) >> (4 * (7 - ((index) & 7)))) & 0xf;
            }
#if Net45
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
            set
            {
                uint* wordPtr = data + ((index) >> 3);
                *wordPtr &= ~(0xf0000000 >> (4 * ((index) & 7)));
                *wordPtr |= (((value) & 15) << (28 - 4 * ((index) & 7)));
            }
        }
    }


    public unsafe sealed class Pix8Bit : Pix
    {
        public Pix8Bit(IntPtr handle)
            : base(handle)
        {
            if (Depth != 8) throw new TesseractException("Pix must have a depth of 8 bits.");
        }

        // data access routines
        public byte this[int index]
        {
#if Net45
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
            get
            {
#if LittleEndian
    #if X64
                        return *((byte*)((ulong)((byte*)data + index) ^ 3));
    #else
                        return *((byte*)((uint)((byte*)data + index) ^ 3));
    #endif
#else
                    return *((byte*)data + index);  
#endif
            }
#if Net45
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
            set
            {
#if LittleEndian
    #if X64
                *(byte*)((ulong)((byte*)data + index) ^ 3) = value;
    #else
                *(byte*)((uint)((byte*)data + index) ^ 3) = value;
    #endif
#else
                *((byte*)data + index) = value;
#endif
            }
        }
    }

    public unsafe sealed class Pix16Bit : Pix
    {
        public Pix16Bit(IntPtr handle)
            : base(handle)
        {
            if (Depth != 16) throw new TesseractException("Pix must have a depth of 16 bits.");
        }

        // data access routines
        public ushort this[int index]
        {
#if Net45
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
            get
            {
#if LittleEndian
    #if X64
                return *(ushort*)((ulong)((ushort*)data + index) ^ 2);
    #else
                return *(ushort*)((uint)((ushort*)data + index) ^ 2);
    #endif
#else
                return *((ushort*)data + index);
#endif
            }
#if Net45
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
            set
            {
#if LittleEndian
    #if X64
                *(ushort*)((ulong)((ushort*)data + index) ^ 2) = value;
    #else
                *(ushort*)((uint)((ushort*)data + index) ^ 2) = value;
    #endif
#else
                *((ushort*)data + index) = value;
#endif
            }
        }
    }

    public unsafe sealed class Pix32Bit : Pix
    {
        public Pix32Bit(IntPtr handle)
            : base(handle)
        {
            if (Depth != 32) throw new TesseractException("Pix must have a depth of 32 bits.");
        }

        // data access routines
        public uint this[int index]
        {
#if Net45
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
            get
            {
                return *(data + index);
            }
#if Net45
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
            set
            {
                *(data + index) = value;
            }
        }
    }
}
