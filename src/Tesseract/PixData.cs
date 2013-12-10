using System;
using System.Runtime.CompilerServices;

namespace Tesseract
{
	public unsafe class PixData
	{
     
        public delegate uint Getter(uint* data, int index);
        public delegate uint Setter(uint* data, int index);

        public Pix Pix { get; private set; }

		internal PixData(Pix pix)
		{
            Pix = pix;
            Data = Interop.LeptonicaApi.pixGetData(pix.Handle);
            WordsPerLine = Interop.LeptonicaApi.pixGetWpl(pix.Handle);
		}
		
		/// <summary>
		/// Pointer to the data.
		/// </summary>
		public IntPtr Data { get; private set; }
		
		/// <summary>
		/// Number of 32-bit words per line. 
		/// </summary>
		public int WordsPerLine { get; private set; }

        /// <summary>
        /// Swaps the bytes on little-endian platforms within a word; bytes 0 and 3 swapped, and bytes `1 and 2 are swapped.
        /// </summary>
        /// <remarks>
        /// This is required for little-endians in situations where we convert from a serialized byte order that is in raster order, 
        /// as one typically has in file formats, to one with MSB-to-the-left in each 32-bit word, or v.v. See <seealso cref="http://www.leptonica.com/byte-addressing.html"/>
        /// </remarks>
        public void EndianByteSwap()
        {
            Interop.LeptonicaApi.pixEndianByteSwap(Pix.Handle);
        }

#if Net45
       	[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static uint EncodeAsRGBA(byte red, byte green, byte blue, byte alpha)
        {
            return (uint)((red << 24) |
                (green << 16) |
                (blue << 8) |
                alpha);
        }

        /// <summary>
        /// Gets the pixel value for a 1bpp image.
        /// </summary>
#if Net45
      	[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static uint GetDataBit(uint* data, int index)
		{
			return (*(data + ((index) >> 5)) >> (31 - ((index) & 31))) & 1;			
		}


        /// <summary>
        /// Sets the pixel value for a 1bpp image.
        /// </summary>
#if Net45
      	[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static void SetDataBit(uint* data, int index, uint value)
		{
			uint* wordPtr = data + ((index) >> 5);
            *wordPtr &= ~(0x80000000 >> ((index) & 31));
            *wordPtr |= (value << (31 - ((index) & 31)));		
		}


        /// <summary>
        /// Gets the pixel value for a 2bpp image.
        /// </summary>
#if Net45
      	[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static uint GetDataDIBit(uint* data, int index)
		{
			return (*(data + ((index) >> 4)) >> (2 * (15 - ((index) & 15)))) & 3;	
		}



        /// <summary>
        /// Sets the pixel value for a 2bpp image.
        /// </summary>
#if Net45
      	[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static void SetDataDIBit(uint* data, int index, uint value)
		{
			uint* wordPtr = data + ((index) >> 4);
            *wordPtr &= ~(0xc0000000 >> (2 * ((index) & 15)));
            *wordPtr |= (((value) & 3) << (30 - 2 * ((index) & 15)));			
		}


        /// <summary>
        /// Gets the pixel value for a 4bpp image.
        /// </summary>
#if Net45
      	[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static uint GetDataQBit(uint* data, int index)
		{
			return (*(data + ((index) >> 3)) >> (4 * (7 - ((index) & 7)))) & 0xf;			
		}


        /// <summary>
        /// Sets the pixel value for a 4bpp image.
        /// </summary>
#if Net45
      	[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static void SetDataQBit(uint* data, int index, uint value)
		{
			uint* wordPtr = data + ((index) >> 3);
            *wordPtr &= ~(0xf0000000 >> (4 * ((index) & 7)));
            *wordPtr |= (((value) & 15) << (28 - 4 * ((index) & 7)));		
		}


        /// <summary>
        /// Gets the pixel value for a 8bpp image.
        /// </summary>
#if Net45
      	[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static uint GetDataByte(uint* data, int index)
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


        /// <summary>
        /// Sets the pixel value for a 8bpp image.
        /// </summary>
#if Net45
      	[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static void SetDataByte(uint* data, int index, uint value)
		{
#if LittleEndian
    #if X64
    		*(byte*)((ulong)((byte*)data + index) ^ 3) = (byte)value;
    #else
            *(byte*)((uint)((byte*)data + index) ^ 3) = (byte)value;
    #endif
#else
			*((byte*)data + index) =  (byte)value;
#endif	
		}


        /// <summary>
        /// Gets the pixel value for a 16bpp image.
        /// </summary>
#if Net45
      	[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static uint GetDataTwoByte(uint* data, int index)
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


        /// <summary>
        /// Sets the pixel value for a 16bpp image.
        /// </summary>
#if Net45
      	[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static void SetDataTwoByte(uint* data, int index, uint value)
		{
#if LittleEndian
    #if X64
    		*(ushort*)((ulong)((ushort*)data + index) ^ 2) = (ushort)value;
    #else
            *(ushort*)((uint)((ushort*)data + index) ^ 2) = (ushort)value;
    #endif
#else
            *((ushort*)data + index) = (ushort)value;
#endif	
		}


        /// <summary>
        /// Gets the pixel value for a 32bpp image.
        /// </summary>
#if Net45
      	[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static uint GetDataFourByte(uint* data, int index)
		{
         	return *(data + index);		
		}


        /// <summary>
        /// Sets the pixel value for a 32bpp image.
        /// </summary>
#if Net45
      	[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static void SetDataFourByte(uint* data, int index, uint value)
		{
			*(data + index) = value;			
		}		
	}
}
