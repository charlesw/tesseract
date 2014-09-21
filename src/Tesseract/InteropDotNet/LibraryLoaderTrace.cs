//  Copyright (c) 2014 Andrey Akinshin
//  Project URL: https://github.com/AndreyAkinshin/InteropDotNet
//  Distributed under the MIT License: http://opensource.org/licenses/MIT
using System;
using System.Diagnostics;
using System.Globalization;

namespace InteropDotNet
{
    static class LibraryLoaderTrace
    {
        const bool printToConsole = false;
		readonly static TraceSource trace = new TraceSource("Tesseract");

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
                trace.TraceEvent(TraceEventType.Information, 0, string.Format(CultureInfo.CurrentCulture, format, args));
        }

        public static void TraceError(string format, params object[] args)
        {
            if (printToConsole)
                Print(string.Format(CultureInfo.CurrentCulture, format, args));
            else
                trace.TraceEvent(TraceEventType.Error, 0, string.Format(CultureInfo.CurrentCulture, format, args));
        }

        public static void TraceWarning(string format, params object[] args)
        {
            if (printToConsole)
                Print(string.Format(CultureInfo.CurrentCulture, format, args));
            else
                trace.TraceEvent(TraceEventType.Warning, 0, string.Format(CultureInfo.CurrentCulture, format, args));
        }
    }
}