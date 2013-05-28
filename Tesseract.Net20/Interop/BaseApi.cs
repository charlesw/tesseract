
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Tesseract.Interop
{
	public static class TessApi
	{
        public const string htmlBeginTag =
            "<!DOCTYPE html PUBLIC \"-//W3C//DTD HTML 4.01 Transitional//EN\""
            + " \"http://www.w3.org/TR/html4/loose.dtd\">\n"
            + "<html>\n<head>\n<title></title>\n"
            + "<meta http-equiv=\"Content-Type\" content=\"text/html;"
            + "charset=utf-8\" />\n<meta name='ocr-system' content='tesseract'/>\n"
            + "</head>\n<body>\n";

        public const string htmlEndTag = "</body>\n</html>\n";

        static TessApi()
        {
            // first load liblept as tesseract depends on it.
            WindowsLibraryLoader.Instance.LoadLibrary(Constants.LeptonicaDllName);
            // now load unmanaged tesseract dll.
            WindowsLibraryLoader.Instance.LoadLibrary(Constants.TesseractDllName);
        }

		// Helper functions
		[DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint="TessVersion")]
		public static extern string GetVersion();
		
		[DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint="TessDeleteText")]
		public static extern void DeleteText(IntPtr textPtr);
				
		[DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint="TessDeleteTextArray")]
		public static extern void DeleteTextArray(IntPtr arr);		
		
		[DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint="TessDeleteIntArray")]
		public static extern void DeleteIntArray(IntPtr arr);		
		
		[DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint="TessDeleteBlockList")]
		public static extern void DeleteBlockList(IntPtr arr);
		
		// Base API		
		
		/// <summary>
		/// Creates a new BaseAPI instance
		/// </summary>
		/// <returns></returns>
		[DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint="TessBaseAPICreate")]
		public static extern IntPtr BaseApiCreate();
		
		
		/// <summary>
		/// Deletes a base api instance.
		/// </summary>
		/// <returns></returns>
		[DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint="TessBaseAPIDelete")]
		public static extern void BaseApiDelete(IntPtr ptr);
		
		[DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint="TessBaseAPIInit1")]
		public static extern int BaseApiInit(IntPtr handle,
		                              string datapath,
		                              string language,
		                              int mode,
		                              IntPtr configs, int configs_size,
		                              IntPtr vars_vec, int vars_vec_size, IntPtr vars_values, int vars_values_size);
		
		
		[DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint="TessBaseAPISetVariable")]
		public static extern int BaseApiSetVariable(IntPtr handle, string name, string value);
		
		
		[DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint="TessBaseAPISetDebugVariable")]
		public static extern int BaseApiSetDebugVariable(IntPtr handle, string name, string value);		
				
		[DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint="TessBaseAPIGetIntVariable")]
		public static extern int BaseApiGetIntVariable(IntPtr handle, string name, out int value);
		
		[DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint="TessBaseAPIGetBoolVariable")]
		public static extern int BaseApiGetBoolVariable(IntPtr handle, string name, out int value);
		
		[DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint="TessBaseAPIGetDoubleVariable")]
		public static extern int BaseApiGetDoubleVariable(IntPtr handle, string name, out double value);
		
		[DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint="TessBaseAPIGetStringVariable")]
		public static extern string BaseApiGetStringVariable(IntPtr handle, string name);
				
		[DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint="TessBaseAPIGetStringVariable")]
		public static extern int BaseApiGetVariableAsString(IntPtr handle, string name, out string val);

        
		[DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint="TessBaseAPISetPageSegMode")]
        public static extern void BaseAPISetPageSegMode(IntPtr handle, PageSegMode mode);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetPageSegMode")]
        public static extern PageSegMode BaseAPIGetPageSegMode(IntPtr handle);

        // image analysis        
        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPISetImage2")]
        public static extern void BaseApiSetImage(IntPtr handle, IntPtr pixHandle);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPISetRectangle")]
        public static extern void  BaseApiSetRectangle(IntPtr handle, int left, int top, int width, int height);
                
        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIRecognize")]
        public static extern int BaseApiRecognize(IntPtr handle, IntPtr monitor);
        
        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIAnalyseLayout")]
        public static extern IntPtr BaseAPIAnalyseLayout(IntPtr handle);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetIterator")]
        public static extern IntPtr BaseApiGetIterator(IntPtr handle);

        public static string BaseAPIGetUTF8Text(IntPtr handle)
        {
            IntPtr txtHandle = BaseAPIGetUTF8TextInternal(handle);
            if (txtHandle != IntPtr.Zero) {
                var result = MarshalHelper.PtrToString(txtHandle, Encoding.UTF8);
                TessApi.DeleteText(txtHandle);
                return result;
            } else {
                return null;
            }
        }

        public static string BaseAPIGetHOCRText(IntPtr handle, int pageNum)
        {
            IntPtr txtHandle = BaseAPIGetHOCRTextInternal(handle, pageNum);
            if (txtHandle != IntPtr.Zero)
            {
                var result = MarshalHelper.PtrToString(txtHandle, Encoding.UTF8);
                TessApi.DeleteText(txtHandle);
                return htmlBeginTag + result + htmlEndTag;
            }
            else
            {
                return null;
            }
        }

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetUTF8Text")]
        private static extern IntPtr BaseAPIGetUTF8TextInternal(IntPtr handle);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetHOCRText")]
        private static extern IntPtr BaseAPIGetHOCRTextInternal(IntPtr handle, int pageNum);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIMeanTextConf")]
        public static extern int BaseAPIMeanTextConf(IntPtr handle);
        
        
        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIClear")]
        public static extern void BaseAPIClear(IntPtr handle);

        // result iterator

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorDelete")]
        public static extern void ResultIteratorDelete(IntPtr handle);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorCopy")]
        public static extern IntPtr ResultIteratorCopy(IntPtr handle);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorGetPageIterator")]
        public static extern IntPtr ResultIteratorGetPageIterator(IntPtr handle);

        public static string ResultIteratorGetUTF8Text(IntPtr handle, PageIteratorLevel level)
        {
            IntPtr txtHandle = ResultIteratorGetUTF8TextInternal(handle, level);
            if (txtHandle != IntPtr.Zero) {
                var result = MarshalHelper.PtrToString(txtHandle, Encoding.UTF8);
                TessApi.DeleteText(txtHandle);
                return result;
            } else {
                return null;
            }
        }

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorGetUTF8Text")]
        private static extern IntPtr ResultIteratorGetUTF8TextInternal(IntPtr handle, PageIteratorLevel level);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorConfidence")]
        public static extern float ResultIteratorGetConfidence(IntPtr handle, PageIteratorLevel level);

        // page iterator

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorDelete")]
        public static extern void PageIteratorDelete(IntPtr handle);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorCopy")]
        public static extern IntPtr PageIteratorCopy(IntPtr handle);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorBegin")]
        public static extern void PageIteratorBegin(IntPtr handle);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorNext")]
        public static extern int PageIteratorNext(IntPtr handle, PageIteratorLevel level);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorIsAtBeginningOf")]
        public static extern int PageIteratorIsAtBeginningOf(IntPtr handle, PageIteratorLevel level);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorIsAtFinalElement")]
        public static extern int PageIteratorIsAtFinalElement(IntPtr handle, PageIteratorLevel level, PageIteratorLevel element);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorBoundingBox")]
        public static extern int PageIteratorBoundingBox(IntPtr handle, PageIteratorLevel level, out int left, out int top, out int right, out int bottom);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorBlockType")]
        public static extern PolyBlockType PageIteratorBlockType(IntPtr handle);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorGetBinaryImage")]
        public static extern IntPtr PageIteratorGetBinaryImage(IntPtr handle, PageIteratorLevel level);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorGetImage")]
        public static extern IntPtr PageIteratorGetImage(IntPtr handle, PageIteratorLevel level, int padding, out int left, out int top);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorBaseline")]
        public static extern int PageIteratorBaseline(IntPtr handle, PageIteratorLevel level, out int x1, out int y1, out int x2, out int y2);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorOrientation")]
        public static extern void PageIteratorOrientation(IntPtr handle, out Orientation orientation, out WritingDirection writing_direction, out TextLineOrder textLineOrder, out float deskew_angle);
	}
}
