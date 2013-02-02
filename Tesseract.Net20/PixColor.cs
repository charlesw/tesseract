using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Tesseract
{
    [StructLayout(LayoutKind.Sequential, Pack=1)]
	public struct PixColor : IEquatable<PixColor>
    {
        private byte red;
        private byte blue;
        private byte green;
        private byte alpha;

        public PixColor(byte red, byte green, byte blue, byte alpha = 255)
        {
            this.red = red;
            this.green = green;
            this.blue = blue;
            this.alpha = alpha;
        }

        public byte Red { get { return red; } }
        public byte Green { get { return green; } }
        public byte Blue { get { return blue; } }
        public byte Alpha { get { return alpha; } }

        public static PixColor FromRgba(uint value)
        {
            return new PixColor(
               (byte)((value >> 24) & 0xFF),
               (byte)((value >> 16) & 0xFF),
               (byte)((value >> 8) & 0xFF),
               (byte)(value & 0xFF));
        }

        public static PixColor FromRgb(uint value)
        {
            return new PixColor(
               (byte)((value >> 24) & 0xFF),
               (byte)((value >> 16) & 0xFF),
               (byte)((value >> 8) & 0xFF),
               (byte)0xFF);
        }

        public uint ToRGBA()
        {
            return BitmapHelper.EncodeAsRGBA(red, green, blue, alpha);
        }

        public static explicit operator System.Drawing.Color(PixColor color)
        {
            return System.Drawing.Color.FromArgb(color.alpha, color.red, color.green, color.blue);
        }

        public static explicit operator PixColor(System.Drawing.Color color)
        {
            return new PixColor(color.R, color.G, color.B, color.A);
        }


        #region Equals and GetHashCode implementation
        public override bool Equals(object obj)
		{
			return (obj is PixColor) && Equals((PixColor)obj);
		}
        
		public bool Equals(PixColor other)
		{
			return this.red == other.red && this.blue == other.blue && this.green == other.green && this.alpha == other.alpha;
		}
        
		public override int GetHashCode()
		{
			int hashCode = 0;
			unchecked {
				hashCode += 1000000007 * red.GetHashCode();
				hashCode += 1000000009 * blue.GetHashCode();
				hashCode += 1000000021 * green.GetHashCode();
				hashCode += 1000000033 * alpha.GetHashCode();
			}
			return hashCode;
		}
        
		public static bool operator ==(PixColor lhs, PixColor rhs)
		{
			return lhs.Equals(rhs);
		}
        
		public static bool operator !=(PixColor lhs, PixColor rhs)
		{
			return !(lhs == rhs);
		}
        #endregion

        public override string ToString()
        {
            return String.Format("Color(0x{0:X})", ToRGBA());
        }

    }
}
