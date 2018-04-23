using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Tesseract.Interop;

namespace Tesseract
{
    public class WordResult : ResultBase, IEnumerable<SymbolResult>
    {
        public const PageIteratorLevel Level = PageIteratorLevel.Word;
        public readonly FontAttributes FontAttributes;
        public readonly string RecognitionLanguage;
        public readonly bool IsFromDictionary;
        public readonly bool IsNumeric;

        private static Dictionary<int, FontInfo> _fontInfoCache = new Dictionary<int, FontInfo>();
        private FontAttributes GetWordFontAttributes(HandleRef handle)
        {
            bool isBold, isItalic, isUnderlined, isMonospace, isSerif, isSmallCaps;
            int pointSize, fontId;

            // per docs (ltrresultiterator.h:104 as of 4897796 in github:tesseract-ocr/tesseract)
            // this return value points to an internal table and should not be deleted.
            IntPtr nameHandle =
                Interop.TessApi.Native.ResultIteratorWordFontAttributes(
                    handle,
                    out isBold, out isItalic, out isUnderlined,
                    out isMonospace, out isSerif, out isSmallCaps,
                    out pointSize, out fontId);

            // This can happen in certain error conditions
            if (nameHandle == IntPtr.Zero)
            {
                return null;
            }

            FontInfo fontInfo;
            if (!_fontInfoCache.TryGetValue(fontId, out fontInfo))
            {
                string fontName = MarshalHelper.PtrToString(nameHandle, Encoding.UTF8);
                fontInfo = new FontInfo(fontName, fontId, isItalic, isBold, isMonospace, isSerif);
                _fontInfoCache.Add(fontId, fontInfo);
            }

            return new FontAttributes(fontInfo, isUnderlined, isSmallCaps, pointSize);
        }

        internal WordResult(Iterator.ResultIterator iterator)
            : base(iterator)
        {
            if (Iterator._Handle.Handle == IntPtr.Zero)
            {
                FontAttributes = null;
                RecognitionLanguage = string.Empty;
                IsFromDictionary = false;
                IsNumeric = false;
            }
            else
            {
                FontAttributes = GetWordFontAttributes(Iterator._Handle);
                RecognitionLanguage = Interop.TessApi.ResultIteratorWordRecognitionLanguage(Iterator._Handle);
                IsFromDictionary = Interop.TessApi.Native.ResultIteratorWordIsFromDictionary(Iterator._Handle);
                IsNumeric = Interop.TessApi.Native.ResultIteratorWordIsNumeric(Iterator._Handle);
            }
        }

        public IEnumerator<SymbolResult> GetEnumerator() => new Iterator.GenericResultSubIterator<SymbolResult>(Iterator);
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerator<SymbolResult>)this);
    }
}