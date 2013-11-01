
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Tesseract.Interop
{
	internal static class TessImports32
	{
		internal static TesseractImplementation Initialize()
		{
			WindowsLibraryLoader.Instance.LoadLibrary(Constants.TesseractDllName32);

			var impl = new TesseractImplementation();
			impl.GetVersion = GetVersion;
			impl.DeleteText = DeleteText;
			impl.DeleteTextArray = DeleteTextArray;
			impl.DeleteIntArray = DeleteIntArray;
			impl.DeleteBlockList = DeleteBlockList;
			impl.BaseApiCreate = BaseApiCreate;
			impl.BaseApiDelete = BaseApiDelete;
			impl.BaseApiInit = BaseApiInit;
			impl.BaseApiSetVariable = BaseApiSetVariable;
			impl.BaseApiSetDebugVariable = BaseApiSetDebugVariable;
			impl.BaseApiGetIntVariable = BaseApiGetIntVariable;
			impl.BaseApiGetBoolVariable = BaseApiGetBoolVariable;
			impl.BaseApiGetDoubleVariable = BaseApiGetDoubleVariable;
			impl.BaseApiGetStringVariable = BaseApiGetStringVariable;
			impl.BaseApiGetVariableAsString = BaseApiGetVariableAsString;
			impl.BaseAPISetPageSegMode = BaseAPISetPageSegMode;
			impl.BaseAPIGetPageSegMode = BaseAPIGetPageSegMode;
			impl.BaseApiSetImage = BaseApiSetImage;
			impl.BaseApiSetRectangle = BaseApiSetRectangle;
			impl.BaseApiRecognize = BaseApiRecognize;
			impl.BaseAPIAnalyseLayout = BaseAPIAnalyseLayout;
			impl.BaseApiGetIterator = BaseApiGetIterator;
			impl.BaseAPIGetUTF8TextInternal = BaseAPIGetUTF8TextInternal;
			impl.BaseAPIGetHOCRTextInternal = BaseAPIGetHOCRTextInternal;
			impl.BaseAPIMeanTextConf = BaseAPIMeanTextConf;
			impl.BaseAPIClear = BaseAPIClear;
			impl.ResultIteratorDelete = ResultIteratorDelete;
			impl.ResultIteratorCopy = ResultIteratorCopy;
			impl.ResultIteratorGetPageIterator = ResultIteratorGetPageIterator;
			impl.ResultIteratorGetUTF8TextInternal = ResultIteratorGetUTF8TextInternal;
			impl.ResultIteratorGetConfidence = ResultIteratorGetConfidence;
			impl.PageIteratorDelete = PageIteratorDelete;
			impl.PageIteratorCopy = PageIteratorCopy;
			impl.PageIteratorBegin = PageIteratorBegin;
			impl.PageIteratorNext = PageIteratorNext;
			impl.PageIteratorIsAtBeginningOf = PageIteratorIsAtBeginningOf;
			impl.PageIteratorIsAtFinalElement = PageIteratorIsAtFinalElement;
			impl.PageIteratorBoundingBox = PageIteratorBoundingBox;
			impl.PageIteratorBlockType = PageIteratorBlockType;
			impl.PageIteratorGetBinaryImage = PageIteratorGetBinaryImage;
			impl.PageIteratorGetImage = PageIteratorGetImage;
			impl.PageIteratorBaseline = PageIteratorBaseline;
			impl.PageIteratorOrientation = PageIteratorOrientation;

			return impl;
		}

		[DllImport(Constants.TesseractDllName32, CallingConvention = CallingConvention.Cdecl, EntryPoint="TessVersion")]
		internal static extern string GetVersion();
		
		[DllImport(Constants.TesseractDllName32, CallingConvention = CallingConvention.Cdecl, EntryPoint="TessDeleteText")]
		internal static extern void DeleteText(IntPtr textPtr);
				
		[DllImport(Constants.TesseractDllName32, CallingConvention = CallingConvention.Cdecl, EntryPoint="TessDeleteTextArray")]
		internal static extern void DeleteTextArray(IntPtr arr);		
		
		[DllImport(Constants.TesseractDllName32, CallingConvention = CallingConvention.Cdecl, EntryPoint="TessDeleteIntArray")]
		internal static extern void DeleteIntArray(IntPtr arr);		
		
		[DllImport(Constants.TesseractDllName32, CallingConvention = CallingConvention.Cdecl, EntryPoint="TessDeleteBlockList")]
		internal static extern void DeleteBlockList(IntPtr arr);
		
		[DllImport(Constants.TesseractDllName32, CallingConvention = CallingConvention.Cdecl, EntryPoint="TessBaseAPICreate")]
		internal static extern IntPtr BaseApiCreate();

		[DllImport(Constants.TesseractDllName32, CallingConvention = CallingConvention.Cdecl, EntryPoint="TessBaseAPIDelete")]
		internal static extern void BaseApiDelete(IntPtr ptr);
		
		[DllImport(Constants.TesseractDllName32, CallingConvention = CallingConvention.Cdecl, EntryPoint="TessBaseAPIInit1")]
		internal static extern int BaseApiInit(IntPtr handle,
		                              string datapath,
		                              string language,
		                              int mode,
		                              IntPtr configs, int configs_size,
		                              IntPtr vars_vec, int vars_vec_size, IntPtr vars_values, int vars_values_size);
				
		[DllImport(Constants.TesseractDllName32, CallingConvention = CallingConvention.Cdecl, EntryPoint="TessBaseAPISetVariable")]
		internal static extern int BaseApiSetVariable(IntPtr handle, string name, string value);		
		
		[DllImport(Constants.TesseractDllName32, CallingConvention = CallingConvention.Cdecl, EntryPoint="TessBaseAPISetDebugVariable")]
		internal static extern int BaseApiSetDebugVariable(IntPtr handle, string name, string value);		
				
		[DllImport(Constants.TesseractDllName32, CallingConvention = CallingConvention.Cdecl, EntryPoint="TessBaseAPIGetIntVariable")]
		internal static extern int BaseApiGetIntVariable(IntPtr handle, string name, out int value);
		
		[DllImport(Constants.TesseractDllName32, CallingConvention = CallingConvention.Cdecl, EntryPoint="TessBaseAPIGetBoolVariable")]
		internal static extern int BaseApiGetBoolVariable(IntPtr handle, string name, out int value);
		
		[DllImport(Constants.TesseractDllName32, CallingConvention = CallingConvention.Cdecl, EntryPoint="TessBaseAPIGetDoubleVariable")]
		internal static extern int BaseApiGetDoubleVariable(IntPtr handle, string name, out double value);
		
		[DllImport(Constants.TesseractDllName32, CallingConvention = CallingConvention.Cdecl, EntryPoint="TessBaseAPIGetStringVariable")]
		internal static extern string BaseApiGetStringVariable(IntPtr handle, string name);
				
		[DllImport(Constants.TesseractDllName32, CallingConvention = CallingConvention.Cdecl, EntryPoint="TessBaseAPIGetStringVariable")]
		internal static extern int BaseApiGetVariableAsString(IntPtr handle, string name, out string val);
        
		[DllImport(Constants.TesseractDllName32, CallingConvention = CallingConvention.Cdecl, EntryPoint="TessBaseAPISetPageSegMode")]
        internal static extern void BaseAPISetPageSegMode(IntPtr handle, PageSegMode mode);

        [DllImport(Constants.TesseractDllName32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetPageSegMode")]
        internal static extern PageSegMode BaseAPIGetPageSegMode(IntPtr handle);

        [DllImport(Constants.TesseractDllName32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPISetImage2")]
        internal static extern void BaseApiSetImage(IntPtr handle, IntPtr pixHandle);

        [DllImport(Constants.TesseractDllName32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPISetRectangle")]
        internal static extern void  BaseApiSetRectangle(IntPtr handle, int left, int top, int width, int height);
                
        [DllImport(Constants.TesseractDllName32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIRecognize")]
        internal static extern int BaseApiRecognize(IntPtr handle, IntPtr monitor);
        
        [DllImport(Constants.TesseractDllName32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIAnalyseLayout")]
        internal static extern IntPtr BaseAPIAnalyseLayout(IntPtr handle);

        [DllImport(Constants.TesseractDllName32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetIterator")]
        internal static extern IntPtr BaseApiGetIterator(IntPtr handle);

        [DllImport(Constants.TesseractDllName32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetUTF8Text")]
        internal static extern IntPtr BaseAPIGetUTF8TextInternal(IntPtr handle);

        [DllImport(Constants.TesseractDllName32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetHOCRText")]
        internal static extern IntPtr BaseAPIGetHOCRTextInternal(IntPtr handle, int pageNum);

        [DllImport(Constants.TesseractDllName32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIMeanTextConf")]
        internal static extern int BaseAPIMeanTextConf(IntPtr handle);
               
        [DllImport(Constants.TesseractDllName32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIClear")]
        internal static extern void BaseAPIClear(IntPtr handle);

        [DllImport(Constants.TesseractDllName32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorDelete")]
        internal static extern void ResultIteratorDelete(IntPtr handle);

        [DllImport(Constants.TesseractDllName32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorCopy")]
        internal static extern IntPtr ResultIteratorCopy(IntPtr handle);

        [DllImport(Constants.TesseractDllName32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorGetPageIterator")]
        internal static extern IntPtr ResultIteratorGetPageIterator(IntPtr handle);

        [DllImport(Constants.TesseractDllName32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorGetUTF8Text")]
        internal static extern IntPtr ResultIteratorGetUTF8TextInternal(IntPtr handle, PageIteratorLevel level);

        [DllImport(Constants.TesseractDllName32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorConfidence")]
        internal static extern float ResultIteratorGetConfidence(IntPtr handle, PageIteratorLevel level);

        [DllImport(Constants.TesseractDllName32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorDelete")]
        internal static extern void PageIteratorDelete(IntPtr handle);

        [DllImport(Constants.TesseractDllName32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorCopy")]
        internal static extern IntPtr PageIteratorCopy(IntPtr handle);

        [DllImport(Constants.TesseractDllName32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorBegin")]
        internal static extern void PageIteratorBegin(IntPtr handle);

        [DllImport(Constants.TesseractDllName32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorNext")]
        internal static extern int PageIteratorNext(IntPtr handle, PageIteratorLevel level);

        [DllImport(Constants.TesseractDllName32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorIsAtBeginningOf")]
        internal static extern int PageIteratorIsAtBeginningOf(IntPtr handle, PageIteratorLevel level);

        [DllImport(Constants.TesseractDllName32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorIsAtFinalElement")]
        internal static extern int PageIteratorIsAtFinalElement(IntPtr handle, PageIteratorLevel level, PageIteratorLevel element);

        [DllImport(Constants.TesseractDllName32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorBoundingBox")]
        internal static extern int PageIteratorBoundingBox(IntPtr handle, PageIteratorLevel level, out int left, out int top, out int right, out int bottom);

        [DllImport(Constants.TesseractDllName32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorBlockType")]
        internal static extern PolyBlockType PageIteratorBlockType(IntPtr handle);

        [DllImport(Constants.TesseractDllName32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorGetBinaryImage")]
        internal static extern IntPtr PageIteratorGetBinaryImage(IntPtr handle, PageIteratorLevel level);

        [DllImport(Constants.TesseractDllName32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorGetImage")]
        internal static extern IntPtr PageIteratorGetImage(IntPtr handle, PageIteratorLevel level, int padding, out int left, out int top);

        [DllImport(Constants.TesseractDllName32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorBaseline")]
        internal static extern int PageIteratorBaseline(IntPtr handle, PageIteratorLevel level, out int x1, out int y1, out int x2, out int y2);

        [DllImport(Constants.TesseractDllName32, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorOrientation")]
        internal static extern void PageIteratorOrientation(IntPtr handle, out Orientation orientation, out WritingDirection writing_direction, out TextLineOrder textLineOrder, out float deskew_angle);
	}
}
