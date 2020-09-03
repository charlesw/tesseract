using System;
using System.Collections.Generic;
using System.Text;

using Tesseract.Interop;

namespace Tesseract
{
    public sealed class ResultIterator : PageIterator
    {
        internal ResultIterator(Page page, IntPtr handle)
            : base(page, handle)
        {
        }

        public float GetConfidence(PageIteratorLevel level)
        {
            VerifyNotDisposed();
            if (handle.Handle == IntPtr.Zero)
                return 0f;

            return Interop.TessApiSignatures.ResultIteratorGetConfidence(handle, level);
        }

        public string GetText(PageIteratorLevel level)
        {
            VerifyNotDisposed();
            if (handle.Handle == IntPtr.Zero) {
                return String.Empty;
            }

            return Interop.TessApiSignatures.ResultIteratorGetUTF8Text(handle, level);
        }
        
        private Dictionary<int, FontInfo> _fontInfoCache = new Dictionary<int, FontInfo>();
        public FontAttributes GetWordFontAttributes() {
            VerifyNotDisposed();
            if (handle.Handle == IntPtr.Zero) {
                return null;
            }

            bool isBold, isItalic, isUnderlined, isMonospace, isSerif, isSmallCaps;
            int pointSize, fontId;

            // per docs (ltrresultiterator.h:104 as of 4897796 in github:tesseract-ocr/tesseract)
            // this return value points to an internal table and should not be deleted.
            string name =
                Interop.TessApiSignatures.ResultIteratorWordFontAttributes(
                    handle,
                    out isBold, out isItalic, out isUnderlined,
                    out isMonospace, out isSerif, out isSmallCaps,
                    out pointSize, out fontId);

            // This can happen in certain error conditions
            if (null == name) {
                return null;
            }

            FontInfo fontInfo;
            if (!_fontInfoCache.TryGetValue(fontId, out fontInfo)) {
                fontInfo = new FontInfo(name, fontId, isItalic, isBold, isMonospace, isSerif);
                _fontInfoCache.Add(fontId, fontInfo);
            }

            return new FontAttributes(fontInfo, isUnderlined, isSmallCaps, pointSize);
        }

        public string GetWordRecognitionLanguage()
        {
            VerifyNotDisposed();
            if (handle.Handle == IntPtr.Zero) {
                return null;
            }

            return Interop.TessApiSignatures.ResultIteratorWordRecognitionLanguage(handle);
        }

        public bool GetWordIsFromDictionary()
        {
            VerifyNotDisposed();
            if (handle.Handle == IntPtr.Zero) {
                return false;
            }

            return Interop.TessApiSignatures.ResultIteratorWordIsFromDictionary(handle);
        }

        public bool GetWordIsNumeric()
        {
            VerifyNotDisposed();
            if (handle.Handle == IntPtr.Zero) {
                return false;
            }

            return Interop.TessApiSignatures.ResultIteratorWordIsNumeric(handle);
        }

        public bool GetSymbolIsSuperscript()
        {
            VerifyNotDisposed();
            if (handle.Handle == IntPtr.Zero) {
                return false;
            }

            return Interop.TessApiSignatures.ResultIteratorSymbolIsSuperscript(handle);
        }

        public bool GetSymbolIsSubscript()
        {
            VerifyNotDisposed();
            if (handle.Handle == IntPtr.Zero) {
                return false;
            }

            return Interop.TessApiSignatures.ResultIteratorSymbolIsSubscript(handle);
        }

        public bool GetSymbolIsDropcap()
        {
            VerifyNotDisposed();
            if (handle.Handle == IntPtr.Zero) {
                return false;
            }

            return Interop.TessApiSignatures.ResultIteratorSymbolIsDropcap(handle);
        }

        /// <summary>
        /// Gets an instance of a choice iterator using the current symbol of interest. The ChoiceIterator allows a one-shot iteration over the
        /// choices for this symbol and after that is is useless.
        /// </summary>
        /// <returns>an instance of a Choice Iterator</returns>
        public ChoiceIterator GetChoiceIterator()
        {
            var choiceIteratorHandle = Interop.TessApiSignatures.ResultIteratorGetChoiceIterator(this.handle);
            if (choiceIteratorHandle == IntPtr.Zero)
                return null;
            return new ChoiceIterator(choiceIteratorHandle);
        }
    }
}
