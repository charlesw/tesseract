using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Tesseract.Internal;

namespace Tesseract
{
    public sealed class Page : DisposableBase
    {
        private bool runRecognitionPhase;
        private Rect regionOfInterest;

        public TesseractEngine Engine { get; private set; }

        public Pix Image { get; private set; }

        public string ImageName { get; private set; }

        public PageSegMode PageSegmentMode { get; private set; }

        internal Page(TesseractEngine engine, Pix image, string imageName, Rect regionOfInterest, PageSegMode pageSegmentMode)
        {
            Engine = engine;
            Image = image;
            ImageName = imageName;
            RegionOfInterest = regionOfInterest;
            PageSegmentMode = pageSegmentMode;
        }

        /// <summary>
        /// The current region of interest being parsed.
        /// </summary>
        public Rect RegionOfInterest
        {
            get
            {
                return regionOfInterest;
            }
            set
            {
                if (value.X1 < 0 || value.Y1 < 0 || value.X2 > Image.Width || value.Y2 > Image.Height)
                    throw new ArgumentException("The region of interest to be processed must be within the image bounds.", "value");

                if (regionOfInterest != value) {
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
            Guard.Verify(PageSegmentMode != PageSegMode.OsdOnly, "Cannot analyse image layout when using OSD only page segmentation, please use DetectBestOrientation instead.");

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

        public void DetectBestOrientation(out Orientation orientation, out float confidence)
        {
            Interop.OSResult result = new Interop.OSResult();
            result.Init();
            if (Interop.TessApi.Native.TessBaseAPIDetectOS(Engine.Handle, ref result) != 0) {
                result.GetBestOrientation(out orientation, out confidence);
            } else {
                throw new TesseractException("Failed to detect image orientation.");
            }
        }

#if Net45

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private void Recognize()
        {
            Guard.Verify(PageSegmentMode != PageSegMode.OsdOnly, "Cannot OCR image when using OSD only page segmentation, please use DetectBestOrientation instead.");
            if (!runRecognitionPhase) {
                if (Interop.TessApi.Native.BaseApiRecognize(Engine.Handle, new HandleRef(this, IntPtr.Zero)) != 0) {
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