using System;
using System.Collections.Generic;
using System.Text;

namespace Tesseract.Interop
{
    public unsafe static class MarshalHelper
    {
        public static string PtrToString(IntPtr handle, Encoding encoding)
        {
            var length = StrLength(handle);
            return new String((sbyte*)handle.ToPointer(), 0, length, encoding);
        }

        /// <summary>
        /// Gets the number of bytes in a null terminated byte array.
        /// </summary>
        public static int StrLength(IntPtr handle)
        {
            var ptr = (byte*)handle.ToPointer();
            int length = 0;
            while (*(ptr + length) != 0) {
                length++;
            }
            return length;
        }


    }
}
