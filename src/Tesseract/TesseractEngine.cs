
using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security;
using Tesseract.Internal;

namespace Tesseract
{
	/// <summary>
	/// The tesseract OCR engine.
	/// </summary>
	public class TesseractEngine : DisposableBase
	{		
		/// <summary>
		/// Ties the specified pix to the lifecycle of a page.
		/// </summary>
		class PageDisposalHandle
		{
			readonly Page page;
			readonly Pix pix;
			
			public PageDisposalHandle(Page page, Pix pix)
			{
				this.page = page;
				this.pix = pix;
				page.Disposed += OnPageDisposed;
			}

			void OnPageDisposed(object sender, System.EventArgs e)
			{
				
				
				page.Disposed -= OnPageDisposed;
			}
		}
		
		private HandleRef handle;
        private int processCount = 0;
				
        /// <summary>
        /// Creates a new instance of <see cref="TesseractEngine"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The <paramref name="datapath"/> parameter should point to the directory that contains the 'tessdata' folder
		/// for example if your tesseract language data is installed in <c>C:\Tesseract\tessdata</c> the value of datapath should
        /// be <c>C:\Tesseract</c>. Note that tesseract will use the value of the <c>TESSDATA_PREFIX</c> environment variable if defined,
        /// effectively ignoring the value of <paramref name="datapath"/> parameter.
		/// </para>
        /// </remarks>
        /// <param name="datapath">The path to the parent directory that contains the 'tessdata' directory, ignored if the <c>TESSDATA_PREFIX</c> environment variable is defined.</param>
        /// <param name="language">The language to load, for example 'eng' for English.</param>
        /// <param name="engineMode">The <see cref="EngineMode"/> value to use when initialising the tesseract engine.</param>
		public TesseractEngine(string datapath, string language, EngineMode engineMode = EngineMode.Default)
        {			
            DefaultPageSegMode = PageSegMode.Auto;
            handle = new HandleRef(this, Interop.TessApi.Native.BaseApiCreate());
			
			Initialise(datapath, language, engineMode);
		}

        internal HandleRef Handle
        {
            get { return handle; }
        }
		
		public string Version
		{
            get { return Interop.TessApi.Native.GetVersion(); }
		}
		
		#region Config
		
		/// <summary>
		/// Sets the value of a string variable.
		/// </summary>
		/// <param name="name">The name of the variable.</param>
		/// <param name="value">The new value of the variable.</param>
		/// <returns>Returns <c>True</c> if successful; otherwise <c>False</c>.</returns>
		public bool SetVariable(string name, string value)
		{
            return Interop.TessApi.Native.BaseApiSetVariable(handle, name, value) != 0;
		}
		
		/// <summary>
		/// Sets the value of a boolean variable.
		/// </summary>
		/// <param name="name">The name of the variable.</param>
		/// <param name="value">The new value of the variable.</param>
		/// <returns>Returns <c>True</c> if successful; otherwise <c>False</c>.</returns>
		public bool SetVariable(string name, bool value)
		{
			var strEncodedValue = value ? "TRUE" : "FALSE";
            return Interop.TessApi.Native.BaseApiSetVariable(handle, name, strEncodedValue) != 0;
		}
				
		/// <summary>
		/// Sets the value of a integer variable.
		/// </summary>
		/// <param name="name">The name of the variable.</param>
		/// <param name="value">The new value of the variable.</param>
		/// <returns>Returns <c>True</c> if successful; otherwise <c>False</c>.</returns>
		public bool SetVariable(string name, int value)
		{
			var strEncodedValue = value.ToString("D", CultureInfo.InvariantCulture.NumberFormat);
            return Interop.TessApi.Native.BaseApiSetVariable(handle, name, strEncodedValue) != 0;
		}
		
		/// <summary>
		/// Sets the value of a double variable.
		/// </summary>
		/// <param name="name">The name of the variable.</param>
		/// <param name="value">The new value of the variable.</param>
		/// <returns>Returns <c>True</c> if successful; otherwise <c>False</c>.</returns>
		public bool SetVariable(string name, double value)
		{
			var strEncodedValue = value.ToString("R", CultureInfo.InvariantCulture.NumberFormat);
            return Interop.TessApi.Native.BaseApiSetVariable(handle, name, strEncodedValue) != 0;
		}
		
		public bool SetDebugVariable(string name, string value)
		{
            return Interop.TessApi.Native.BaseApiSetDebugVariable(handle, name, value) != 0;
		}
		
		/// <summary>
		/// Attempts to retrieve the value for a boolean variable.
		/// </summary>
		/// <param name="name">The name of the variable.</param>
		/// <param name="value">The current value of the variable.</param>
		/// <returns>Returns <c>True</c> if successful; otherwise <c>False</c>.</returns>
		public bool TryGetBoolVariable(string name, out bool value)
		{
			int val;
            if (Interop.TessApi.Native.BaseApiGetBoolVariable(handle, name, out val) != 0)
            {
				value = (val != 0);
				return true;
			} else {
				value = false;
				return false;
			}
		}
		
		/// <summary>
		/// Attempts to retrieve the value for an integer variable.
		/// </summary>
		/// <param name="name">The name of the variable.</param>
		/// <param name="value">The current value of the variable.</param>
		/// <returns>Returns <c>True</c> if successful; otherwise <c>False</c>.</returns>
		public bool TryGetIntVariable(string name, out int value)
		{
            return Interop.TessApi.Native.BaseApiGetIntVariable(handle, name, out value) != 0;
		}
		
		/// <summary>
		/// Attempts to retrieve the value for a double variable.
		/// </summary>
		/// <param name="name">The name of the variable.</param>
		/// <param name="value">The current value of the variable.</param>
		/// <returns>Returns <c>True</c> if successful; otherwise <c>False</c>.</returns>
		public bool TryGetDoubleVariable(string name, out double value)
		{
            return Interop.TessApi.Native.BaseApiGetDoubleVariable(handle, name, out value) != 0;
		}	
		
		/// <summary>
		/// Attempts to retrieve the value for a string variable.
		/// </summary>
		/// <param name="name">The name of the variable.</param>
		/// <param name="value">The current value of the variable.</param>
		/// <returns>Returns <c>True</c> if successful; otherwise <c>False</c>.</returns>
		public bool TryGetStringVariable(string name, out string value)
		{
			value = Interop.TessApi.BaseApiGetStringVariable(handle, name);
			return value != null;
		}

		/// <summary>
		/// Gets or sets default <see cref="PageSegMode" /> mode used by <see cref="Tesseract.TesseractEngine.Process(Pix, Rect, PageSegMode?)" />.
		/// </summary>
        public PageSegMode DefaultPageSegMode
        {
            get;
            set;
        }
		
		#endregion
		
		private void Initialise(string datapath, string language, EngineMode engineMode)
		{
			const string TessDataDirectory = "tessdata";
			Guard.RequireNotNullOrEmpty("language", language);
					
			
			// do some minor processing on datapath to fix some common errors (this basically mirrors what tesseract does as of 3.02)
			if(!String.IsNullOrEmpty(datapath)) {
				// remove any trialing '\' or '/' characters
				if(datapath.EndsWith("\\", StringComparison.Ordinal) || datapath.EndsWith("/", StringComparison.Ordinal)) {
					datapath = datapath.Substring(0, datapath.Length-1);
				}
				// remove 'tessdata', if it exists, tesseract will add it when building up the tesseract path
				if(datapath.EndsWith("tessdata", StringComparison.OrdinalIgnoreCase)) {
					datapath = datapath.Substring(0, datapath.Length - TessDataDirectory.Length);
				}
			} 
			
			// log a warning if TESSDATA_PREFIX is set			
			var tessDataPrefix = GetTessDataPrefix();
			if(tessDataPrefix != null) {
				Trace.TraceWarning("Detected that the environment variable 'TESSDATA_PREFIX' is set to '{0}', this will be used as the data directory by tesseract.", tessDataPrefix);
			}

            if (Interop.TessApi.Native.BaseApiInit(handle, datapath, language, (int)engineMode, IntPtr.Zero, 0, IntPtr.Zero, 0, IntPtr.Zero, 0) != 0)
            {
				// Special case logic to handle cleaning up as init has already released the handle if it fails.
				handle = new HandleRef(this, IntPtr.Zero);
				GC.SuppressFinalize(this);
				
				throw new TesseractException(ErrorMessage.Format(1, "Failed to initialise tesseract engine."));
			}
		}

        /// <summary>
        /// Processes the specific image.
        /// </summary>
        /// <remarks>
        /// You can only have one result iterator open at any one time.
        /// </remarks>
        /// <param name="image">The image to process.</param>
        /// <param name="pageSegMode">The page layout analyasis method to use.</param>
        public Page Process(Pix image, PageSegMode? pageSegMode = null)
        {
            return Process(image, null, new Rect(0, 0, image.Width, image.Height), pageSegMode);
        }
        
        /// <summary>
        /// Processes a specified region in the image using the specified page layout analysis mode.
        /// </summary>
        /// <remarks>
        /// You can only have one result iterator open at any one time.
        /// </remarks>
        /// <param name="image">The image to process.</param>
        /// <param name="region">The image region to process.</param>
        /// <param name="pageSegMode">The page layout analyasis method to use.</param>
        /// <returns>A result iterator</returns>
        public Page Process(Pix image, Rect region, PageSegMode? pageSegMode = null)
        {        	
            return Process(image, null, region, pageSegMode);
        }

        
        /// <summary>
        /// Processes the specific image.
        /// </summary>
        /// <remarks>
        /// You can only have one result iterator open at any one time.
        /// </remarks>
        /// <param name="image">The image to process.</param>
        /// <param name="inputName">Sets the input file's name, only needed for training or loading a uzn file.</param>
        /// <param name="pageSegMode">The page layout analyasis method to use.</param>
        public Page Process(Pix image, string inputName, PageSegMode? pageSegMode = null)
        {
            return Process(image, inputName, new Rect(0, 0, image.Width, image.Height), pageSegMode);
        }
        
        /// <summary>
        /// Processes a specified region in the image using the specified page layout analysis mode.
        /// </summary>
        /// <remarks>
        /// You can only have one result iterator open at any one time.
        /// </remarks>
        /// <param name="image">The image to process.</param>
        /// <param name="inputName">Sets the input file's name, only needed for training or loading a uzn file.</param>
        /// <param name="region">The image region to process.</param>
        /// <param name="pageSegMode">The page layout analyasis method to use.</param>
        /// <returns>A result iterator</returns>
        public Page Process(Pix image, string inputName, Rect region, PageSegMode? pageSegMode = null)
        {
            if (image == null) throw new ArgumentNullException("image");
            if (region.X1 < 0 || region.Y1 < 0 || region.X2 > image.Width || region.Y2 > image.Height)
                throw new ArgumentException("The image region to be processed must be within the image bounds.", "region");
            if (processCount > 0) throw new InvalidOperationException("Only one image can be processed at once. Please make sure you dispose of the page once your finished with it.");

            processCount++;

            Interop.TessApi.Native.BaseAPISetPageSegMode(handle, pageSegMode.HasValue ? pageSegMode.Value : DefaultPageSegMode);
            Interop.TessApi.Native.BaseApiSetImage(handle, image.Handle);
            if(!String.IsNullOrEmpty(inputName)) {
                Interop.TessApi.Native.BaseApiSetInputName(handle, inputName);
            }
            var page = new Page(this, image, region);
            page.Disposed += OnIteratorDisposed;
            return page;
        }

        /// <summary>
        /// Process the specified bitmap image.
        /// </summary>
        /// <remarks>
        /// Please consider <see cref="Process(Pix, PageSegMode?)"/> instead. This is because
        /// this method must convert the bitmap to a pix for processing which will add additional overhead.
        /// Leptonica also supports a large number of image pre-processing functions as well.
        /// </remarks>
        /// <param name="image">The image to process.</param>
        /// <param name="pageSegMode">The page segmentation mode.</param>
        /// <returns></returns>
        public Page Process(Bitmap image, PageSegMode? pageSegMode = null)
        {
            return Process(image, new Rect(0, 0, image.Width, image.Height), pageSegMode);
        }
        
        /// <summary>
        /// Process the specified bitmap image.
        /// </summary>
        /// <remarks>
        /// Please consider <see cref="Process(Pix, String, PageSegMode?)"/> instead. This is because
        /// this method must convert the bitmap to a pix for processing which will add additional overhead.
        /// Leptonica also supports a large number of image pre-processing functions as well.
        /// </remarks>
        /// <param name="image">The image to process.</param>
        /// <param name="inputName">Sets the input file's name, only needed for training or loading a uzn file.</param>
        /// <param name="pageSegMode">The page segmentation mode.</param>
        /// <returns></returns>
        public Page Process(Bitmap image, string inputName, PageSegMode? pageSegMode = null)
        {
            return Process(image, inputName, new Rect(0, 0, image.Width, image.Height), pageSegMode);
        }

        /// <summary>
        /// Process the specified bitmap image.
        /// </summary>
        /// <remarks>
        /// Please consider <see cref="TesseractEngine.Process(Pix, Rect, PageSegMode?)"/> instead. This is because
        /// this method must convert the bitmap to a pix for processing which will add additional overhead.
        /// Leptonica also supports a large number of image pre-processing functions as well.
        /// </remarks>
        /// <param name="image">The image to process.</param>
        /// <param name="region">The region of the image to process.</param>
        /// <param name="pageSegMode">The page segmentation mode.</param>
        /// <returns></returns>
        public Page Process(Bitmap image, Rect region, PageSegMode? pageSegMode = null)
        {
            return Process(image, null, region, pageSegMode);
        }
        
         /// <summary>
        /// Process the specified bitmap image.
        /// </summary>
        /// <remarks>
        /// Please consider <see cref="TesseractEngine.Process(Pix, String, Rect, PageSegMode?)"/> instead. This is because
        /// this method must convert the bitmap to a pix for processing which will add additional overhead.
        /// Leptonica also supports a large number of image pre-processing functions as well.
        /// </remarks>
        /// <param name="image">The image to process.</param>
        /// <param name="inputName">Sets the input file's name, only needed for training or loading a uzn file.</param>
        /// <param name="region">The region of the image to process.</param>
        /// <param name="pageSegMode">The page segmentation mode.</param>
        /// <returns></returns>
        public Page Process(Bitmap image, string inputName, Rect region, PageSegMode? pageSegMode = null)
        {
        	var pix = PixConverter.ToPix(image);
            var page = Process(pix, inputName, region, pageSegMode);
            new PageDisposalHandle(page, pix);
            return page;
        }

        protected override void Dispose(bool disposing)
        {
            if (handle.Handle != IntPtr.Zero) {
                Interop.TessApi.Native.BaseApiDelete(handle);
                handle = new HandleRef(this, IntPtr.Zero);
            }
        }

        string GetTessDataPrefix()
        {
        	try {
				return Environment.GetEnvironmentVariable("TESSDATA_PREFIX");
			} catch (SecurityException e) {
				Trace.TraceError("Failed to detect if the environment variable 'TESSDATA_PREFIX' is set: {0}", e.Message);
				return null;
			}
        }
        
        #region Event Handlers

        private void OnIteratorDisposed(object sender, EventArgs e)
        {
            processCount--;
        }

        #endregion
	}
}
