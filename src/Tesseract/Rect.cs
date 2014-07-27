using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Tesseract
{
    public struct Rect : IEquatable<Rect>
    {
        public static readonly Rect Empty = new Rect();

        #region Fields

        private int x;
        private int y;
        private int width;
        private int height;

        #endregion

        #region Constructors + Factory Methods

        public Rect(int x, int y, int width, int height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public static Rect FromCoords(int x1, int y1, int x2, int y2)
        {
            return new Rect(x1, y1, x2 - x1, y2 - y1);
        }

        #endregion
        
        #region Properties

        public int X1
        {
            get { return x; }
        }

        public int Y1
        {
            get { return y; }
        }

        public int X2
        {
            get { return x + width; }
        }

        public int Y2
        {
            get { return y + height; }
        }

        public int Width
        {
            get { return width; }
        }

        public int Height
        {
            get { return height; }
        }

        #endregion

        #region Equals and GetHashCode implementation
        public override bool Equals(object obj)
        {
            return (obj is Rect) && Equals((Rect)obj);
        }

        public bool Equals(Rect other)
        {
            return this.x == other.x && this.y == other.y && this.width == other.width && this.height == other.height;
        }

        public override int GetHashCode()
        {
            int hashCode = 0;
            unchecked {
                hashCode += 1000000007 * x.GetHashCode();
                hashCode += 1000000009 * y.GetHashCode();
                hashCode += 1000000021 * width.GetHashCode();
                hashCode += 1000000033 * height.GetHashCode();
            }
            return hashCode;
        }

        public static bool operator ==(Rect lhs, Rect rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Rect lhs, Rect rhs)
        {
            return !(lhs == rhs);
        }
        #endregion
        
        #region ToString
        
		public override string ToString()
		{
			return string.Format("[Rect X={0}, Y={1}, Width={2}, Height={3}]", x, y, width, height);
		}

        
        #endregion
        
    }
}
