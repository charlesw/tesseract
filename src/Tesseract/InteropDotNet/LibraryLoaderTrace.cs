//  Copyright (c) 2014 Andrey Akinshin
//  Project URL: https://github.com/AndreyAkinshin/InteropDotNet
//  Distributed under the MIT License: http://opensource.org/licenses/MIT
using System;
using System.Diagnostics;
using System.Globalization;

namespace InteropDotNet
{
    internal static class LibraryLoaderTrace
    {
        private static bool printToConsole = false;

        private static void Print(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static void TraceInformation(string format, params object[] args)
        {
            if (printToConsole)
                Print(string.Format(CultureInfo.CurrentCulture, format, args));
            else
                Trace.TraceInformation(string.Format(CultureInfo.CurrentCulture, format, args));
        }

        public static void TraceError(string format, params object[] args)
        {
            if (printToConsole)
                Print(string.Format(CultureInfo.CurrentCulture, format, args));
            else
                Trace.TraceError(string.Format(CultureInfo.CurrentCulture, format, args));
        }

        public static void TraceWarning(string format, params object[] args)
        {
            if (printToConsole)
                Print(string.Format(CultureInfo.CurrentCulture, format, args));
            else
                Trace.TraceWarning(string.Format(CultureInfo.CurrentCulture, format, args));
        }
    }
}