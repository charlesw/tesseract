using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Tesseract
{
    public class SymbolResult : ResultBase
    {
        public const PageIteratorLevel Level = PageIteratorLevel.Symbol;

        /// <summary>
        /// Gets the choices for this symbol.
        /// </summary>
        public readonly Choice[] Choices;

        public readonly bool IsSuperscript;
        public readonly bool IsSubscript;
        public readonly bool IsDropcap;

        internal SymbolResult(Iterator.ResultIterator iterator)
            : base(iterator)
        {
            if (Iterator._Handle.Handle == IntPtr.Zero)
            {
                Choices = new Choice[0];
                IsSuperscript = false;
                IsSubscript = false;
                IsDropcap = false;
            }
            else
            {
                Choices = new List<Choice>(new Iterator.ChoiceIterator(Iterator._Handle)).ToArray();
                IsSuperscript = Interop.TessApi.Native.ResultIteratorSymbolIsSuperscript(Iterator._Handle);
                IsSubscript = Interop.TessApi.Native.ResultIteratorSymbolIsSubscript(Iterator._Handle);
                IsDropcap = Interop.TessApi.Native.ResultIteratorSymbolIsDropcap(Iterator._Handle);
            }
        }
    }
}
