using System;

namespace Tesseract.Interop
{
	/// <summary>
	/// Provides information about the hosting process.
	/// </summary>
	static class HostProcessInfo
	{
		public static readonly bool Is64Bit;
		
		static HostProcessInfo() {
			Is64Bit = IntPtr.Size == 8;
		}
	}
}
