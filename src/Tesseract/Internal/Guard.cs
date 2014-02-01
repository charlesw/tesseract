
using System;
using System.Diagnostics;

namespace Tesseract.Internal
{
	internal static class Guard
	{
		// Generic pre-condition checks
		
        /// <summary>
	    /// Ensures the given <paramref name="condition"/> is true.
	    /// </summary>
	    /// <exception cref="System.ArgumentException">The <paramref name="condition"/> is not true.</exception>
	    /// <param name="paramName">The name of the parameter, used when generating the exception.</param>
	    /// <param name="condition">The value of the parameter to check.</param>
		[DebuggerHidden]
		public static void Require(string paramName, bool condition) 
		{
			if(!condition) throw new ArgumentException(string.Empty, paramName);
		}

		/// <summary>
	    /// Ensures the given <paramref name="condition"/> is true.
	    /// </summary>
	    /// <exception cref="System.ArgumentException">The <paramref name="condition"/> is not true.</exception>
	    /// <param name="paramName">The name of the parameter, used when generating the exception.</param>
	    /// <param name="condition">The value of the parameter to check.</param>
	    /// <param name="message">The error message.</param>
        [DebuggerHidden]
		public static void Require(string paramName, bool condition, string message) 
		{
			if(!condition) throw new ArgumentException(message, paramName);
		}

		/// <summary>
	    /// Ensures the given <paramref name="condition"/> is true.
	    /// </summary>
	    /// <exception cref="System.ArgumentException">The <paramref name="condition"/> is not true.</exception>
	    /// <param name="paramName">The name of the parameter, used when generating the exception.</param>
	    /// <param name="condition">The value of the parameter to check.</param>
	    /// <param name="message">The error message.</param>
	    /// <param name="args">The message argument used to format <paramref name="message" />.</param>
        [DebuggerHidden]
		public static void Require(string paramName, bool condition, string message, params object[] args)
		{
			if(!condition) throw new ArgumentException(String.Format(message, args), paramName);
		}
		
		
		
		[DebuggerHidden]
		public static void RequireNotNull(string argName, object value)
		{
			if(value == null) {
				throw new ArgumentException(String.Format("Argument \"{0}\" must not be null.", value));
			}
		}
		
		/// <summary>
	    /// Ensures the given <paramref name="value"/> is not null or empty.
	    /// </summary>
	    /// <exception cref="System.ArgumentException">The <paramref name="value"/> is null or empty.</exception>
	    /// <param name="paramName">The name of the parameter, used when generating the exception.</param>
	    /// <param name="value">The value of the parameter to check.</param>
        [DebuggerHidden]
        public static void RequireNotNullOrEmpty(string paramName, string value)
        {
        	RequireNotNull(paramName, value);
        	if (value.Length == 0) {
                throw new ArgumentException(paramName,
        		                                String.Format(@"The argument ""{0}"" must not be null or empty.", paramName));
            }
        }
	}
}
