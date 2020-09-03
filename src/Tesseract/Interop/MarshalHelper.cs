using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Tesseract.Interop
{
    unsafe internal static class MarshalHelper
    {
        private static readonly Encoding defaultEncoding = new UTF8Encoding(false);

        internal static IntPtr StringToPtr(string value, Encoding encoding = null)
        {
            encoding = encoding ?? defaultEncoding;
            var encoder = encoding.GetEncoder();
            var length = encoding.GetByteCount(value);
            // The encoded value is null terminated that's the reason for the '+1'.
            var encodedValue = new byte[length + 1];
            encoding.GetBytes(value, 0, value.Length, encodedValue, 0);
            var handle = Marshal.AllocHGlobal(new IntPtr(encodedValue.Length));
            Marshal.Copy(encodedValue, 0, handle, encodedValue.Length);
            return handle;
        }


        internal static string PtrToString(IntPtr handle, Encoding encoding = null)
        {
            encoding = encoding ?? defaultEncoding;
            if (IntPtr.Zero == handle) return null;
            var length = StrLength(handle);
            return new string((sbyte*)handle.ToPointer(), 0, length, encoding);
        }

        /// <summary>
        /// Gets the number of bytes in a null terminated byte array.
        /// </summary>
        internal static int StrLength(IntPtr handle)
        {
            if(handle == IntPtr.Zero)
            {
                return 0;
            }

            var ptr = (byte*)handle.ToPointer();
            int length = 0;
            while (*(ptr + length) != 0) {
                length++;
            }
            return length;
        }
    }
}
