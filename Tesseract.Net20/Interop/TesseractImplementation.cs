using System;
namespace Tesseract.Interop
{

	internal class TesseractImplementation
	{
		internal delegate string GetVersionFn();
		internal GetVersionFn GetVersion;

		internal delegate void DeleteTextFn(IntPtr textPtr);
		internal DeleteTextFn DeleteText;

		internal delegate void DeleteTextArrayFn(IntPtr arr);
		internal DeleteTextArrayFn DeleteTextArray;

		internal delegate void DeleteIntArrayFn(IntPtr arr);
		internal DeleteIntArrayFn DeleteIntArray;

		internal delegate void DeleteBlockListFn(IntPtr arr);
		internal DeleteBlockListFn DeleteBlockList;

		internal delegate IntPtr BaseApiCreateFn();
		internal BaseApiCreateFn BaseApiCreate;

		internal delegate void BaseApiDeleteFn(IntPtr ptr);
		internal BaseApiDeleteFn BaseApiDelete;

		internal delegate int BaseApiInitFn(IntPtr handle, string datapath, string language, int mode, IntPtr configs, int configs_size, IntPtr vars_vec, int vars_vec_size, IntPtr vars_values, int vars_values_size);
		internal BaseApiInitFn BaseApiInit;

		internal delegate int BaseApiSetVariableFn(IntPtr handle, string name, string value);
		internal BaseApiSetVariableFn BaseApiSetVariable;

		internal delegate int BaseApiSetDebugVariableFn(IntPtr handle, string name, string value);
		internal BaseApiSetDebugVariableFn BaseApiSetDebugVariable;

		internal delegate int BaseApiGetIntVariableFn(IntPtr handle, string name, out int value);
		internal BaseApiGetIntVariableFn BaseApiGetIntVariable;

		internal delegate int BaseApiGetBoolVariableFn(IntPtr handle, string name, out int value);
		internal BaseApiGetBoolVariableFn BaseApiGetBoolVariable;

		internal delegate int BaseApiGetDoubleVariableFn(IntPtr handle, string name, out double value);
		internal BaseApiGetDoubleVariableFn BaseApiGetDoubleVariable;

		internal delegate string BaseApiGetStringVariableFn(IntPtr handle, string name);
		internal BaseApiGetStringVariableFn BaseApiGetStringVariable;

		internal delegate int BaseApiGetVariableAsStringFn(IntPtr handle, string name, out string val);
		internal BaseApiGetVariableAsStringFn BaseApiGetVariableAsString;

		internal delegate void BaseAPISetPageSegModeFn(IntPtr handle, PageSegMode mode);
		internal BaseAPISetPageSegModeFn BaseAPISetPageSegMode;

		internal delegate PageSegMode BaseAPIGetPageSegModeFn(IntPtr handle);
		internal BaseAPIGetPageSegModeFn BaseAPIGetPageSegMode;

		internal delegate void BaseApiSetImageFn(IntPtr handle, IntPtr pixHandle);
		internal BaseApiSetImageFn BaseApiSetImage;

		internal delegate void BaseApiSetRectangleFn(IntPtr handle, int left, int top, int width, int height);
		internal BaseApiSetRectangleFn BaseApiSetRectangle;

		internal delegate int BaseApiRecognizeFn(IntPtr handle, IntPtr monitor);
		internal BaseApiRecognizeFn BaseApiRecognize;

		internal delegate IntPtr BaseAPIAnalyseLayoutFn(IntPtr handle);
		internal BaseAPIAnalyseLayoutFn BaseAPIAnalyseLayout;

		internal delegate IntPtr BaseApiGetIteratorFn(IntPtr handle);
		internal BaseApiGetIteratorFn BaseApiGetIterator;

		internal delegate IntPtr BaseAPIGetUTF8TextInternalFn(IntPtr handle);
		internal BaseAPIGetUTF8TextInternalFn BaseAPIGetUTF8TextInternal;

		internal delegate IntPtr BaseAPIGetHOCRTextInternalFn(IntPtr handle, int pageNum);
		internal BaseAPIGetHOCRTextInternalFn BaseAPIGetHOCRTextInternal;

		internal delegate int BaseAPIMeanTextConfFn(IntPtr handle);
		internal BaseAPIMeanTextConfFn BaseAPIMeanTextConf;

		internal delegate void BaseAPIClearFn(IntPtr handle);
		internal BaseAPIClearFn BaseAPIClear;

		internal delegate void ResultIteratorDeleteFn(IntPtr handle);
		internal ResultIteratorDeleteFn ResultIteratorDelete;

		internal delegate IntPtr ResultIteratorCopyFn(IntPtr handle);
		internal ResultIteratorCopyFn ResultIteratorCopy;

		internal delegate IntPtr ResultIteratorGetPageIteratorFn(IntPtr handle);
		internal ResultIteratorGetPageIteratorFn ResultIteratorGetPageIterator;

		internal delegate IntPtr ResultIteratorGetUTF8TextInternalFn(IntPtr handle, PageIteratorLevel level);
		internal ResultIteratorGetUTF8TextInternalFn ResultIteratorGetUTF8TextInternal;

		internal delegate float ResultIteratorGetConfidenceFn(IntPtr handle, PageIteratorLevel level);
		internal ResultIteratorGetConfidenceFn ResultIteratorGetConfidence;

		internal delegate void PageIteratorDeleteFn(IntPtr handle);
		internal PageIteratorDeleteFn PageIteratorDelete;

		internal delegate IntPtr PageIteratorCopyFn(IntPtr handle);
		internal PageIteratorCopyFn PageIteratorCopy;

		internal delegate void PageIteratorBeginFn(IntPtr handle);
		internal PageIteratorBeginFn PageIteratorBegin;

		internal delegate int PageIteratorNextFn(IntPtr handle, PageIteratorLevel level);
		internal PageIteratorNextFn PageIteratorNext;

		internal delegate int PageIteratorIsAtBeginningOfFn(IntPtr handle, PageIteratorLevel level);
		internal PageIteratorIsAtBeginningOfFn PageIteratorIsAtBeginningOf;

		internal delegate int PageIteratorIsAtFinalElementFn(IntPtr handle, PageIteratorLevel level, PageIteratorLevel element);
		internal PageIteratorIsAtFinalElementFn PageIteratorIsAtFinalElement;

		internal delegate int PageIteratorBoundingBoxFn(IntPtr handle, PageIteratorLevel level, out int left, out int top, out int right, out int bottom);
		internal PageIteratorBoundingBoxFn PageIteratorBoundingBox;

		internal delegate PolyBlockType PageIteratorBlockTypeFn(IntPtr handle);
		internal PageIteratorBlockTypeFn PageIteratorBlockType;

		internal delegate IntPtr PageIteratorGetBinaryImageFn(IntPtr handle, PageIteratorLevel level);
		internal PageIteratorGetBinaryImageFn PageIteratorGetBinaryImage;

		internal delegate IntPtr PageIteratorGetImageFn(IntPtr handle, PageIteratorLevel level, int padding, out int left, out int top);
		internal PageIteratorGetImageFn PageIteratorGetImage;

		internal delegate int PageIteratorBaselineFn(IntPtr handle, PageIteratorLevel level, out int x1, out int y1, out int x2, out int y2);
		internal PageIteratorBaselineFn PageIteratorBaseline;

		internal delegate void PageIteratorOrientationFn(IntPtr handle, out Orientation orientation, out WritingDirection writing_direction, out TextLineOrder textLineOrder, out float deskew_angle);
		internal PageIteratorOrientationFn PageIteratorOrientation;

	}
}
