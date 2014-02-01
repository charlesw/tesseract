
using System;
using System.Globalization;

namespace Tesseract.Internal
{
	/// <summary>
	/// Utility helpers to handle converting variable values.
	/// </summary>
	internal static class TessConvert
	{
		public static bool TryToString(object value, out string result) 
		{			
			if(value is bool) {
				result = ToString((bool)value);
			} else if(value is decimal) {
				result = ToString((decimal)value);
			} else if(value is double) {
				result = ToString((double)value);
			} else if(value is float) {
				result = ToString((float)value);
			} else if(value is Int16) {
				result = ToString((Int16)value);
			} else if(value is Int32) {
				result = ToString((Int32)value);
			} else if(value is Int64) {
				result = ToString((Int64)value);			
			} else if(value is UInt16) {
				result = ToString((UInt16)value);
			} else if(value is UInt32) {
				result = ToString((UInt32)value);
			} else if(value is UInt64) {
				result = ToString((UInt64)value);
			} else if(value is string) {
				result = (string)value;
			} else {
				result = null;
				return false;
			}
			return true;
		}
		
		public static string ToString(bool value)
		{
			return value ? "TRUE" : "FALSE";
		}
		
		public static string ToString(decimal value)
		{
			return value.ToString("R", CultureInfo.InvariantCulture.NumberFormat);
		}
		
		public static string ToString(double value)
		{
			return value.ToString("R", CultureInfo.InvariantCulture.NumberFormat);
		}
		
		public static string ToString(float value)
		{
			return value.ToString("R", CultureInfo.InvariantCulture.NumberFormat);
		}
		
		public static string ToString(Int16 value)
		{
			return value.ToString("D", CultureInfo.InvariantCulture.NumberFormat);
		}
		
		public static string ToString(Int32 value)
		{
			return value.ToString("D", CultureInfo.InvariantCulture.NumberFormat);
		}		
		
		public static string ToString(Int64 value)
		{
			return value.ToString("D", CultureInfo.InvariantCulture.NumberFormat);
		}
		
		public static string ToString(UInt16 value)
		{
			return value.ToString("D", CultureInfo.InvariantCulture.NumberFormat);
		}
		
		public static string ToString(UInt32 value)
		{
			return value.ToString("D", CultureInfo.InvariantCulture.NumberFormat);
		}		
		
		public static string ToString(UInt64 value)
		{
			return value.ToString("D", CultureInfo.InvariantCulture.NumberFormat);
		}
	}
}
