
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
		
		// String functions
				
		[DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint="TessStringCreate")]
		public static extern IntPtr TessStringCreate( string value);
		
		[DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint="TessStringDelete")]
		public static extern void TessStringDelete(IntPtr handle);
		
		[DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint="TessStringGetCStr")]
		public static extern string TessStringGetCStr(IntPtr handle);

		
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
		public static extern void BaseApiDelete(HandleRef ptr);
		
		[DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint="TessBaseAPIInit")]
		public static extern int BaseApiInit(HandleRef handle,
		                              string datapath,
		                              string language,
		                              int mode,
		                              IntPtr configs, int configs_size,
		                              IntPtr vars_vec, int vars_vec_size, 
		                              IntPtr vars_values, int vars_values_size,
		                              bool set_only_init_params);
		
		
		[DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint="TessBaseAPISetVariable")]
		public static extern int BaseApiSetVariable(HandleRef handle, string name, string value);
		
		
		[DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint="TessBaseAPISetDebugVariable")]
		public static extern int BaseApiSetDebugVariable(HandleRef handle, string name, string value);		
				
		[DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint="TessBaseAPIGetIntVariable")]
		public static extern int BaseApiGetIntVariable(HandleRef handle, string name, out int value);
		
		[DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint="TessBaseAPIGetBoolVariable")]
		public static extern int BaseApiGetBoolVariable(HandleRef handle, string name, out int value);
		
		[DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint="TessBaseAPIGetDoubleVariable")]
		public static extern int BaseApiGetDoubleVariable(HandleRef handle, string name, out double value);
		
		public static string BaseApiGetStringVariable(HandleRef handle, string name)
		{
			var resultHandle = BaseApiGetStringVariableInternal(handle, name);
			
			return Marshal.PtrToStringAnsi(resultHandle);
		}
		
		[DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint="TessBaseAPIGetStringVariable")]
		public static extern IntPtr BaseApiGetStringVariableInternal(HandleRef handle, string name);
				
		[DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint="TessBaseAPISetPageSegMode")]
        public static extern void BaseAPISetPageSegMode(HandleRef handle, PageSegMode mode);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetPageSegMode")]
        public static extern PageSegMode BaseAPIGetPageSegMode(HandleRef handle);

        // image analysis        
        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPISetImage2")]
        public static extern void BaseApiSetImage(HandleRef handle, HandleRef pixHandle);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPISetInputName")]
        public static extern void BaseApiSetInputName(HandleRef handle, string value);

        
        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPISetRectangle")]
        public static extern void  BaseApiSetRectangle(HandleRef handle, int left, int top, int width, int height);
                
        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIRecognize")]
        public static extern int BaseApiRecognize(HandleRef handle, HandleRef monitor);
        
        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIAnalyseLayout")]
        public static extern IntPtr BaseAPIAnalyseLayout(HandleRef handle);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetIterator")]
        public static extern IntPtr BaseApiGetIterator(HandleRef handle);

        public static string BaseAPIGetUTF8Text(HandleRef handle)
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

        public static string BaseAPIGetHOCRText(HandleRef handle, int pageNum)
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
        private static extern IntPtr BaseAPIGetUTF8TextInternal(HandleRef handle);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetHOCRText")]
        private static extern IntPtr BaseAPIGetHOCRTextInternal(HandleRef handle, int pageNum);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIMeanTextConf")]
        public static extern int BaseAPIMeanTextConf(HandleRef handle);
        
        
        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIClear")]
        public static extern void BaseAPIClear(HandleRef handle);

        // result iterator

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorDelete")]
        public static extern void ResultIteratorDelete(HandleRef handle);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorCopy")]
        public static extern IntPtr ResultIteratorCopy(HandleRef handle);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorGetPageIterator")]
        public static extern IntPtr ResultIteratorGetPageIterator(HandleRef handle);

        public static string ResultIteratorGetUTF8Text(HandleRef handle, PageIteratorLevel level)
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
        private static extern IntPtr ResultIteratorGetUTF8TextInternal(HandleRef handle, PageIteratorLevel level);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorConfidence")]
        public static extern float ResultIteratorGetConfidence(HandleRef handle, PageIteratorLevel level);

        // page iterator

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorDelete")]
        public static extern void PageIteratorDelete(HandleRef handle);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorCopy")]
        public static extern IntPtr PageIteratorCopy(HandleRef handle);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorBegin")]
        public static extern void PageIteratorBegin(HandleRef handle);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorNext")]
        public static extern int PageIteratorNext(HandleRef handle, PageIteratorLevel level);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorIsAtBeginningOf")]
        public static extern int PageIteratorIsAtBeginningOf(HandleRef handle, PageIteratorLevel level);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorIsAtFinalElement")]
        public static extern int PageIteratorIsAtFinalElement(HandleRef handle, PageIteratorLevel level, PageIteratorLevel element);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorBoundingBox")]
        public static extern int PageIteratorBoundingBox(HandleRef handle, PageIteratorLevel level, out int left, out int top, out int right, out int bottom);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorBlockType")]
        public static extern PolyBlockType PageIteratorBlockType(HandleRef handle);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorGetBinaryImage")]
        public static extern IntPtr PageIteratorGetBinaryImage(HandleRef handle, PageIteratorLevel level);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorGetImage")]
        public static extern IntPtr PageIteratorGetImage(HandleRef handle, PageIteratorLevel level, int padding, out int left, out int top);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorBaseline")]
        public static extern int PageIteratorBaseline(HandleRef handle, PageIteratorLevel level, out int x1, out int y1, out int x2, out int y2);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorOrientation")]
        public static extern void PageIteratorOrientation(HandleRef handle, out Orientation orientation, out WritingDirection writing_direction, out TextLineOrder textLineOrder, out float deskew_angle);		
	}
}
