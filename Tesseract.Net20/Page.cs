using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Tesseract
{
    public sealed class Page : DisposableBase
    {
        private bool runRecognitionPhase;

        public TesseractEngine Engine { get; private set; }

        internal Page(TesseractEngine engine)
        {
            Engine = engine;
        }

       
        public PageIterator AnalyseLayout()
        {
            var resultIteratorHandle = Interop.TessApi.BaseAPIAnalyseLayout(Engine.Handle);
            return new PageIterator(resultIteratorHandle);
        }

        public ResultIterator GetIterator()
        {
            Recognize();
            var resultIteratorHandle = Interop.TessApi.BaseApiGetIterator(Engine.Handle);
            return new ResultIterator(resultIteratorHandle);
        }

        public string GetText()
        {
            Recognize();
            return Interop.TessApi.BaseAPIGetUTF8Text(Engine.Handle);
        }

        public string GetHOCRText(int pageNum)
        {
            Recognize();
            return Interop.TessApi.BaseAPIGetHOCRText(Engine.Handle, pageNum);
        }

        public float GetMeanConfidence()
        {
            Recognize();
            return Interop.TessApi.BaseAPIMeanTextConf(Engine.Handle) / 100.0f;
        }


#if Net45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private void Recognize()
        {            
            if (!runRecognitionPhase) {
                if (Interop.TessApi.BaseApiRecognize(Engine.Handle, IntPtr.Zero) != 0) {
                    throw new InvalidOperationException("Recognition of image failed.");
                }
                runRecognitionPhase = true;
            }
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                Interop.TessApi.BaseAPIClear(Engine.Handle);
            }
        }
    }
}
