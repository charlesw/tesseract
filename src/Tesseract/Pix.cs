using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Tesseract.Internal;

namespace Tesseract
{
    public unsafe sealed class Pix : DisposableBase, IEquatable<Pix>
    {
        #region Constants

        public const float Deg2Rad = (float)(Math.PI / 180.0);

        // Skew Defaults
        public const int DefaultBinarySearchReduction = 2; // binary search part

        public const int DefaultBinaryThreshold = 130;

        /// <summary>
        /// A small angle, in radians, for threshold checking. Equal to about 0.06 degrees.
        /// </summary>
        private const float VerySmallAngle = 0.001F;

        private static readonly List<int> AllowedDepths = new List<int> { 1, 2, 4, 8, 16, 32 };

        /// <summary>
        /// Used to lookup image formats by extension.
        /// </summary>
        private static readonly Dictionary<string, ImageFormat> imageFomatLookup = new Dictionary<string, ImageFormat>
        {
            { ".jpg", ImageFormat.JfifJpeg },
            { ".jpeg", ImageFormat.JfifJpeg },
            { ".gif", ImageFormat.Gif },
            { ".tif", ImageFormat.Tiff },
            { ".tiff", ImageFormat.Tiff },
            { ".png", ImageFormat.Png },
            { ".bmp", ImageFormat.Bmp }
        };

        #endregion Constants

        #region Fields

        private readonly int depth;
        private readonly int height;
        private readonly int width;
        private PixColormap colormap;
        private HandleRef handle;

        #endregion Fields

        #region Create\Load methods

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

            this.handle = new HandleRef(this, handle);
            this.width = Interop.LeptonicaApi.Native.pixGetWidth(this.handle);
            this.height = Interop.LeptonicaApi.Native.pixGetHeight(this.handle);
            this.depth = Interop.LeptonicaApi.Native.pixGetDepth(this.handle);

            var colorMapHandle = Interop.LeptonicaApi.Native.pixGetColormap(this.handle);
            if (colorMapHandle != IntPtr.Zero)
            {
                this.colormap = new PixColormap(colorMapHandle);
            }
        }

        public static Pix Create(int width, int height, int depth)
        {
            if (!AllowedDepths.Contains(depth))
                throw new ArgumentException("Depth must be 1, 2, 4, 8, 16, or 32 bits.", "depth");

            if (width <= 0) throw new ArgumentException("Width must be greater than zero", "width");
            if (height <= 0) throw new ArgumentException("Height must be greater than zero", "height");

            var handle = Interop.LeptonicaApi.Native.pixCreate(width, height, depth);
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
            var pixHandle = Interop.LeptonicaApi.Native.pixRead(filename);
            if (pixHandle == IntPtr.Zero)
            {
                throw new IOException(String.Format("Failed to load image '{0}'.", filename));
            }
            return Create(pixHandle);
        }

        public static Pix LoadTiffFromMemory(byte[] bytes)
        {
            IntPtr handle;
            fixed (byte* ptr = bytes)
            {
                handle = Interop.LeptonicaApi.Native.pixReadMemTiff(ptr, bytes.Length, 0);
            }
            if (handle == IntPtr.Zero)
            {
                throw new IOException("Failed to load image from memory.");
            }
            return Create(handle);
        }

        #endregion Create\Load methods

        #region Properties

        public PixColormap Colormap
        {
            get { return colormap; }
            set
            {
                if (value != null)
                {
                    if (Interop.LeptonicaApi.Native.pixSetColormap(handle, value.Handle) == 0)
                    {
                        colormap = value;
                    }
                }
                else
                {
                    if (Interop.LeptonicaApi.Native.pixDestroyColormap(handle) == 0)
                    {
                        colormap = null;
                    }
                }
            }
        }

        public int Depth
        {
            get { return depth; }
        }

        public int Height
        {
            get { return height; }
        }

        public int Width
        {
            get { return width; }
        }

        internal HandleRef Handle
        {
            get { return handle; }
        }

        public PixData GetData()
        {
            return new PixData(this);
        }

        #endregion Properties

        #region Equals

        public override bool Equals(object obj)
        {
            // Check for null values and compare run-time types.
            if (obj == null || GetType() != obj.GetType())
                return false;

            return Equals((Pix)obj);
        }

        public bool Equals(Pix other)
        {
            if (other == null)
            {
                return false;
            }

            int same;
            if(Interop.LeptonicaApi.Native.pixEqual(Handle, other.Handle, out same) != 0)
            {
                throw new TesseractException("Failed to compare pix");
            }
            return same != 0;
        }

        #endregion

        #region Save methods

        /// <summary>
        /// Saves the image to the specified file.
        /// </summary>
        /// <param name="filename">The path to the file.</param>
        /// <param name="format">The format to use when saving the image, if not specified the file extension is used to guess the format.</param>
        public void Save(string filename, ImageFormat? format = null)
        {
            ImageFormat actualFormat;
            if (!format.HasValue)
            {
                var extension = Path.GetExtension(filename).ToLowerInvariant();
                if (!imageFomatLookup.TryGetValue(extension, out actualFormat))
                {
                    // couldn't find matching format, perhaps there is no extension or it's not recognised, fallback to default.
                    actualFormat = ImageFormat.Default;
                }
            }
            else
            {
                actualFormat = format.Value;
            }

            if (Interop.LeptonicaApi.Native.pixWrite(filename, handle, actualFormat) != 0)
            {
                throw new IOException(String.Format("Failed to save image '{0}'.", filename));
            }
        }

        #endregion Save methods

        #region Clone

        /// <summary>
        /// Increments this pix's reference count and returns a reference to the same pix data.
        /// </summary>
        /// <remarks>
        /// A "clone" is simply a reference to an existing pix. It is implemented this way because
        /// image can be large and hence expensive to copy and extra handles need to be made with a simple
        /// policy to avoid double frees and memory leaks.
        ///
        /// The general usage protocol is:
        /// <list type="number">
        /// 	<item>Whenever you want a new reference to an existing <see cref="Pix" /> call <see cref="Pix.Clone" />.</item>
        ///     <item>
        /// 		Always call <see cref="Pix.Dispose" /> on all references. This decrements the reference count and
        /// 		will destroy the pix when the reference count reaches zero.
        /// 	</item>
        /// </list>
        /// </remarks>
        /// <returns>The pix with it's reference count incremented.</returns>
        public Pix Clone()
        {
            var clonedHandle = Interop.LeptonicaApi.Native.pixClone(handle);
            return new Pix(clonedHandle);
        }

        #endregion Clone

        #region Image manipulation

        /// <summary>
        /// Binarization of the input image based on the passed parameters and the Otsu method
        /// </summary>
        /// <param name="sx"> sizeX Desired tile X dimension; actual size may vary.</param>
        /// <param name="sy"> sizeY Desired tile Y dimension; actual size may vary.</param>
        /// <param name="smoothx"> smoothX Half-width of convolution kernel applied to threshold array: use 0 for no smoothing.</param>
        /// <param name="smoothy"> smoothY Half-height of convolution kernel applied to threshold array: use 0 for no smoothing.</param>
        /// <param name="scorefract"> scoreFraction Fraction of the max Otsu score; typ. 0.1 (use 0.0 for standard Otsu).</param>
        /// <returns>The binarized image.</returns>
        public Pix BinarizeOtsuAdaptiveThreshold(int sx, int sy, int smoothx, int smoothy, float scorefract)
        {
            Guard.Verify(Depth == 8, "Image must have a depth of 8 bits per pixel to be binerized using Otsu.");
            Guard.Require("sx", sx >= 16, "The sx parameter must be greater than or equal to 16");
            Guard.Require("sy", sy >= 16, "The sy parameter must be greater than or equal to 16");

            IntPtr ppixth, ppixd;
            int result = Interop.LeptonicaApi.Native.pixOtsuAdaptiveThreshold(handle, sx, sy, smoothx, smoothy, scorefract, out ppixth, out ppixd);

            if (ppixth != IntPtr.Zero)
            {
                // free memory held by ppixth, an array of threshold values found for each tile
                Interop.LeptonicaApi.Native.pixDestroy(ref ppixth);
            }

            if (result == 1) throw new TesseractException("Failed to binarize image.");

            return new Pix(ppixd);
        }

        /// <summary>
        /// Binarization of the input image using the Sauvola local thresholding method.
        ///
        /// Note: The source image must be 8 bpp grayscale; not colormapped.
        /// </summary>
        /// <remarks>
        /// <list type="number">
        ///     <listheader>Notes</listheader>
        ///     <item>The window width and height are 2 * <paramref name="whsize"/> + 1. The minimum value for <paramref name="whsize"/> is 2; typically it is >= 7.</item>
        ///     <item>The local statistics, measured over the window, are the average and standard deviation.</item>
        ///     <item>
        ///     The measurements of the mean and standard deviation are performed inside a border of (<paramref name="whsize"/> + 1) pixels.
        ///     If source pix does not have these added border pixels, use <paramref name="addborder"/> = <c>True</c> to add it here; otherwise use
        ///     <paramref name="addborder"/> = <c>False</c>.
        ///     </item>
        ///     <item>
        ///     The Sauvola threshold is determined from the formula:  t = m * (1 - k * (1 - s / 128)) where t = local threshold, m = local mean,
        ///     k = <paramref name="factor"/>, and s = local standard deviation which is maximised at 127.5 when half the samples are 0 and the other
        ///     half are 255.
        ///     </item>
        ///     <item>
        ///     The basic idea of Niblack and Sauvola binarization is that the local threshold should be less than the median value,
        ///     and the larger the variance, the closer to the median it should be chosen. Typical values for k are between 0.2 and 0.5.
        ///     </item>
        /// </list>
        /// </remarks>
        /// <param name="whsize">the window half-width for measuring local statistics.</param>
        /// <param name="factor">The factor for reducing threshold due to variances greater than or equal to zero (0). Typically around 0.35.</param>
        /// <param name="addborder">If <c>True</c> add a border of width (<paramref name="whsize"/> + 1) on all sides.</param>
        /// <returns>The binarized image.</returns>
        public Pix BinarizeSauvola(int whsize, float factor, bool addborder)
        {
            Guard.Verify(Depth == 8, "Source image must be 8bpp");
            Guard.Verify(Colormap == null, "Source image must not be color mapped.");
            Guard.Require("whsize", whsize >= 2, "The window half-width (whsize) must be greater than 2.");
            int maxWhSize = Math.Min((Width - 3) / 2, (Height - 3) / 2);
            Guard.Require("whsize", whsize < maxWhSize, "The window half-width (whsize) must be less than {0} for this image.", maxWhSize);
            Guard.Require("factor", factor >= 0, "Factor must be greater than zero (0).");

            IntPtr ppixm, ppixsd, ppixth, ppixd;
            int result = Interop.LeptonicaApi.Native.pixSauvolaBinarize(handle, whsize, factor, addborder ? 1 : 0, out ppixm, out ppixsd, out ppixth, out ppixd);

            // Free memory held by other unused pix's

            if (ppixm != IntPtr.Zero)
            {
                Interop.LeptonicaApi.Native.pixDestroy(ref ppixm);
            }

            if (ppixsd != IntPtr.Zero)
            {
                Interop.LeptonicaApi.Native.pixDestroy(ref ppixsd);
            }

            if (ppixth != IntPtr.Zero)
            {
                Interop.LeptonicaApi.Native.pixDestroy(ref ppixth);
            }

            if (result == 1) throw new TesseractException("Failed to binarize image.");

            return new Pix(ppixd);
        }

        /// <summary>
        /// Binarization of the input image using the Sauvola local thresholding method on tiles
        /// of the source image.
        ///
        /// Note: The source image must be 8 bpp grayscale; not colormapped.
        /// </summary>
        /// <remarks>
        /// A tiled version of Sauvola can become neccisary for large source images (over 16M pixels) because:
        ///
        /// * The mean value accumulator is a uint32, overflow can occur for an image with more than 16M pixels.
        /// * The mean value accumulator array for 16M pixels is 64 MB. While the mean square accumulator array for 16M pixels is 128 MB.
        ///   Using tiles reduces the size of these arrays.
        /// * Each tile can be processed independently, in parallel, on a multicore processor.
        /// </remarks>
        /// <param name="whsize">The window half-width for measuring local statistics</param>
        /// <param name="factor">The factor for reducing threshold due to variances greater than or equal to zero (0). Typically around 0.35.</param>
        /// <param name="nx">The number of tiles to subdivide the source image into on the x-axis.</param>
        /// <param name="ny">The number of tiles to subdivide the source image into on the y-axis.</param>
        /// <returns>THe binarized image.</returns>
        public Pix BinarizeSauvolaTiled(int whsize, float factor, int nx, int ny)
        {
            Guard.Verify(Depth == 8, "Source image must be 8bpp");
            Guard.Verify(Colormap == null, "Source image must not be color mapped.");
            Guard.Require("whsize", whsize >= 2, "The window half-width (whsize) must be greater than 2.");
            int maxWhSize = Math.Min((Width - 3) / 2, (Height - 3) / 2);
            Guard.Require("whsize", whsize < maxWhSize, "The window half-width (whsize) must be less than {0} for this image.", maxWhSize);
            Guard.Require("factor", factor >= 0, "Factor must be greater than zero (0).");

            IntPtr ppixth, ppixd;
            int result = Interop.LeptonicaApi.Native.pixSauvolaBinarizeTiled(handle, whsize, factor, nx, ny, out ppixth, out ppixd);

            // Free memory held by other unused pix's
            if (ppixth != IntPtr.Zero)
            {
                Interop.LeptonicaApi.Native.pixDestroy(ref ppixth);
            }

            if (result == 1) throw new TesseractException("Failed to binarize image.");

            return new Pix(ppixd);
        }

        /// <summary>
        /// Conversion from RBG to 8bpp grayscale using the specified weights. Note red, green, blue weights should add up to 1.0.
        /// </summary>
        /// <param name="rwt">Red weight</param>
        /// <param name="gwt">Green weight</param>
        /// <param name="bwt">Blue weight</param>
        /// <returns>The Grayscale pix.</returns>
        public Pix ConvertRGBToGray(float rwt, float gwt, float bwt)
        {
            Guard.Verify(Depth == 32, "The source image must have a depth of 32 (32 bpp).");
            Guard.Require("rwt", rwt >= 0, "All weights must be greater than or equal to zero; red was not.");
            Guard.Require("gwt", gwt >= 0, "All weights must be greater than or equal to zero; green was not.");
            Guard.Require("bwt", bwt >= 0, "All weights must be greater than or equal to zero; blue was not.");

            var resultPixHandle = Interop.LeptonicaApi.Native.pixConvertRGBToGray(handle, rwt, gwt, bwt);
            if (resultPixHandle == IntPtr.Zero) throw new TesseractException("Failed to convert to grayscale.");
            return new Pix(resultPixHandle);
        }
        /// <summary>
        /// Conversion from RBG to 8bpp grayscale.
        /// </summary>
        /// <returns>The Grayscale pix.</returns>
        public Pix ConvertRGBToGray()
        {
            return ConvertRGBToGray(0, 0, 0);
        }

        /// <summary>
        /// Removes horizontal lines from a grayscale image. 
        /// The algorithm is based on Leptonica <code>lineremoval.c</code> example.
        /// See <a href="http://www.leptonica.com/line-removal.html">line-removal</a>.
        /// </summary>
        /// <returns>image with lines removed</returns>
        public Pix RemoveLines()
        {
            float angle, conf;
            IntPtr pix1, pix2, pix3, pix4, pix5, pix6, pix7, pix8, pix9;

            pix1 = pix2 = pix3 = pix4 = pix5 = pix6 = pix7 = pix8 = pix9 = IntPtr.Zero;

            try
            {
                /* threshold to binary, extracting much of the lines */
                pix1 = Interop.LeptonicaApi.Native.pixThresholdToBinary(handle, 170);

                /* find the skew angle and deskew using an interpolated
                 * rotator for anti-aliasing (to avoid jaggies) */
                Interop.LeptonicaApi.Native.pixFindSkew(new HandleRef(this, pix1), out angle, out conf);
                pix2 = Interop.LeptonicaApi.Native.pixRotateAMGray(handle, (float)(Deg2Rad * angle), (byte)255);

                /* extract the lines to be removed */
                pix3 = Interop.LeptonicaApi.Native.pixCloseGray(new HandleRef(this, pix2), 51, 1);

                /* solidify the lines to be removed */
                pix4 = Interop.LeptonicaApi.Native.pixErodeGray(new HandleRef(this, pix3), 1, 5);

                /* clean the background of those lines */
                pix5 = Interop.LeptonicaApi.Native.pixThresholdToValue(new HandleRef(this, IntPtr.Zero), new HandleRef(this, pix4), 210, 255);

                pix6 = Interop.LeptonicaApi.Native.pixThresholdToValue(new HandleRef(this, IntPtr.Zero), new HandleRef(this, pix5), 200, 0);

                /* get paint-through mask for changed pixels */
                pix7 = Interop.LeptonicaApi.Native.pixThresholdToBinary(new HandleRef(this, pix6), 210);

                /* add the inverted, cleaned lines to orig.  Because
                 * the background was cleaned, the inversion is 0,
                 * so when you add, it doesn't lighten those pixels.
                 * It only lightens (to white) the pixels in the lines! */
                Interop.LeptonicaApi.Native.pixInvert(new HandleRef(this, pix6), new HandleRef(this, pix6));
                pix8 = Interop.LeptonicaApi.Native.pixAddGray(new HandleRef(this, IntPtr.Zero), new HandleRef(this, pix2), new HandleRef(this, pix6));

                pix9 = Interop.LeptonicaApi.Native.pixOpenGray(new HandleRef(this, pix8), 1, 9);

                Interop.LeptonicaApi.Native.pixCombineMasked(new HandleRef(this, pix8), new HandleRef(this, pix9), new HandleRef(this, pix7));
                if (pix8 == IntPtr.Zero)
                {
                    throw new TesseractException("Failed to remove lines from image.");
                }

                return new Pix(pix8);
            }
            finally
            {
                // destroy any created intermediate pix's, regardless of if the process 
                // failed for any reason.
                if (pix1 != IntPtr.Zero)
                {
                    Interop.LeptonicaApi.Native.pixDestroy(ref pix1);
                }

                if (pix2 != IntPtr.Zero)
                {
                    Interop.LeptonicaApi.Native.pixDestroy(ref pix2);
                }

                if (pix3 != IntPtr.Zero)
                {
                    Interop.LeptonicaApi.Native.pixDestroy(ref pix3);
                }

                if (pix4 != IntPtr.Zero)
                {
                    Interop.LeptonicaApi.Native.pixDestroy(ref pix4);
                }

                if (pix5 != IntPtr.Zero)
                {
                    Interop.LeptonicaApi.Native.pixDestroy(ref pix5);
                }

                if (pix6 != IntPtr.Zero)
                {
                    Interop.LeptonicaApi.Native.pixDestroy(ref pix6);
                }

                if (pix7 != IntPtr.Zero)
                {
                    Interop.LeptonicaApi.Native.pixDestroy(ref pix7);
                }

                if (pix9 != IntPtr.Zero)
                {
                    Interop.LeptonicaApi.Native.pixDestroy(ref pix9);
                }
            }
        }

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
            var resultPixHandle = Interop.LeptonicaApi.Native.pixDeskewGeneral(handle, sweep.Reduction, sweep.Range, sweep.Delta, redSearch, thresh, out pAngle, out pConf);
            if (resultPixHandle == IntPtr.Zero) throw new TesseractException("Failed to deskew image.");
            scew = new Scew(pAngle, pConf);
            return new Pix(resultPixHandle);
        }

        /// <summary>
        /// Creates a new image by rotating this image about it's centre.
        /// </summary>
        /// <remarks>
        /// Please note the following:
        /// <list type="bullet">
        /// <item>
        /// Rotation will bring in either white or black pixels, as specified by <paramref name="fillColor" /> from
        /// the outside as required.
        /// </item>
        /// <item>Above 20 degrees, sampling rotation will be used if shear was requested.</item>
        /// <item>Colormaps are removed for rotation by area map and shear.</item>
        /// <item>
        /// The resulting image can be expanded so that no image pixels are lost. To invoke expansion,
        /// input the original width and height. For repeated rotation, use of the original width and heigh allows
        /// expansion to stop at the maximum required size which is a square of side = sqrt(w*w + h*h).
        /// </item>
        /// </list>
        /// <para>
        /// Please note there is an implicit assumption about RGB component ordering.
        /// </para>
        /// </remarks>
        /// <param name="angleInRadians">The angle to rotate by, in radians; clockwise is positive.</param>
        /// <param name="method">The rotation method to use.</param>
        /// <param name="fillColor">The fill color to use for pixels that are brought in from the outside.</param>
        /// <param name="width">The original width; use 0 to avoid embedding</param>
        /// <param name="height">The original height; use 0 to avoid embedding</param>
        /// <returns>The image rotated around it's centre.</returns>
        public Pix Rotate(float angleInRadians, RotationMethod method = RotationMethod.AreaMap, RotationFill fillColor = RotationFill.White, int? width = null, int? height = null)
        {
            if (width == null) width = this.Width;
            if (height == null) height = this.Height;

            if (Math.Abs(angleInRadians) < VerySmallAngle) return this.Clone();

            IntPtr resultHandle;

            var rotations = 2 * angleInRadians / Math.PI;
            if (Math.Abs(rotations - Math.Floor(rotations)) < VerySmallAngle)
            {
                // handle special case of orthoganal rotations (90, 180, 270)
                resultHandle = Interop.LeptonicaApi.Native.pixRotateOrth(handle, (int)rotations);
            }
            else
            {
                // handle general case
                resultHandle = Interop.LeptonicaApi.Native.pixRotate(handle, angleInRadians, method, fillColor, width.Value, height.Value);
            }

            if (resultHandle == IntPtr.Zero) throw new LeptonicaException("Failed to rotate image around its centre.");

            return new Pix(resultHandle);
        }

        /// <summary>
        /// 90 degree rotation.
        /// </summary>
        /// <param name="direction">1 = clockwise,  -1 = counter-clockwise</param>
        /// <returns>rotated image</returns>
        public Pix Rotate90(int direction)
        {
            IntPtr resultHandle = Interop.LeptonicaApi.Native.pixRotate90(handle, direction);

            if (resultHandle == IntPtr.Zero)
            {
                throw new LeptonicaException("Failed to rotate image.");
            }
            return new Pix(resultHandle);
        }

        #endregion Image manipulation

        #region Scaling


        /// <summary>
        /// Scales the current pix by the specified <paramref name="scaleX"/> and <paramref name="scaleY"/> factors returning a new <see cref="Pix"/> of the same depth. 
        /// </summary>
        /// <param name="scaleX"></param>
        /// <param name="scaleY"></param>
        /// <returns>The scaled image.</returns>
        /// <remarks>
        /// <para>
        ///  This function scales 32 bpp RGB; 2, 4 or 8 bpp palette color;
        /// 2, 4, 8 or 16 bpp gray; and binary images.
        /// </para>
        /// <para>
        /// When the input has palette color, the colormap is removed and
        /// the result is either 8 bpp gray or 32 bpp RGB, depending on whether
        /// the colormap has color entries.  Images with 2, 4 or 16 bpp are
        /// converted to 8 bpp.
        /// </para>
        /// <para>
        /// Because Scale() is meant to be a very simple interface to a
        /// number of scaling functions, including the use of unsharp masking,
        /// the type of scaling and the sharpening parameters are chosen
        /// by default.  Grayscale and color images are scaled using one
        /// of four methods, depending on the scale factors:
        /// <list type="number">
        /// <item>
        /// <description>
        /// antialiased subsampling (lowpass filtering followed by
        /// subsampling, implemented here by area mapping), for scale factors
        /// less than 0.2
        /// </description>
        /// </item>
        /// <item>
        /// <description>
        /// antialiased subsampling with sharpening, for scale factors
        /// between 0.2 and 0.7.
        /// </description>
        /// </item>
        /// <item>
        /// <description>
        /// linear interpolation with sharpening, for scale factors between
        /// 0.7 and 1.4.
        /// </description>
        /// </item>
        /// <item>
        /// <description>
        /// linear interpolation without sharpening, for scale factors >= 1.4.
        /// </description>
        /// </item>
        /// </list>
        /// One could use subsampling for scale factors very close to 1.0,
        /// because it preserves sharp edges.  Linear interpolation blurs
        /// edges because the dest pixels will typically straddle two src edge
        /// pixels.  Subsmpling removes entire columns and rows, so the edge is
        /// not blurred.  However, there are two reasons for not doing this.
        /// First, it moves edges, so that a straight line at a large angle to
        /// both horizontal and vertical will have noticable kinks where
        /// horizontal and vertical rasters are removed.  Second, although it
        /// is very fast, you get good results on sharp edges by applying
        /// a sharpening filter.
        /// </para>
        /// <para>
        /// For images with sharp edges, sharpening substantially improves the
        /// image quality for scale factors between about 0.2 and about 2.0.
        /// pixScale() uses a small amount of sharpening by default because
        /// it strengthens edge pixels that are weak due to anti-aliasing.
        /// The default sharpening factors are:
        /// <list type="bullet">
        /// <item>
        /// <description><![CDATA[for scaling factors < 0.7:   sharpfract = 0.2    sharpwidth = 1]]></description>
        /// </item>
        /// <item>
        /// <description>for scaling factors >= 0.7:  sharpfract = 0.4    sharpwidth = 2</description>
        /// </item>
        /// </list>
        /// The cases where the sharpening halfwidth is 1 or 2 have special
        /// implementations and are about twice as fast as the general case.
        /// </para>
        /// <para>
        /// However, sharpening is computationally expensive, and one needs
        /// to consider the speed-quality tradeoff:
        /// <list type="bullet">
        /// <item>
        /// <description>
        /// For upscaling of RGB images, linear interpolation plus default
        /// sharpening is about 5 times slower than upscaling alone.</description>
        /// </item>
        /// <item>
        /// <description>
        /// For downscaling, area mapping plus default sharpening is
        /// about 10 times slower than downscaling alone.
        /// </description>
        /// </item>
        /// </list>
        /// When the scale factor is larger than 1.4, the cost of sharpening,
        /// which is proportional to image area, is very large compared to the
        /// incremental quality improvement, so we cut off the default use of
        /// sharpening at 1.4.  Thus, for scale factors greater than 1.4,
        /// pixScale() only does linear interpolation.
        /// </para>
        /// <para>
        /// In many situations you will get a satisfactory result by scaling
        /// without sharpening: call pixScaleGeneral() with @sharpfract = 0.0.
        /// Alternatively, if you wish to sharpen but not use the default
        /// value, first call pixScaleGeneral() with @sharpfract = 0.0, and
        /// then sharpen explicitly using pixUnsharpMasking().
        /// </para>
        /// <para>
        /// Binary images are scaled to binary by sampling the closest pixel,
        /// without any low-pass filtering (averaging of neighboring pixels).
        /// This will introduce aliasing for reductions.  Aliasing can be
        /// prevented by using pixScaleToGray() instead.
        /// </para>
        /// <para>
        /// Warning: implicit assumption about RGB component order for LI color scaling
        /// </para>
        ///</remarks>
        public Pix Scale(float scaleX, float scaleY)
        {
            IntPtr result = Interop.LeptonicaApi.Native.pixScale(handle, scaleX, scaleY);

            if (result == IntPtr.Zero) throw new InvalidOperationException("Failed to scale pix.");

            return new Pix(result);
        }

        #endregion

        #region Disposal

        protected override void Dispose(bool disposing)
        {
            var tmpHandle = handle.Handle;
            Interop.LeptonicaApi.Native.pixDestroy(ref tmpHandle);
            this.handle = new HandleRef(this, IntPtr.Zero);
        }

        #endregion Disposal
    }
}