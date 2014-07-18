using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Tesseract.Interop;

namespace Tesseract
{
	public unsafe class PixData
	{
		public Pix Pix { get; private set; }

		
		internal PixData(Pix pix)
		{		
            Pix = pix;
            Data = Interop.LeptonicaApi.Native.pixGetData(Pix.Handle);
            WordsPerLine = Interop.LeptonicaApi.Native.pixGetWpl(Pix.Handle);
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
        /// as one typically has in file formats, to one with MSB-to-the-left in each 32-bit word, or v.v. See <seealso href="http://www.leptonica.com/byte-addressing.html"/>
        /// </remarks>
        public void EndianByteSwap()
        {
            Interop.LeptonicaApi.Native.pixEndianByteSwap(Pix.Handle);
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
			// Must do direct size comparison to detect x64 process, since in this will be jited out and results in a lot faster code (e.g. 6x faster for image conversion)
			if(IntPtr.Size == 8) {
        		return *((byte*)((ulong)((byte*)data + index) ^ 3));
			} else {
            	return *((byte*)((uint)((byte*)data + index) ^ 3));
			}
			// Architecture types that are NOT little edian are not currently supported
            //return *((byte*)data + index);  
		}


        /// <summary>
        /// Sets the pixel value for a 8bpp image.
        /// </summary>
#if Net45
      	[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static void SetDataByte(uint* data, int index, uint value)
		{			
			// Must do direct size comparison to detect x64 process, since in this will be jited out and results in a lot faster code (e.g. 6x faster for image conversion)
			if(IntPtr.Size == 8) {
				*(byte*)((ulong)((byte*)data + index) ^ 3) = (byte)value;
			} else {
			 	*(byte*)((uint)((byte*)data + index) ^ 3) = (byte)value;
			}
			
			// Architecture types that are NOT little edian are not currently supported
			// *((byte*)data + index) =  (byte)value;
		}


        /// <summary>
        /// Gets the pixel value for a 16bpp image.
        /// </summary>
#if Net45
      	[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static uint GetDataTwoByte(uint* data, int index)
		{	
			// Must do direct size comparison to detect x64 process, since in this will be jited out and results in a lot faster code (e.g. 6x faster for image conversion)
			if(IntPtr.Size == 8) {
                return *(ushort*)((ulong)((ushort*)data + index) ^ 2);
			} else {
                return *(ushort*)((uint)((ushort*)data + index) ^ 2);
			}
			// Architecture types that are NOT little edian are not currently supported
            // return *((ushort*)data + index);
		}


        /// <summary>
        /// Sets the pixel value for a 16bpp image.
        /// </summary>
#if Net45
      	[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static void SetDataTwoByte(uint* data, int index, uint value)
		{
			// Must do direct size comparison to detect x64 process, since in this will be jited out and results in a lot faster code (e.g. 6x faster for image conversion)
			if(IntPtr.Size == 8) {
    			*(ushort*)((ulong)((ushort*)data + index) ^ 2) = (ushort)value;
			} else {
           		*(ushort*)((uint)((ushort*)data + index) ^ 2) = (ushort)value;
			}
			// Architecture types that are NOT little edian are not currently supported
            //*((ushort*)data + index) = (ushort)value;
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
