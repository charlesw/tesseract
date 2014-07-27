using System;

namespace Tesseract.Internal
{
	static class ErrorMessage
	{
		private const string ErrorMessageFormat = "{0}. See {1} for details.";
		private const string WikiUrlFormat = "https://github.com/charlesw/tesseract/wiki/Error-{0}";
		
		public static string Format(int errorNumber, string messageFormat, params object[] messageArgs)
		{
			var errorMessage = String.Format(messageFormat, messageArgs);
			var errorPageUrl = ErrorPageUrl(errorNumber);
			return String.Format(ErrorMessageFormat, errorMessage, errorPageUrl);
		}
		
		public static string ErrorPageUrl(int errorNumber)
		{
			return String.Format(WikiUrlFormat, errorNumber);
		}
		
		
	}
}
