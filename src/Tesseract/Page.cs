using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Tesseract.Internal;
using Tesseract.Interop;

namespace Tesseract
{
    public sealed class Page : DisposableBase
    {
        private static readonly TraceSource trace = new TraceSource("Tesseract");

        private bool runRecognitionPhase;
        private Rect regionOfInterest;

        public TesseractEngine Engine { get; private set; }

        /// <summary>
        /// Gets the <see cref="Pix"/> that is being ocr'd.
        /// </summary>
        public Pix Image { get; private set; }

        /// <summary>
        /// Gets the name of the image being ocr'd.
        /// </summary>
        /// <remarks>
        /// This is also used for some of the more advanced functionality such as identifying the associated UZN file if present.
        /// </remarks>
        public string ImageName { get; private set; }

        /// <summary>
        /// Gets the page segmentation mode used to OCR the specified image.
        /// </summary>
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

        /// <summary>
        /// Gets the thresholded image that was OCR'd.
        /// </summary>
        /// <returns></returns>
        public Pix GetThresholdedImage()
        {
            Recognize();

            var pixHandle = Interop.TessApi.Native.BaseAPIGetThresholdedImage(Engine.Handle);
            if (pixHandle == IntPtr.Zero) {
                throw new TesseractException("Failed to get thresholded image.");
            }

            return Pix.Create(pixHandle);
        }

        /// <summary>
        /// Creates a <see cref="PageIterator"/> object that is used to iterate over the page's layout as defined by the current <see cref="Page.RegionOfInterest"/>.
        /// </summary>
        /// <returns></returns>
        public PageIterator AnalyseLayout()
        {
            Guard.Verify(PageSegmentMode != PageSegMode.OsdOnly, "Cannot analyse image layout when using OSD only page segmentation, please use DetectBestOrientation instead.");

            var resultIteratorHandle = Interop.TessApi.Native.BaseAPIAnalyseLayout(Engine.Handle);
            return new PageIterator(this, resultIteratorHandle);
        }

        /// <summary>
        /// Creates a <see cref="ResultIterator"/> object that is used to iterate over the page as defined by the current <see cref="Page.RegionOfInterest"/>.
        /// </summary>
        /// <returns></returns>
        public ResultIterator GetIterator()
        {
            Recognize();
            var resultIteratorHandle = Interop.TessApi.Native.BaseApiGetIterator(Engine.Handle);
            return new ResultIterator(this, resultIteratorHandle);
        }

        /// <summary>
        /// Gets the page's content as plain text.
        /// </summary>
        /// <returns></returns>
        public string GetText()
        {
            Recognize();
            return Interop.TessApi.BaseAPIGetUTF8Text(Engine.Handle);
        }

        /// <summary>
        /// Gets the page's content as an HOCR text.
        /// </summary>
        /// <param name="pageNum">The page number (zero based).</param>
        /// <param name="useXHtml">True to use XHTML Output, False to HTML Output</param>
        /// <returns>The OCR'd output as an HOCR text string.</returns>
        public string GetHOCRText(int pageNum, bool useXHtml = false)
        {
            //Why Not Use 'nameof(pageNum)' instead of '"pageNum"'
            Guard.Require("pageNum", pageNum >= 0, "Page number must be greater than or equal to zero (0).");
            Recognize();
            if(useXHtml)
                return Interop.TessApi.BaseAPIGetHOCRText2(Engine.Handle, pageNum);
            else
                return Interop.TessApi.BaseAPIGetHOCRText(Engine.Handle, pageNum);
        }

        /// <summary>
        /// Gets the page's content as an Alto text.
        /// </summary>
        /// <param name="pageNum">The page number (zero based).</param>
        /// <returns>The OCR'd output as an Alto text string.</returns>
        public string GetAltoText(int pageNum)
        {
            Guard.Require("pageNum", pageNum >= 0, "Page number must be greater than or equal to zero (0).");
            Recognize();
            return Interop.TessApi.BaseAPIGetAltoText(Engine.Handle, pageNum);
        }

        /// <summary>
        /// Gets the page's content as a Tsv text.
        /// </summary>
        /// <param name="pageNum">The page number (zero based).</param>
        /// <returns>The OCR'd output as a Tsv text string.</returns>
        public string GetTsvText(int pageNum)
        {
            Guard.Require("pageNum", pageNum >= 0, "Page number must be greater than or equal to zero (0).");
            Recognize();
            return Interop.TessApi.BaseAPIGetTsvText(Engine.Handle, pageNum);
        }

        /// <summary>
        /// Gets the page's content as a Box text.
        /// </summary>
        /// <param name="pageNum">The page number (zero based).</param>
        /// <returns>The OCR'd output as a Box text string.</returns>
        public string GetBoxText(int pageNum)
        {
            Guard.Require("pageNum", pageNum >= 0, "Page number must be greater than or equal to zero (0).");
            Recognize();
            return Interop.TessApi.BaseAPIGetBoxText(Engine.Handle, pageNum);
        }

        /// <summary>
        /// Gets the page's content as a LSTMBox text.
        /// </summary>
        /// <param name="pageNum">The page number (zero based).</param>
        /// <returns>The OCR'd output as a LSTMBox text string.</returns>
        public string GetLSTMBoxText(int pageNum)
        {
            Guard.Require("pageNum", pageNum >= 0, "Page number must be greater than or equal to zero (0).");
            Recognize();
            return Interop.TessApi.BaseAPIGetLSTMBoxText(Engine.Handle, pageNum);
        }

        /// <summary>
        /// Gets the page's content as a WordStrBox text.
        /// </summary>
        /// <param name="pageNum">The page number (zero based).</param>
        /// <returns>The OCR'd output as a WordStrBox text string.</returns>
        public string GetWordStrBoxText(int pageNum)
        {
            Guard.Require("pageNum", pageNum >= 0, "Page number must be greater than or equal to zero (0).");
            Recognize();
            return Interop.TessApi.BaseAPIGetWordStrBoxText(Engine.Handle, pageNum);
        }

        /// <summary>
        /// Gets the page's content as an UNLV text.
        /// </summary>
        /// <param name="pageNum">The page number (zero based).</param>
        /// <returns>The OCR'd output as an UNLV text string.</returns>
        public string GetUNLVText()
        {
            Recognize();
            return Interop.TessApi.BaseAPIGetUNLVText(Engine.Handle);
        }

        /// <summary>
        /// Get's the mean confidence that as a percentage of the recognized text.
        /// </summary>
        /// <returns></returns>
        public float GetMeanConfidence()
        {
            Recognize();
            return Interop.TessApi.Native.BaseAPIMeanTextConf(Engine.Handle) / 100.0f;
        }

        /// <summary>
        /// Get segmented regions at specified page iterator level.
        /// </summary>
        /// <param name="pageIteratorLevel">PageIteratorLevel enum</param>
        /// <returns></returns>
        public List<Rectangle> GetSegmentedRegions(PageIteratorLevel pageIteratorLevel)
        {
            var boxArray = Interop.TessApi.Native.BaseAPIGetComponentImages(Engine.Handle, pageIteratorLevel, Interop.Constants.TRUE, IntPtr.Zero, IntPtr.Zero);
            int boxCount = Interop.LeptonicaApi.Native.boxaGetCount(new HandleRef(this, boxArray));

            List<Rectangle> boxList = new List<Rectangle>();

            for (int i = 0; i < boxCount; i++) {
                var box = Interop.LeptonicaApi.Native.boxaGetBox(new HandleRef(this, boxArray), i, PixArrayAccessType.Clone);
                if (box == IntPtr.Zero) {
                    continue;
                }

                int px, py, pw, ph;
                Interop.LeptonicaApi.Native.boxGetGeometry(new HandleRef(this, box), out px, out py, out pw, out ph);
                boxList.Add(new Rectangle(px, py, pw, ph));
                Interop.LeptonicaApi.Native.boxDestroy(ref box);
            }

            Interop.LeptonicaApi.Native.boxaDestroy(ref boxArray);

            return boxList;
        }

        /// <summary>
        /// Detects the page orientation, with corresponding confidence when using <see cref="PageSegMode.OsdOnly"/>.
        /// </summary>
        /// <remarks>
        /// If using full page segmentation mode (i.e. AutoOsd) then consider using <see cref="AnalyseLayout"/> instead as this also provides a
        /// deskew angle which isn't available when just performing orientation detection.
        /// </remarks>
        /// <param name="orientation">The page orientation.</param>
        /// <param name="confidence">The confidence level of the orientation (15 is reasonably confident).</param>
        /// 
        [Obsolete("Use DetectBestOrientation(int orientationDegrees, float confidence) that returns orientation in degrees instead.")]
        public void DetectBestOrientation(out Orientation orientation, out float confidence)
        {
            int orientationDegrees;
            float orientationConfidence;
            DetectBestOrientation(out orientationDegrees, out orientationConfidence);

            // convert angle to 0-360 (shouldn't be required but do it just o be safe).
            orientationDegrees = orientationDegrees % 360;
            if (orientationDegrees < 0) {
                orientationDegrees += 360;
            }

            if (orientationDegrees > 315 || orientationDegrees <= 45) {
                orientation = Orientation.PageUp;
            } else if (orientationDegrees > 45 && orientationDegrees <= 135) {
                orientation = Orientation.PageRight;
            } else if (orientationDegrees > 135 && orientationDegrees <= 225) {
                orientation = Orientation.PageDown;
            } else {
                orientation = Orientation.PageLeft;
            }

            confidence = orientationConfidence;
        }

        /// <summary>
        /// Detects the page orientation, with corresponding confidence when using <see cref="PageSegMode.OsdOnly"/>.
        /// </summary>
        /// <remarks>
        /// If using full page segmentation mode (i.e. AutoOsd) then consider using <see cref="AnalyseLayout"/> instead as this also provides a
        /// deskew angle which isn't available when just performing orientation detection.
        /// </remarks>
        /// <param name="orientation">The detected clockwise page rotation in degrees (0, 90, 180, or 270).</param>
        /// <param name="confidence">The confidence level of the orientation (15 is reasonably confident).</param>
        public void DetectBestOrientation(out int orientation, out float confidence)
        {
            string scriptName;
            float scriptConfidence;
            DetectBestOrientationAndScript(out orientation, out confidence, out scriptName, out scriptConfidence);
        }


        /// <summary>
        /// Detects the page orientation, with corresponding confidence when using <see cref="PageSegMode.OsdOnly"/>.
        /// </summary>
        /// <remarks>
        /// If using full page segmentation mode (i.e. AutoOsd) then consider using <see cref="AnalyseLayout"/> instead as this also provides a
        /// deskew angle which isn't available when just performing orientation detection.
        /// </remarks>
        /// <param name="orientation">The detected clockwise page rotation in degrees (0, 90, 180, or 270).</param>
        /// <param name="confidence">The confidence level of the orientation (15 is reasonably confident).</param>
        /// <param name="scriptName">The name of the script (e.g. Latin)<param>
        /// <param name="scriptConfidence">The confidence level in the script</param>
        public void DetectBestOrientationAndScript(out int orientation, out float confidence, out string scriptName, out float scriptConfidence)
        {
            int orient_deg;
            float orient_conf;
            IntPtr script_nameHandle;
            float script_conf;

            if (Interop.TessApi.Native.TessBaseAPIDetectOrientationScript(Engine.Handle, out orient_deg, out orient_conf, out script_nameHandle, out script_conf) != 0)
            {
                orientation = orient_deg;
                confidence = orient_conf;
                if (script_nameHandle != IntPtr.Zero)
                {
                    scriptName = MarshalHelper.PtrToString(script_nameHandle, Encoding.ASCII);
                    // Don't delete script_nameHandle as it points to internal memory managed by Tesseract.
                } else
                {
                    scriptName = null;
                }
                scriptConfidence = script_conf;
            }
            else
            {
                throw new TesseractException("Failed to detect image orientation.");
            }
        }

        internal void Recognize()
        {
            Guard.Verify(PageSegmentMode != PageSegMode.OsdOnly, "Cannot OCR image when using OSD only page segmentation, please use DetectBestOrientation instead.");
            if (!runRecognitionPhase) {
                if (Interop.TessApi.Native.BaseApiRecognize(Engine.Handle, new HandleRef(this, IntPtr.Zero)) != 0) {
                    throw new InvalidOperationException("Recognition of image failed.");
                }

                runRecognitionPhase = true;

                // now write out the thresholded image if required to do so
                bool tesseditWriteImages;
                if (Engine.TryGetBoolVariable("tessedit_write_images", out tesseditWriteImages) && tesseditWriteImages) {
                    using (Pix thresholdedImage = GetThresholdedImage()) {
                        string filePath = Path.Combine(Environment.CurrentDirectory, "tessinput.tif");
                        try {
                            thresholdedImage.Save(filePath, ImageFormat.TiffG4);
                            trace.TraceEvent(TraceEventType.Information, 2, "Successfully saved the thresholded image to '{0}'", filePath);
                        } catch (Exception error) {
                            trace.TraceEvent(TraceEventType.Error, 2, "Failed to save the thresholded image to '{0}'.\nError: {1}", filePath, error.Message);
                        }
                    }
                }
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