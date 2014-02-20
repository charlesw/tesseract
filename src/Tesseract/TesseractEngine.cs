
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using Tesseract.Internal;

namespace Tesseract
{
	/// <summary>
	/// Description of Engine.
	/// </summary>
	public class TesseractEngine : DisposableBase
	{
		private HandleRef handle;
        private int processCount = 0;
				
		public TesseractEngine(string datapath, string language, EngineMode engineMode = EngineMode.Default)
			: this(datapath, language, engineMode, null, false)
        {
		}
		
			
		public TesseractEngine(string datapath, string language, EngineMode engineMode = EngineMode.Default, IDictionary<string, object> initValues = null, bool setOnlyNonDebugVariables = false)
        {
            DefaultPageSegMode = PageSegMode.Auto;
            handle = new HandleRef(this, Interop.TessApi.BaseApiCreate());
			
			Initialise(datapath, language, engineMode, initValues, setOnlyNonDebugVariables);			
		}

        public HandleRef Handle
        {
            get { return handle; }
        }
		
		public string Version
		{
            get { return Interop.TessApi.GetVersion(); }
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
            return Interop.TessApi.BaseApiSetVariable(handle, name, value) != 0;
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
            return Interop.TessApi.BaseApiSetVariable(handle, name, strEncodedValue) != 0;
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
            return Interop.TessApi.BaseApiSetVariable(handle, name, strEncodedValue) != 0;
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
            return Interop.TessApi.BaseApiSetVariable(handle, name, strEncodedValue) != 0;
		}
		
		public bool SetDebugVariable(string name, string value)
		{
            return Interop.TessApi.BaseApiSetDebugVariable(handle, name, value) != 0;
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
            if (Interop.TessApi.BaseApiGetBoolVariable(handle, name, out val) != 0) {
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
			return Interop.TessApi.BaseApiGetIntVariable(handle, name, out value) != 0;
		}
		
		/// <summary>
		/// Attempts to retrieve the value for a double variable.
		/// </summary>
		/// <param name="name">The name of the variable.</param>
		/// <param name="value">The current value of the variable.</param>
		/// <returns>Returns <c>True</c> if successful; otherwise <c>False</c>.</returns>
		public bool TryGetDoubleVariable(string name, out double value)
		{
            return Interop.TessApi.BaseApiGetDoubleVariable(handle, name, out value) != 0;
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
		
		private void Initialise(string datapath, string language, EngineMode engineMode, IDictionary<string, object> values, bool setOnlyNonDebugVariables)
		{
			string[] varNames = null;
			string[] varValues = null;
			if(values != null) {
				varNames = new string[values.Count];
				varValues = new string[values.Count];
				int i = 0;
				foreach (var pair in values) {
					Guard.Require("values", !String.IsNullOrEmpty(pair.Key), "Variable must have a name.");
					
					varNames[i] = pair.Key;
					string varValue;
					if(TessConvert.TryToString(pair.Value, out varValue)) {
						varValues[i] = varValue;
					} else {
						if(pair.Value != null) {
							throw new ArgumentException(
								String.Format("Variable '{0}': Must not be null.", pair.Key),
								"values"						
							);
						} else {
							throw new ArgumentException(
								String.Format("Variable '{0}': The type '{1}' is not supported.", pair.Key, pair.Value.GetType()),
								"values"						
							);
						}
					}
					i++;
				}
			}
			
			if (Interop.TessApi.BaseApiInit(handle, datapath, language, (int)engineMode, null, 0, varNames, varValues,  new UIntPtr(  varValues != null ? (uint)varValues.Length : 0 ), setOnlyNonDebugVariables) != 0)
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
            return Process(image, null, new Rect(0, 0, image.Width, image.Height), pageSegMode);
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

            Interop.TessApi.BaseAPISetPageSegMode(handle, pageSegMode.HasValue ? pageSegMode.Value : DefaultPageSegMode);
            Interop.TessApi.BaseApiSetImage(handle, image.Handle);
            Interop.TessApi.BaseApiSetRectangle(handle, region.X1, region.Y1, region.Width, region.Height);
            if(!String.IsNullOrEmpty(inputName)) {
            	Interop.TessApi.BaseApiSetInputName(handle, inputName);
            }
            var page = new Page(this);
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
            using (var pix = PixConverter.ToPix(image)) {
                return Process(pix, inputName, region, pageSegMode);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (handle.Handle != IntPtr.Zero) {
                Interop.TessApi.BaseApiDelete(handle);
                handle = new HandleRef(this, IntPtr.Zero);
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
