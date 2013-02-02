
using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Tesseract
{
	/// <summary>
	/// Description of Engine.
	/// </summary>
	public class TesseractEngine : DisposableBase
	{
		private IntPtr handle;
        private int processCount = 0;
				
		public TesseractEngine(string datapath, string language, EngineMode engineMode = EngineMode.Default)
        {
            DefaultPageSegMode = PageSegMode.Auto;
			handle = Interop.TessApi.BaseApiCreate();
			
			Initialise(datapath, language, engineMode);
		}

        public IntPtr Handle
        {
            get { return handle; }
        }
		
		public string Version
		{
            get { return Interop.TessApi.GetVersion(); }
		}
		
		#region Config
		
		public bool SetVariable(string name, string value)
		{
            return Interop.TessApi.BaseApiSetVariable(handle, name, value) != 0;
		}
		
		public bool SetDebugVariable(string name, string value)
		{
            return Interop.TessApi.BaseApiSetDebugVariable(handle, name, value) != 0;
		}
		
		public bool TryGetBoolVariable(string name, out bool value)
		{
			int val;
            if (Interop.TessApi.BaseApiGetIntVariable(handle, name, out val) != 0) {
				value = (val != 0);
				return true;
			} else {
				value = false;
				return false;
			}
		}
		
		public bool TryGetIntVariable(string name, out int value)
		{
			return Interop.TessApi.BaseApiGetIntVariable(handle, name, out value) != 0;
		}
		
		public bool TryGetDoubleVariable(string name, out double value)
		{
            return Interop.TessApi.BaseApiGetDoubleVariable(handle, name, out value) != 0;
		}	
		
		public bool TryGetStringVariable(string name, out string value)
		{
            return Interop.TessApi.BaseApiGetVariableAsString(handle, name, out value) != 0;
		}

        public PageSegMode DefaultPageSegMode
        {
            get;
            set;
        }
		
		#endregion
		
		private void Initialise(string datapath, string language, EngineMode engineMode)
		{
            if (Interop.TessApi.BaseApiInit(handle, datapath, language, (int)engineMode, IntPtr.Zero, 0, IntPtr.Zero, 0, IntPtr.Zero, 0) != 0)
            {
				// Special case logic to handle cleaning up as init has already released the handle if it fails.
				handle = IntPtr.Zero;
				GC.SuppressFinalize(this);
				
				throw new TesseractException("Failed to initialise tesseract engine.");
			}
		}

        public Page Process(Pix image, PageSegMode? pageSegMode = null)
        {
            return Process(image, new Rect(0, 0, image.Width, image.Height), pageSegMode);
        }

        /// <summary>
        /// Processes the specific image.
        /// </summary>
        /// <remarks>
        /// You can only have one result iterator open at any one time.
        /// </remarks>
        /// <param name="image">The image to process.</param>
        /// <param name="region">The image region to process.</param>
        /// <returns>A result iterator</returns>
        public Page Process(Pix image, Rect region, PageSegMode? pageSegMode = null)
        {
            if (image == null) throw new ArgumentNullException("image");
            if (region.X1 < 0 || region.Y1 < 0 || region.X2 > image.Width || region.Y2 > image.Height)
                throw new ArgumentException("The image region to be processed must be within the image bounds.", "region");
            if (processCount > 0) throw new InvalidOperationException("Only one image can be processed at once. Please make sure you dispose of the page once your finished with it.");

            processCount++;

            Interop.TessApi.BaseAPISetPageSegMode(handle, pageSegMode.HasValue ? pageSegMode.Value : DefaultPageSegMode);
            Interop.TessApi.BaseApiSetImage(handle, image.Handle);
            Interop.TessApi.BaseApiSetRectangle(handle, region.X1, region.Y1, region.Width, region.Height);

            var page = new Page(this);
            page.Disposed += OnIteratorDisposed;
            return page;
        }

        /// <summary>
        /// Process the specified bitmap image.
        /// </summary>
        /// <remarks>
        /// Please consider <see cref="Process(Pix image,PageSegMode? pageSegMode)"/> instead. This is because
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
        /// Please consider <see cref="Process(Pix image, Rect region, PageSegMode? pageSegMode)"/> instead. This is because
        /// this method must convert the bitmap to a pix for processing which will add additional overhead.
        /// Leptonica also supports a large number of image pre-processing functions as well.
        /// </remarks>
        /// <param name="image">The image to process.</param>
        /// <param name="region">The region of the image to process.</param>
        /// <param name="pageSegMode">The page segmentation mode.</param>
        /// <returns></returns>
        public Page Process(Bitmap image, Rect region, PageSegMode? pageSegMode = null)
        {
            using (var pix = PixConverter.ToPix(image)) {
                return Process(pix, region, pageSegMode);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (handle != IntPtr.Zero) {
                Interop.TessApi.BaseApiDelete(handle);
                handle = IntPtr.Zero;
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
