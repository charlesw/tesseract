
using System;
using System.Diagnostics;

namespace Tesseract.Internal
{
	internal static class Guard
	{
		[DebuggerHidden]
		public static void RequireNotNull(string argName, object value)
		{
			if(value == null) {
				throw new ArgumentException(String.Format("Argument {0} must not be null.", value));
			}
		}
		
		[DebuggerHidden]
		public static void RequireNotNullOrEmpty(string argName, string value)
		{
			if(!String.IsNullOrEmpty(value)) {
				throw new ArgumentException(String.Format("Argument {0} must not be null.", value));
			}
		}
	}
}
