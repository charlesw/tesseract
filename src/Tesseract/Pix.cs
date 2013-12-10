using System;
using System.Collections.Generic;
using System.IO;

namespace Tesseract
{
    public unsafe sealed class Pix : DisposableBase
    {
        #region Constants

        // Skew Defaults
        public const int DefaultBinarySearchReduction = 2; // binary search part
        public const int DefaultBinaryThreshold = 130;

        #endregion

        #region Fields

        private static readonly List<int> AllowedDepths = new List<int> { 1, 2, 4, 8, 16, 32 };

        private IntPtr handle;
        private PixColormap colormap;
        private readonly int width;
        private readonly int height;
        private readonly int depth;

        #endregion


        #region Create\Load methods

        public static Pix Create(int width, int height, int depth)
        {
            if (!AllowedDepths.Contains(depth))
                throw new ArgumentException("Depth must be 1, 2, 4, 8, 16, or 32 bits.", "depth");

            if (width <= 0) throw new ArgumentException("Width must be greater than zero", "width");
            if (height <= 0) throw new ArgumentException("Height must be greater than zero", "height");
            
            var handle = Interop.LeptonicaApi.pixCreate(width, height, depth);
            if (handle == IntPtr.Zero) throw new InvalidOperationException("Failed to create pix, this normally occurs because the requested image size is too large, please check Standard Error Output.");

            return Create(handle);
        }

        public static Pix Create(IntPtr handle)
        {
            if (handle == IntPtr.Zero) throw new ArgumentException("Pix handle must not be zero (null).", "handle");

            return new Pix(handle);
        }

        public static Pix LoadFromFile(string filename)
        {
            var pixHandle = Interop.LeptonicaApi.pixRead(filename);
            if (pixHandle == IntPtr.Zero) {
                throw new IOException(String.Format("Failed to load image '{0}'.", filename));
            }
            return Create(pixHandle);
        }
        
        /// <summary>
        /// Creates a new pix instance using an existing handle to a pix structure.
        /// </summary>
        /// <remarks>
        /// Note that the resulting instance takes ownership of the data structure.
        /// </remarks>
        /// <param name="handle"></param>
        private Pix(IntPtr handle)
        {
            if (handle == IntPtr.Zero) throw new ArgumentNullException("handle");

            this.handle = handle;
            this.width = Interop.LeptonicaApi.pixGetWidth(handle);
            this.height = Interop.LeptonicaApi.pixGetHeight(handle);
            this.depth = Interop.LeptonicaApi.pixGetDepth(handle);

            var colorMapHandle = Interop.LeptonicaApi.pixGetColormap(handle);
            if (colorMapHandle != IntPtr.Zero) {
                this.colormap = new PixColormap(colorMapHandle);
            }
        }

        #endregion

        #region Properties

        public PixColormap Colormap
        {
            get { return colormap; }
            set
            {
                if (value != null) {
                    if (Interop.LeptonicaApi.pixSetColormap(Handle, value.Handle) == 0) {
                        colormap = value;
                    }
                } else {
                    if (Interop.LeptonicaApi.pixDestroyColormap(Handle) == 0) {
                        colormap = null;
                    }
                }
            }
        }

        public int Width
        {
            get { return width; }
        }

        public int Height
        {
            get { return height; }
        }

        public int Depth
        {
            get { return depth; }
        }

        public PixData GetData()
        {
            return new PixData(this);
        }

        public IntPtr Handle
        {
            get { return handle; }
        }

        #endregion

        #region Methods


        public void Save(string filename, ImageFormat format = ImageFormat.Default)
        {
            if (Interop.LeptonicaApi.pixWrite(filename, handle, format) != 0) {
                throw new IOException(String.Format("Failed to save image '{0}'.", filename));
            }
        }

        // image manipulation

        /// <summary>
        /// Determines the scew angle and if confidence is high enough returns the descewed image as the result, otherwise returns clone of original image.
        /// </summary>
        /// <remarks>
        /// This binarizes if necessary and finds the skew angle.  If the
        /// angle is large enough and there is sufficient confidence,
        /// it returns a deskewed image; otherwise, it returns a clone.
        /// </remarks>
        /// <returns>Returns deskewed image if confidence was high enough, otherwise returns clone of original pix.</returns>
        public Pix Deskew()
        {
            Scew scew;
            return Deskew(DefaultBinarySearchReduction, out scew);
        }

        /// <summary>
        /// Determines the scew angle and if confidence is high enough returns the descewed image as the result, otherwise returns clone of original image.
        /// </summary>
        /// <remarks>
        /// This binarizes if necessary and finds the skew angle.  If the
        /// angle is large enough and there is sufficient confidence,
        /// it returns a deskewed image; otherwise, it returns a clone.
        /// </remarks>
        /// <param name="scew">The scew angle and confidence</param>
        /// <returns>Returns deskewed image if confidence was high enough, otherwise returns clone of original pix.</returns>
        public Pix Deskew(out Scew scew)
        {
            return Deskew(DefaultBinarySearchReduction, out scew);
        }

        /// <summary>
        /// Determines the scew angle and if confidence is high enough returns the descewed image as the result, otherwise returns clone of original image.
        /// </summary>
        /// <remarks>
        /// This binarizes if necessary and finds the skew angle.  If the
        /// angle is large enough and there is sufficient confidence,
        /// it returns a deskewed image; otherwise, it returns a clone.
        /// </remarks>
        /// <param name="redSearch">The reduction factor used by the binary search, can be 1, 2, or 4.</param>
        /// <param name="scew">The scew angle and confidence</param>
        /// <returns>Returns deskewed image if confidence was high enough, otherwise returns clone of original pix.</returns>
        public Pix Deskew(int redSearch, out Scew scew)
        {
            return Deskew(ScewSweep.Default, redSearch, DefaultBinaryThreshold, out scew);
        }

        /// <summary>
        /// Determines the scew angle and if confidence is high enough returns the descewed image as the result, otherwise returns clone of original image.
        /// </summary>
        /// <remarks>
        /// This binarizes if necessary and finds the skew angle.  If the
        /// angle is large enough and there is sufficient confidence,
        /// it returns a deskewed image; otherwise, it returns a clone.
        /// </remarks>
        /// <param name="sweep">linear sweep parameters</param>
        /// <param name="redSearch">The reduction factor used by the binary search, can be 1, 2, or 4.</param>
        /// <param name="thresh">The threshold value used for binarizing the image.</param>
        /// <param name="scew">The scew angle and confidence</param>
        /// <returns>Returns deskewed image if confidence was high enough, otherwise returns clone of original pix.</returns>
        public Pix Deskew(ScewSweep sweep, int redSearch, int thresh, out Scew scew)
        {
            float pAngle, pConf;
            var resultPixHandle = Interop.LeptonicaApi.pixDeskewGeneral(handle, sweep.Reduction, sweep.Range, sweep.Delta, redSearch, thresh, out pAngle, out pConf);
            if (resultPixHandle == IntPtr.Zero) throw new TesseractException("Failed to deskew image.");
            scew = new Scew(pAngle, pConf);
            return new Pix(resultPixHandle);
        }

        /// <summary>
        /// Binarization of the input image based on the passed parameters and the Otsu method
        /// </summary>
        /// <param name="sx"> sizeX Desired tile X dimension; actual size may vary.</param>
        /// <param name="sy"> sizeY Desired tile Y dimension; actual size may vary.</param>
        /// <param name="smoothx"> smoothX Half-width of convolution kernel applied to threshold array: use 0 for no smoothing.</param>
        /// <param name="smoothy"> smoothY Half-height of convolution kernel applied to threshold array: use 0 for no smoothing.</param>
        /// <param name="scorefract"> scoreFraction Fraction of the max Otsu score; typ. 0.1 (use 0.0 for standard Otsu).</param>
        /// <returns> ppixd is a pointer to the thresholded PIX image.</returns>
        public Pix BinarizeOtsuAdaptiveThreshold(int sx, int sy, int smoothx, int smoothy, float scorefract)
        {
            IntPtr ppixth, ppixd;
            int result = Interop.LeptonicaApi.pixOtsuAdaptiveThreshold(handle, sx, sy, smoothx, smoothy, scorefract, out ppixth, out ppixd);
            if (result == 1) throw new TesseractException("Failed to binarize image.");
            return new Pix(ppixd);
        }

        /// <summary>
        /// Conversion from RBG to 8bpp grayscale.
        /// </summary>
        /// <param name="rwt">Red weight</param>
        /// <param name="gwt">Green weight</param>
        /// <param name="bwt">Blue weight</param>
        /// <returns></returns>
        public Pix ConvertRGBToGray(float rwt, float gwt, float bwt)
        {
            var resultPixHandle = Interop.LeptonicaApi.pixConvertRGBToGray(handle, rwt, gwt, bwt);
            if (resultPixHandle == IntPtr.Zero) throw new TesseractException("Failed to convert to grayscale.");
            return new Pix(resultPixHandle);
        }

        protected override void Dispose(bool disposing)
        {
            Interop.LeptonicaApi.pixDestroy(ref handle);
        }

        #endregion

    }
}
