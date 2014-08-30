using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Tesseract
{
    public sealed class Page : DisposableBase
    {
        bool runRecognitionPhase;
		Rect regionOfInterest;
		
        public TesseractEngine Engine { get; private set; }
		public Pix Image { get; private set; }

        internal Page(TesseractEngine engine, Pix image, Rect regionOfInterest)
        {
            Engine = engine;
			Image = image;
			RegionOfInterest = regionOfInterest;
        }
		
		/// <summary>
		/// The current region of interest being parsed.
		/// </summary>
		public Rect RegionOfInterest {
			get {
				return regionOfInterest;
			}
			set {
				if (value.X1 < 0 || value.Y1 < 0 || value.X2 > Image.Width || value.Y2 > Image.Height)
                	throw new ArgumentException("The region of interest to be processed must be within the image bounds.", "value");
				
				if(regionOfInterest != value) {					
					regionOfInterest = value;
					
					// update region of interest in image
                    Interop.TessApi.Native.BaseApiSetRectangle(Engine.Handle, regionOfInterest.X1, regionOfInterest.Y1, regionOfInterest.Width, regionOfInterest.Height);
					
					// request rerun of recognition on the next call that requires recognition
					runRecognitionPhase = false;
				}
			}
		}
       
        public PageIterator AnalyseLayout()
        {
            var resultIteratorHandle = Interop.TessApi.Native.BaseAPIAnalyseLayout(Engine.Handle);
            return new PageIterator(resultIteratorHandle);
        }

        public ResultIterator GetIterator()
        {
            Recognize();
            var resultIteratorHandle = Interop.TessApi.Native.BaseApiGetIterator(Engine.Handle);
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
            return Interop.TessApi.Native.BaseAPIMeanTextConf(Engine.Handle) / 100.0f;
        }


#if Net45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private void Recognize()
        {            
            if (!runRecognitionPhase) {
                if (Interop.TessApi.Native.BaseApiRecognize(Engine.Handle, new HandleRef(this, IntPtr.Zero)) != 0)
                {
                    throw new InvalidOperationException("Recognition of image failed.");
                }
                runRecognitionPhase = true;
            }
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                Interop.TessApi.Native.BaseAPIClear(Engine.Handle);
            }
        }
    }
}
