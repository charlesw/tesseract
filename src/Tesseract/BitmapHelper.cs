using System;
using System.Runtime.CompilerServices;

namespace Tesseract
{
	/// <summary>
	/// Description of BitmapHelper.
	/// </summary>
	public static unsafe class BitmapHelper
    {        
        /// <summary>
        /// gets the number of Bits Per Pixel (BPP)
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static int GetBPP(System.Drawing.Bitmap bitmap)
        {
            switch (bitmap.PixelFormat) {
                case System.Drawing.Imaging.PixelFormat.Format1bppIndexed: return 1;
                case System.Drawing.Imaging.PixelFormat.Format4bppIndexed: return 4;
                case System.Drawing.Imaging.PixelFormat.Format8bppIndexed: return 8;
                case System.Drawing.Imaging.PixelFormat.Format16bppArgb1555:  
                case System.Drawing.Imaging.PixelFormat.Format16bppGrayScale:
                case System.Drawing.Imaging.PixelFormat.Format16bppRgb555:
                case System.Drawing.Imaging.PixelFormat.Format16bppRgb565: return 16;
                case System.Drawing.Imaging.PixelFormat.Format24bppRgb: return 24;
                case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
                case System.Drawing.Imaging.PixelFormat.Format32bppPArgb:
                case System.Drawing.Imaging.PixelFormat.Format32bppRgb: return 32;
                case System.Drawing.Imaging.PixelFormat.Format48bppRgb: return 48;
                case System.Drawing.Imaging.PixelFormat.Format64bppArgb:
                case System.Drawing.Imaging.PixelFormat.Format64bppPArgb: return 64;
                default: throw new ArgumentException(String.Format("The bitmap's pixel format of {0} was not recognised.", bitmap.PixelFormat), "bitmap");
            }
        }

        #region Bitmap Data Access

#if Net45
       	[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte GetDataBit(byte* data, int index)
		{			
			return (byte)((*(data + (index >> 3)) >> (index & 0x7)) & 1);
		}
		
		#if Net45
       	[MethodImpl(MethodImplOptions.AggressiveInlining)]
		#endif
        public static void SetDataBit(byte* data, int index, byte value)
		{			
			byte* wordPtr = data + (index >> 3);
            *wordPtr &= (byte)~(0x80 >> (index & 7)); 			// clear bit, note first pixel in the byte is most significant (1000 0000)
            *wordPtr |= (byte)((value & 1) << (7 - (index & 7)));		// set bit, if value is 1
		}
		
		#if Net45
       	[MethodImpl(MethodImplOptions.AggressiveInlining)]
		#endif
		public static byte GetDataQBit(byte* data, int index)
		{
            return (byte)((*(data + (index >> 1)) >> (4 * (index & 1))) & 0xF);
		}
		
		#if Net45
       	[MethodImpl(MethodImplOptions.AggressiveInlining)]
		#endif
		public static void SetDataQBit(byte* data, int index, byte value)
		{			
			byte* wordPtr = data + (index >> 1);
            *wordPtr &= (byte)~(0xF0 >> (4 * (index & 1))); // clears qbit located at index, note like bit the qbit corresponding to the first pixel is the most significant (0xF0)
            *wordPtr |= (byte)((value & 0x0F) << (4 - (4 * (index & 1)))); // applys qbit to n
		}

		#if Net45
       	[MethodImpl(MethodImplOptions.AggressiveInlining)]
		#endif
		public static byte GetDataByte(byte* data, int index)
		{			
			return *(data + index);
		}
		
		#if Net45
       	[MethodImpl(MethodImplOptions.AggressiveInlining)]
		#endif
		public static void SetDataByte(byte* data, int index, byte value)
		{			
			*(data + index) = value;
		}

		#if Net45
       	[MethodImpl(MethodImplOptions.AggressiveInlining)]
		#endif
		public static ushort GetDataUInt16(ushort* data, int index)
		{			
			return *(data + index);
		}
		
		#if Net45
       	[MethodImpl(MethodImplOptions.AggressiveInlining)]
		#endif
		public static void SetDataUInt16(ushort* data, int index, ushort value)
		{			
			*(data + index) = value;
		}	

#if Net45
       	[MethodImpl(MethodImplOptions.AggressiveInlining)]
		#endif
		public static uint GetDataUInt32(uint* data, int index)
		{			
			return *(data + index);
		}
		
		#if Net45
       	[MethodImpl(MethodImplOptions.AggressiveInlining)]
		#endif
		public static void SetDataUInt32(uint* data, int index, uint value)
		{			
			*(data + index) = value;
        }


        #endregion

        #region PixelFormat conversion

#if Net45
       	[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static uint ConvertRgb555ToRGBA(uint val)
        {
            var red = ((val & 0x7C00) >> 10);
            var green = ((val & 0x3E0) >> 5);
            var blue = (val & 0x1F);

            return ((red << 3 | red >> 2) << 24) |
                ((green << 3 | green >> 2) << 16) |
                ((blue << 3 | blue >> 2) << 8) |
                0xFF;
        }
        
#if Net45
       	[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static uint ConvertRgb565ToRGBA(uint val)
        {
            var red = ((val & 0xF800) >> 11);
            var green = ((val & 0x7E0) >> 5);
            var blue = (val & 0x1F);

            return ((red << 3 | red >> 2) << 24) |
                ((green << 2 | green >> 4) << 16) |
                ((blue << 3 | blue >> 2) << 8) |
                0xFF;
        }


#if Net45
       	[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static uint ConvertArgb1555ToRGBA(uint val)
        {
            var alpha = ((val & 0x8000) >> 15);
            var red = ((val & 0x7C00) >> 10);
            var green = ((val & 0x3E0) >> 5);
            var blue = (val & 0x1F);

            return ((red << 3 | red >> 2) << 24) |
                ((green << 3 | green >> 2) << 16) |
                ((blue << 3 | blue >> 2) << 8) |
                ((alpha << 8) - alpha); // effectively alpha * 255, only works as alpha will be either 0 or 1
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

        #endregion
    }
}
