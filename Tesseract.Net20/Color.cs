using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Tesseract
{
    [StructLayout(LayoutKind.Sequential, Pack=1)]
	public struct Color : IEquatable<Color>
    {
        private byte red;
        private byte blue;
        private byte green;
        private byte alpha;

        public Color(byte red, byte green, byte blue, byte alpha = 255)
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

        public static Color FromRgba(uint value)
        {
            return new Color(
               (byte)((value >> 24) & 0xFF),
               (byte)((value >> 16) & 0xFF),
               (byte)((value >> 8) & 0xFF),
               (byte)(value & 0xFF));
        }

        public uint ToRGBA()
        {
            return BitmapHelper.EncodeAsRGBA(red, green, blue, alpha);
        }

        public static explicit operator System.Drawing.Color(Color color)
        {
            return System.Drawing.Color.FromArgb(color.alpha, color.red, color.green, color.blue);
        }

        public static explicit operator Color(System.Drawing.Color color)
        {
            return new Color(color.R, color.G, color.B, color.A);
        }


        #region Equals and GetHashCode implementation
        public override bool Equals(object obj)
		{
			return (obj is Color) && Equals((Color)obj);
		}
        
		public bool Equals(Color other)
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
        
		public static bool operator ==(Color lhs, Color rhs)
		{
			return lhs.Equals(rhs);
		}
        
		public static bool operator !=(Color lhs, Color rhs)
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
