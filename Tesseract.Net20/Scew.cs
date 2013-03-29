using System;
using System.Collections.Generic;
using System.Text;

namespace Tesseract
{
    public struct Scew
    {
        private float angle;
        private float confidence;

        public Scew(float angle, float confidence)
        {
            this.angle = angle;
            this.confidence = confidence;
        }

        public float Angle
        {
            get { return angle; }
        }


        public float Confidence
        {
            get { return confidence; }
        }

        #region ToString

        public override string ToString()
        {
            return String.Format("Scew: {0} [conf: {1}]", Angle, Confidence);
        }

        #endregion

        #region Equals and GetHashCode implementation
        public override bool Equals(object obj)
        {
            return (obj is Scew) && Equals((Scew)obj);
        }

        public bool Equals(Scew other)
        {
            return this.confidence == other.confidence && this.angle == other.angle;
        }

        public override int GetHashCode()
        {
            int hashCode = 0;
            unchecked {
                hashCode += 1000000007 * angle.GetHashCode();
                hashCode += 1000000009 * confidence.GetHashCode();
            }
            return hashCode;
        }

        public static bool operator ==(Scew lhs, Scew rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Scew lhs, Scew rhs)
        {
            return !(lhs == rhs);
        }
        #endregion
        
    }
}
