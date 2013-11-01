
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

		internal static bool Is64bit
		{
			get { return IntPtr.Size == sizeof(long); }
		}

		private static readonly TesseractImplementation _implementation;

		static TessApi()
		{
			// first load liblept as tesseract depends on it.
			LeptonicaApi.Initialize(Is64bit);
			// then load unmanaged tesseract dll.

			if (Is64bit)
			{
				_implementation = TessImports64.Initialize();
			}
			else
			{
				_implementation = TessImports32.Initialize();
			}
		}

		// Helper functions
		public static string GetVersion()
		{
			return _implementation.GetVersion();
		}
		
		public static void DeleteText(IntPtr textPtr)
		{
			_implementation.DeleteText(textPtr);
		}
				
		public static void DeleteTextArray(IntPtr arr)		
		{
			_implementation.DeleteTextArray(arr);
		}
		
		public static void DeleteIntArray(IntPtr arr)		
		{
			_implementation.DeleteIntArray(arr);
		}
		
		public static void DeleteBlockList(IntPtr arr)
		{
			_implementation.DeleteBlockList(arr);
		}
		
		// Base API		
		
		/// <summary>
		/// Creates a new BaseAPI instance
		/// </summary>
		/// <returns></returns>
		public static IntPtr BaseApiCreate()
		{
			return _implementation.BaseApiCreate();
		}
		
		
		/// <summary>
		/// Deletes a base api instance.
		/// </summary>
		/// <returns></returns>
		public static void BaseApiDelete(IntPtr ptr)
		{
			_implementation.BaseApiDelete(ptr);
		}
		
		public static int BaseApiInit(IntPtr handle, string datapath, string language, int mode, IntPtr configs, int configs_size, IntPtr vars_vec, int vars_vec_size, IntPtr vars_values, int vars_values_size)
		{
			return _implementation.BaseApiInit(handle, datapath, language, mode, configs, configs_size, vars_vec, vars_vec_size, vars_values, vars_values_size);
		}
		
		
		public static int BaseApiSetVariable(IntPtr handle, string name, string value)
		{
			return _implementation.BaseApiSetVariable(handle, name, value);
		}
		
		
		public static int BaseApiSetDebugVariable(IntPtr handle, string name, string value)		
		{
			return _implementation.BaseApiSetDebugVariable(handle, name, value);
		}
				
		public static int BaseApiGetIntVariable(IntPtr handle, string name, out int value)
		{
			return _implementation.BaseApiGetIntVariable(handle, name, out value);
		}
		
		public static int BaseApiGetBoolVariable(IntPtr handle, string name, out int value)
		{
			return _implementation.BaseApiGetBoolVariable(handle, name, out value);
		}
		
		public static int BaseApiGetDoubleVariable(IntPtr handle, string name, out double value)
		{
			return _implementation.BaseApiGetDoubleVariable(handle, name, out value);
		}
		
		public static string BaseApiGetStringVariable(IntPtr handle, string name)
		{
			return _implementation.BaseApiGetStringVariable(handle, name);
		}
				
		public static int BaseApiGetVariableAsString(IntPtr handle, string name, out string val)
		{
			return _implementation.BaseApiGetVariableAsString(handle, name, out val);
		}

        
        public static void BaseAPISetPageSegMode(IntPtr handle, PageSegMode mode)
		{
			_implementation.BaseAPISetPageSegMode(handle, mode);
		}

        public static PageSegMode BaseAPIGetPageSegMode(IntPtr handle)
		{
			return _implementation.BaseAPIGetPageSegMode(handle);
		}

        // image analysis        
        public static void BaseApiSetImage(IntPtr handle, IntPtr pixHandle)
		{
			_implementation.BaseApiSetImage(handle, pixHandle);
		}

        public static void  BaseApiSetRectangle(IntPtr handle, int left, int top, int width, int height)
		{
			_implementation.BaseApiSetRectangle(handle, left, top, width, height);
		}
                
        public static int BaseApiRecognize(IntPtr handle, IntPtr monitor)
		{
			return _implementation.BaseApiRecognize(handle, monitor);
		}
        
        public static IntPtr BaseAPIAnalyseLayout(IntPtr handle)
		{
			return _implementation.BaseAPIAnalyseLayout(handle);
		}

        public static IntPtr BaseApiGetIterator(IntPtr handle)
		{
			return _implementation.BaseApiGetIterator(handle);
		}

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

        public static IntPtr BaseAPIGetUTF8TextInternal(IntPtr handle)
		{
			return _implementation.BaseAPIGetUTF8TextInternal(handle);
		}

        public static IntPtr BaseAPIGetHOCRTextInternal(IntPtr handle, int pageNum)
		{
			return _implementation.BaseAPIGetHOCRTextInternal(handle, pageNum);
		}

        public static int BaseAPIMeanTextConf(IntPtr handle)
		{
			return _implementation.BaseAPIMeanTextConf(handle);
		}
        
        
        public static void BaseAPIClear(IntPtr handle)
		{
			_implementation.BaseAPIClear(handle);
		}

        // result iterator

        public static void ResultIteratorDelete(IntPtr handle)
		{
			_implementation.ResultIteratorDelete(handle);
		}

        public static IntPtr ResultIteratorCopy(IntPtr handle)
		{
			return _implementation.ResultIteratorCopy(handle);
		}

        public static IntPtr ResultIteratorGetPageIterator(IntPtr handle)
		{
			return _implementation.ResultIteratorGetPageIterator(handle);
		}

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

        public static IntPtr ResultIteratorGetUTF8TextInternal(IntPtr handle, PageIteratorLevel level)
		{
			return _implementation.ResultIteratorGetUTF8TextInternal(handle, level);
		}

        public static float ResultIteratorGetConfidence(IntPtr handle, PageIteratorLevel level)
		{
			return _implementation.ResultIteratorGetConfidence(handle, level);
		}

        // page iterator

        public static void PageIteratorDelete(IntPtr handle)
		{
			_implementation.PageIteratorDelete(handle);
		}

        public static IntPtr PageIteratorCopy(IntPtr handle)
		{
			return _implementation.PageIteratorCopy(handle);
		}

        public static void PageIteratorBegin(IntPtr handle)
		{
			_implementation.PageIteratorBegin(handle);
		}

        public static int PageIteratorNext(IntPtr handle, PageIteratorLevel level)
		{
			return _implementation.PageIteratorNext(handle, level);
		}

        public static int PageIteratorIsAtBeginningOf(IntPtr handle, PageIteratorLevel level)
		{
			return _implementation.PageIteratorIsAtBeginningOf(handle, level);
		}

        public static int PageIteratorIsAtFinalElement(IntPtr handle, PageIteratorLevel level, PageIteratorLevel element)
		{
			return _implementation.PageIteratorIsAtFinalElement(handle, level, element);
		}

        public static int PageIteratorBoundingBox(IntPtr handle, PageIteratorLevel level, out int left, out int top, out int right, out int bottom)
		{
			return _implementation.PageIteratorBoundingBox(handle, level, out left, out top, out right, out bottom);
		}

        public static PolyBlockType PageIteratorBlockType(IntPtr handle)
		{
			return _implementation.PageIteratorBlockType(handle);
		}

        public static IntPtr PageIteratorGetBinaryImage(IntPtr handle, PageIteratorLevel level)
		{
			return _implementation.PageIteratorGetBinaryImage(handle, level);
		}

        public static IntPtr PageIteratorGetImage(IntPtr handle, PageIteratorLevel level, int padding, out int left, out int top)
		{
			return _implementation.PageIteratorGetImage(handle, level, padding, out left, out top);
		}

        public static int PageIteratorBaseline(IntPtr handle, PageIteratorLevel level, out int x1, out int y1, out int x2, out int y2)
		{
			return _implementation.PageIteratorBaseline(handle, level, out x1, out y1, out x2, out y2);
		}

        public static void PageIteratorOrientation(IntPtr handle, out Orientation orientation, out WritingDirection writing_direction, out TextLineOrder textLineOrder, out float deskew_angle)
		{
			_implementation.PageIteratorOrientation(handle, out orientation, out writing_direction, out textLineOrder, out deskew_angle);
		}
	}
}
