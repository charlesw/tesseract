using System;
using System.Collections.Generic;
using System.Text;

namespace Tesseract
{
    public class MathHelper
    {
        /// <summary>
        /// Calculates the smallest integer greater than the quotant of dividend and divisor.
        /// </summary>
        /// <see href="http://stackoverflow.com/questions/921180/how-can-i-ensure-that-a-division-of-integers-is-always-rounded-up"/>
        public static int DivRoundUp(int dividend, int divisor)
        {
            var result = dividend / divisor;
            
            
            return (dividend % divisor != 0 && divisor > 0 == dividend > 0) ? result + 1 : result;
        }
    }
}
