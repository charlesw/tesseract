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
        
        public FontAttributes GetWordFontAttributes() {
            VerifyNotDisposed();
            if (handle.Handle == IntPtr.Zero) {
                return null;
            }

            bool is_bold, is_italic, is_underlined,
                    is_monospace, is_serif, is_smallcaps;

            int pointsize, font_id;
            string name =
                Interop.TessApi.ResultIteratorWordFontAttributes(
                    handle,
                    out is_bold, out is_italic, out is_underlined,
                    out is_monospace, out is_serif, out is_smallcaps,
                    out pointsize, out font_id
                );

            // this can happen in certain error conditions.
            if (name == null) {
                return null;
            }

            var fontInfo =
                FontInfo.GetOrCreate(
                    name, font_id,
                    is_italic, is_bold,
                    is_monospace, is_serif
                );

            return new FontAttributes(fontInfo, is_underlined, is_smallcaps, pointsize);
        }

        public string GetWordRecognitionLanguage()
        {
            VerifyNotDisposed();
            if (handle.Handle == IntPtr.Zero) {
                return null;
            }

            return Interop.TessApi.ResultIteratorWordRecognitionLanguage(handle);
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
