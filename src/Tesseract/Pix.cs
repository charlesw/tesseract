using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace Tesseract
{
	public unsafe sealed class Pix : DisposableBase
	{		
		#region Constants
		
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
    	
		#endregion
		
		#region Fields
		
		private HandleRef handle;
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
		
		public static Pix[] LoadMultiPageTiffFromFile(string filename)
		{
			IntPtr pixaHandle = IntPtr.Zero;
			using(var pixA = PixArray.LoadMultiPageTiffFromFile(filename)) {
				var result = new Pix[pixA.Count];
				for (int i = 0; i < result.Length; i++) {
					result[i] = pixA.GetPix(i, PixArrayAccessType.Clone);
				}
				return result;
			}
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
			
			this.handle = new HandleRef(this, handle);
			this.width = Interop.LeptonicaApi.pixGetWidth(this.handle);
			this.height = Interop.LeptonicaApi.pixGetHeight(this.handle);
			this.depth = Interop.LeptonicaApi.pixGetDepth(this.handle);
			
			var colorMapHandle = Interop.LeptonicaApi.pixGetColormap(this.handle);
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
					if (Interop.LeptonicaApi.pixSetColormap(handle, value.Handle) == 0) {
						colormap = value;
					}
				} else {
					if (Interop.LeptonicaApi.pixDestroyColormap(handle) == 0) {
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
		
		public HandleRef Handle
		{
			get { return handle; }
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
			if(!format.HasValue) {
				var extension = Path.GetExtension(filename).ToLowerInvariant();
				if(!imageFomatLookup.TryGetValue(extension, out actualFormat)) {
					// couldn't find matching format, perhaps there is no extension or it's not recognised, fallback to default.
					actualFormat = ImageFormat.Default;
				}
			} else {        		
				actualFormat = format.Value;
			}
        	
        	
			if (Interop.LeptonicaApi.pixWrite(filename, handle, actualFormat) != 0) {
				throw new IOException(String.Format("Failed to save image '{0}'.", filename));
			}
		}
		
		#endregion
        
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
			var clonedHandle = Interop.LeptonicaApi.pixClone(handle);
			return new Pix(clonedHandle);
		}
        
		#endregion
        
		#region Image manipulation
		
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
		/// <param name="angle">The angle to rotate by, in radians; clockwise is positive.</param>
		/// <param name="method">The rotation method to use.</param>
		/// <param name="fillColor">The fill color to use for pixels that are brought in from the outside.</param>
		/// <param name="width">The original width; use 0 to avoid embedding</param>
		/// <param name="height">The original height; use 0 to avoid embedding</param>
		/// <returns>The image rotated around it's centre.</returns>
		public Pix Rotate(float angle, RotationMethod method = RotationMethod.AreaMap, RotationFill fillColor = RotationFill.White, int? width = null, int? height = null)
		{			
			if(width == null) width = this.Width;
			if(height == null) height = this.Height;
			
			if(Math.Abs(angle) < VerySmallAngle) return this.Clone();
			
			IntPtr resultHandle;
			
			var rotations = 2 * angle / Math.PI;
			if(Math.Abs(rotations - Math.Floor(rotations)) < VerySmallAngle) {
				// handle special case of orthoganal rotations (90, 180, 270)
				resultHandle = Interop.LeptonicaApi.pixRotateOrth(handle, (int)rotations);
			} else {
				// handle general case			
				resultHandle = Interop.LeptonicaApi.pixRotate(handle, angle, method, fillColor, width.Value, height.Value);
			}
			
			if(resultHandle == IntPtr.Zero) throw new LeptonicaException("Failed to rotate image around it's centre.");
			
			return new Pix(resultHandle);
		}
		
		#endregion
        
		#region Disposal
        
        
		protected override void Dispose(bool disposing)
		{
			var tmpHandle = handle.Handle;
			Interop.LeptonicaApi.pixDestroy(ref tmpHandle);
			this.handle = new HandleRef(this, IntPtr.Zero);
		}
        
		#endregion
        
		
	}
}
