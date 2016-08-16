using System;

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

            return Interop.TessApi.Native.ResultIteratorGetConfidence(handle, level);
        }

        public string GetText(PageIteratorLevel level)
        {
            VerifyNotDisposed();
            if (handle.Handle == IntPtr.Zero) {
                return String.Empty;
            }

            return Interop.TessApi.ResultIteratorGetUTF8Text(handle, level);
        }

        /// <summary>
        /// Returns the name of the language used to recognize this word.
        /// </summary>
        public string GetWordRecognitionLanguage()
        {
            VerifyNotDisposed();
            if (handle.Handle == IntPtr.Zero) {
                return String.Empty;
            }

            return Interop.TessApi.ResultIteratorGetWordRecognitionLanguage(handle);
        }

        /// <summary>
        /// Returns the font attributes of the current word. If iterating at a higher
        /// level object than words, e.g. text lines, then this will return the
        /// attributes of the first word in that textline.
        /// </summary>
        /// <param name="isBold"><c>true</c> if the font is bold.</param>
        /// <param name="isItalic"><c>true</c> if the font is italic.</param>
        /// <param name="isUnderlined"><c>true</c> if the font is underlined.</param>
        /// <param name="isMonospace"><c>true</c> if the font is a fixed pitch font.</param>
        /// <param name="isSerif"><c>true</c> if the font has serifs.</param>
        /// <param name="isSmallCaps"><c>true</c> if the characters are small capitals.</param>
        /// <param name="pointSize">The point size is returned in printer's points (1/72 inch).</param>
        /// <param name="fontId">The univeral font ID.</param>
        /// <returns>A string representing a font name.</returns>
        public string GetWordFontAttributes(
                out bool isBold, out bool isItalic, out bool isUnderlined,
                out bool isMonospace, out bool isSerif, out bool isSmallCaps,
                out int pointSize, out int fontId)
        {
            VerifyNotDisposed();
            if (handle.Handle == IntPtr.Zero) {
                isBold = false;
                isItalic = false;
                isUnderlined = false;
                isMonospace = false;
                isSerif = false;
                isSmallCaps = false;
                pointSize = 0;
                fontId = -1;
                return String.Empty;
            }
            return Interop.TessApi.ResultIteratorGetWordFontAttributes(handle,
                    out isBold, out isItalic, out isUnderlined,
                    out isMonospace, out isSerif, out isSmallCaps,
                    out pointSize, out fontId);
        }

        /// <summary>
        ///  Returns true if the current word was found in a dictionary.
        /// </summary>
        public bool WordIsFromDictionary()
        {
            VerifyNotDisposed();
            if (handle.Handle == IntPtr.Zero) {
                return false;
            }
            return Interop.TessApi.Native.ResultIteratorWordIsFromDictionary(handle) != 0;
        }

        /// <summary>
        /// Returns <c>true</c> if the current word is numeric.
        /// </summary>
        public bool WordIsNumeric()
        {
            VerifyNotDisposed();
            if (handle.Handle == IntPtr.Zero) {
                return false;
            }
            return Interop.TessApi.Native.ResultIteratorWordIsNumeric(handle) != 0;
        }

        /// <summary>
        /// Returns <c>true</c> if the current symbol is a superscript.
        /// If iterating at a higher level object than symbols, e.g. words, then
        /// this will return the attributes of the first symbol in that word.
        /// </summary>
        public bool SymbolIsSuperscript()
        {
            VerifyNotDisposed();
            if (handle.Handle == IntPtr.Zero) {
                return false;
            }
            return Interop.TessApi.Native.ResultIteratorSymbolIsSuperscript(handle) != 0;
        }

        /// <summary>
        /// Returns <c>true</c> if the current symbol is a subscript.
        /// If iterating at a higher level object than symbols, e.g. words, then
        /// this will return the attributes of the first symbol in that word.
        /// </summary>
        public bool SymbolIsSubscript()
        {
            VerifyNotDisposed();
            if (handle.Handle == IntPtr.Zero) {
                return false;
            }
            return Interop.TessApi.Native.ResultIteratorSymbolIsSubscript(handle) != 0;
        }

        /// <summary>
        /// Returns <c>true</c> if the current symbol is a dropcap.
        /// If iterating at a higher level object than symbols, e.g. words, then
        /// this will return the attributes of the first symbol in that word.
        /// </summary>
        public bool SymbolIsDropcap()
        {
            VerifyNotDisposed();
            if (handle.Handle == IntPtr.Zero) {
                return false;
            }
            return Interop.TessApi.Native.ResultIteratorSymbolIsDropcap(handle) != 0;
        }

        /// <summary>
        /// Gets an instance of a choice iterator using the current symbol of interest. The ChoiceIterator allows a one-shot iteration over the
        /// choices for this symbol and after that is is useless.
        /// </summary>
        /// <returns>an instance of a Choice Iterator</returns>
        public ChoiceIterator GetChoiceIterator()
        {
            var choiceIteratorHandle = Interop.TessApi.Native.ResultIteratorGetChoiceIterator(this.handle);
            if (choiceIteratorHandle == IntPtr.Zero)
                return null;
            return new ChoiceIterator(choiceIteratorHandle);
        }
    }
}
