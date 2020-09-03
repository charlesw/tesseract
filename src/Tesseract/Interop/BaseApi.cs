using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Tesseract.Internal;

namespace Tesseract.Interop
{
    /// <summary>
    /// The exported tesseract api signatures.
    /// </summary>
    /// <remarks>
    /// Please note this is only public for technical reasons (you can't proxy a internal interface).
    /// It should be considered an internal interface and is NOT part of the public api and may have
    /// breaking changes between releases.
    /// </remarks>
    internal class TessApiSignatures
    {
        static TessApiSignatures()
        {
            DllLoader.ForceLoadLibrary(Constants.TesseractDllName);
        }

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetComponentImages")]
        internal static extern IntPtr BaseAPIGetComponentImages(HandleRef handle, PageIteratorLevel level, int text_only, IntPtr pixa, IntPtr blockids);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIAnalyseLayout")]
        internal static extern IntPtr BaseAPIAnalyseLayout(HandleRef handle);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIClear")]
        internal static extern void BaseAPIClear(HandleRef handle);

        /// <summary>
        /// Creates a new BaseAPI instance
        /// </summary>
        /// <returns></returns>
        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPICreate")]
        internal static extern IntPtr BaseApiCreate();

        // Base API
        /// <summary>
        /// Deletes a base api instance.
        /// </summary>
        /// <returns></returns>
        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIDelete")]
        internal static extern void BaseApiDelete(HandleRef ptr);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIDetectOrientationScript", CharSet = CharSet.Ansi)]
        internal static extern bool TessBaseAPIDetectOrientationScriptInternal(HandleRef handle, out int orient_deg, out float orient_conf, out IntPtr script_name, out float script_conf); // The script name is internal memory, canut marshall or free will blow up Tesseract
        internal static bool TessBaseAPIDetectOrientationScript(HandleRef handle, out int orient_deg, out float orient_conf, out string script_name, out float script_conf)
        {
            IntPtr script_name_ptr;
            bool rv = TessBaseAPIDetectOrientationScriptInternal(handle, out orient_deg, out orient_conf, out script_name_ptr, out script_conf);
            script_name = MarshalHelper.PtrToString(script_name_ptr);
            return rv;
        }

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetBoolVariable")]
        internal static extern bool BaseApiGetBoolVariable(HandleRef handle, string name, out bool value);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetDoubleVariable")]
        internal static extern bool BaseApiGetDoubleVariable(HandleRef handle, string name, out double value);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetHOCRText", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.LPUTF8Str)]
        internal static extern string BaseApiGetHOCRText(HandleRef handle, int pageNum); // Can auto free, the result is copied into a new buffer

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetAltoText", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.LPUTF8Str)]
        internal static extern string BaseApiGetAltoText(HandleRef handle, int pageNum); // Can auto free, the result is copied into a new buffer

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetTsvText", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.LPUTF8Str)]
        internal static extern string BaseApiGetTsvText(HandleRef handle, int pageNum); // Can auto free, the result is copied into a new buffer

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetBoxText", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.LPUTF8Str)]
        internal static extern string BaseApiGetBoxText(HandleRef handle, int pageNum); // Can auto free, the result is copied into a new buffer

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetLSTMBoxText", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.LPUTF8Str)]
        internal static extern string BaseApiGetLSTMBoxText(HandleRef handle, int pageNum); // Can auto free, the result is copied into a new buffer

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetWordStrBoxText", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.LPUTF8Str)]
        internal static extern string BaseApiGetWordStrBoxText(HandleRef handle, int pageNum); // Can auto free, the result is copied into a new buffer

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetUNLVText", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.LPUTF8Str)]
        internal static extern string BaseApiGetUNLVText(HandleRef handle); // Can auto free, the result is copied into a new buffer

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetIntVariable")]
        internal static extern bool BaseApiGetIntVariable(HandleRef handle, [MarshalAs(UnmanagedType.LPUTF8Str)] string name, out int value);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetIterator")]
        internal static extern IntPtr BaseApiGetIterator(HandleRef handle);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetPageSegMode")]
        internal static extern PageSegMode BaseAPIGetPageSegMode(HandleRef handle);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetStringVariable", CharSet = CharSet.Ansi)]
        internal static extern IntPtr BaseApiGetStringVariableInternal(HandleRef handle, [MarshalAs(UnmanagedType.LPUTF8Str)] string name);
        internal static string BaseApiGetStringVariable(HandleRef handle, string name)
        {
            return MarshalHelper.PtrToString(BaseApiGetStringVariableInternal(handle, name)); // return string cannot be deleted, it comes from a c_str call.
        }

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetThresholdedImage")]
        internal static extern IntPtr BaseAPIGetThresholdedImage(HandleRef handle);

        // The following were causing issues on Linux/MacOsX when used in .net core
        //[DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIProcessPages")]
        //int BaseAPIProcessPages(HandleRef handle, string filename, string retry_config, int timeout_millisec, HandleRef renderer);

        //[DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIProcessPage")]
        //int BaseAPIProcessPage(HandleRef handle, Pix pix, int page_index, string filename, string retry_config, int timeout_millisec, HandleRef renderer);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPISetInputName")]
        internal static extern void BaseAPISetInputName(HandleRef handle, string name);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetDatapath")]
        internal static extern string BaseAPIGetDatapath(HandleRef handle);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPISetOutputName")]
        internal static extern void BaseAPISetOutputName(HandleRef handle, string name);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetUTF8Text", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.LPUTF8Str)]
        internal static extern string BaseAPIGetUTF8Text(HandleRef handle);

        // Issue on this becuase of the UnmanagedType.LPArray + UnmanagedType.LPUTF8Str combination.
        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIInit4")]
        internal static extern int BaseApiInit(HandleRef handle,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string datapath, [MarshalAs(UnmanagedType.LPUTF8Str)] string language, int mode,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] string[] configs, int configs_size,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] string[] vars_vec, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] string[] vars_values,
            uint vars_vec_size, bool set_only_non_debug_params);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIMeanTextConf")]
        internal static extern int BaseAPIMeanTextConf(HandleRef handle);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIRecognize")]
        internal static extern int BaseApiRecognize(HandleRef handle, HandleRef monitor);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPISetDebugVariable", CharSet = CharSet.Ansi)]
        internal static extern bool BaseApiSetDebugVariable(HandleRef handle, [MarshalAs(UnmanagedType.LPUTF8Str)]string name, [MarshalAs(UnmanagedType.LPUTF8Str)]string value);

        // image analysis
        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPISetImage2")]
        internal static extern void BaseApiSetImage(HandleRef handle, HandleRef pixHandle);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPISetInputName")]
        internal static extern void BaseApiSetInputName(HandleRef handle, string value);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPISetPageSegMode")]
        internal static extern void BaseAPISetPageSegMode(HandleRef handle, PageSegMode mode);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPISetRectangle")]
        internal static extern void BaseApiSetRectangle(HandleRef handle, int left, int top, int width, int height);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPISetVariable", CharSet = CharSet.Ansi)]
        internal static extern bool BaseApiSetVariable(HandleRef handle, [MarshalAs(UnmanagedType.LPUTF8Str)] string name, [MarshalAs(UnmanagedType.LPUTF8Str)] string valPtr);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessDeleteIntArray")]
        internal static extern void DeleteIntArray(IntPtr arr);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessDeleteText")]
        internal static extern void DeleteText(IntPtr textPtr);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessDeleteTextArray")]
        internal static extern void DeleteTextArray(IntPtr arr);

        // Helper functions
        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessVersion", CharSet = CharSet.Ansi)]
        internal static extern IntPtr GetVersionInternal();
        internal static string GetVersion()
        {
            return MarshalHelper.PtrToString(GetVersionInternal()); // Do not free as it is a constant on the stack
        }

        // result iterator

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorBaseline")]
        internal static extern bool PageIteratorBaseline(HandleRef handle, PageIteratorLevel level, out int x1, out int y1, out int x2, out int y2);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorBegin")]
        internal static extern void PageIteratorBegin(HandleRef handle);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorBlockType")]
        internal static extern PolyBlockType PageIteratorBlockType(HandleRef handle);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorBoundingBox")]
        internal static extern bool PageIteratorBoundingBox(HandleRef handle, PageIteratorLevel level, out int left, out int top, out int right, out int bottom);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorCopy")]
        internal static extern HandleRef PageIteratorCopy(HandleRef handle);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorDelete")]
        internal static extern void PageIteratorDelete(HandleRef handle);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorGetBinaryImage")]
        internal static extern IntPtr PageIteratorGetBinaryImage(HandleRef handle, PageIteratorLevel level);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorGetImage")]
        internal static extern IntPtr PageIteratorGetImage(HandleRef handle, PageIteratorLevel level, int padding, HandleRef originalImage, out int left, out int top);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorIsAtBeginningOf")]
        internal static extern bool PageIteratorIsAtBeginningOf(HandleRef handle, PageIteratorLevel level);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorIsAtFinalElement")]
        internal static extern bool PageIteratorIsAtFinalElement(HandleRef handle, PageIteratorLevel level, PageIteratorLevel element);

        // page iterator
        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorNext")]
        internal static extern bool PageIteratorNext(HandleRef handle, PageIteratorLevel level);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorOrientation")]
        internal static extern void PageIteratorOrientation(HandleRef handle, out Orientation orientation, out WritingDirection writing_direction, out TextLineOrder textLineOrder, out float deskew_angle);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorCopy")]
        internal static extern IntPtr ResultIteratorCopy(HandleRef handle);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorDelete")]
        internal static extern void ResultIteratorDelete(HandleRef handle);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorConfidence")]
        internal static extern float ResultIteratorGetConfidence(HandleRef handle, PageIteratorLevel level);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorWordFontAttributes")]
        [return: MarshalAs(UnmanagedType.LPUTF8Str)]
        internal static extern string ResultIteratorWordFontAttributes(HandleRef handle, out bool isBold, out bool isItalic, out bool isUnderlined, out bool isMonospace, out bool isSerif, out bool isSmallCaps, out int pointSize, out int fontId);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorWordIsFromDictionary")]
        internal static extern bool ResultIteratorWordIsFromDictionary(HandleRef handle);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorWordIsNumeric")]
        internal static extern bool ResultIteratorWordIsNumeric(HandleRef handle);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorWordRecognitionLanguage", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.LPUTF8Str)]
        internal static extern string ResultIteratorWordRecognitionLanguage(HandleRef handle);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorSymbolIsSuperscript")]
        internal static extern bool ResultIteratorSymbolIsSuperscript(HandleRef handle);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorSymbolIsSubscript")]
        internal static extern bool ResultIteratorSymbolIsSubscript(HandleRef handle);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorSymbolIsDropcap")]
        internal static extern bool ResultIteratorSymbolIsDropcap(HandleRef handle);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorGetPageIterator")]
        internal static extern IntPtr ResultIteratorGetPageIterator(HandleRef handle);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorGetUTF8Text", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.LPUTF8Str)]
        internal static extern string ResultIteratorGetUTF8Text(HandleRef handle, PageIteratorLevel level);

        #region Choice Iterator

        /// <summary>
        /// Native API call to TessResultIteratorGetChoiceIterator
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorGetChoiceIterator")]
        internal static extern IntPtr ResultIteratorGetChoiceIterator(HandleRef handle);

        /// <summary>
        /// Native API call to TessChoiceIteratorDelete
        /// </summary>
        /// <param name="handle"></param>
        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessChoiceIteratorDelete")]
        internal static extern void ChoiceIteratorDelete(HandleRef handle);

        /// <summary>
        /// Native API call to TessChoiceIteratorNext
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessChoiceIteratorNext")]
        internal static extern int ChoiceIteratorNext(HandleRef handle);

        /// <summary>
        /// Native API call to TessChoiceIteratorGetUTF8Text
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessChoiceIteratorGetUTF8Text", CharSet = CharSet.Ansi)]
        internal static extern IntPtr ChoiceIteratorGetUTF8TextInternal(HandleRef handle); // Returns internal memory
        internal static string ChoiceIteratorGetUTF8Text(HandleRef handle)
        {
            return MarshalHelper.PtrToString(ChoiceIteratorGetUTF8TextInternal(handle));
        }

        /// <summary>
        /// Native API call to TessChoiceIteratorConfidence
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessChoiceIteratorConfidence")]
        internal static extern float ChoiceIteratorGetConfidence(HandleRef handle);

        #endregion Choice Iterator

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIPrintVariablesToFile")]
        internal static extern int BaseApiPrintVariablesToFile(HandleRef handle, string filename);

        #region Renderer API

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessTextRendererCreate")]
        internal static extern IntPtr TextRendererCreate(string outputbase);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessAltoRendererCreate")]
        internal static extern IntPtr AltoRendererCreate(string outputbase);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessTsvRendererCreate")]
        internal static extern IntPtr TsvRendererCreate(string outputbase);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessHOcrRendererCreate")]
        internal static extern IntPtr HOcrRendererCreate(string outputbase);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessHOcrRendererCreate2")]
        internal static extern IntPtr HOcrRendererCreate2(string outputbase, int font_info);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPDFRendererCreate")]
        internal static extern IntPtr PDFRendererCreate(string outputbase, IntPtr datadir, int textonly);
        
        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessUnlvRendererCreate")]
        internal static extern IntPtr UnlvRendererCreate(string outputbase);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBoxTextRendererCreate")]
        internal static extern IntPtr BoxTextRendererCreate(string outputbase);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessLSTMBoxRendererCreate")]
        internal static extern IntPtr LSTMBoxRendererCreate(string outputbase);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessWordStrBoxRendererCreate")]
        internal static extern IntPtr WordStrBoxRendererCreate(string outputbase);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessDeleteResultRenderer")]
        internal static extern void DeleteResultRenderer(HandleRef renderer);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultRendererInsert")]
        internal static extern void ResultRendererInsert(HandleRef renderer, HandleRef next);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultRendererNext")]
        internal static extern IntPtr ResultRendererNext(HandleRef renderer);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultRendererBeginDocument")]
        internal static extern int ResultRendererBeginDocument(HandleRef renderer, IntPtr titlePtr);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultRendererAddImage")]
        internal static extern int ResultRendererAddImage(HandleRef renderer, HandleRef api);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultRendererEndDocument")]
        internal static extern int ResultRendererEndDocument(HandleRef renderer);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultRendererExtention")]
        internal static extern IntPtr ResultRendererExtention(HandleRef renderer);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultRendererTitle")]
        internal static extern IntPtr ResultRendererTitle(HandleRef renderer);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultRendererImageNum")]
        internal static extern int ResultRendererImageNum(HandleRef renderer);

        #endregion Renderer API
    }

    internal static class TessApi
    {
        public static int BaseApiInit(HandleRef handle, string datapath, string language, int mode, IEnumerable<string> configFiles, IDictionary<string, object> initialValues, bool setOnlyNonDebugParams)
        {
            Guard.Require("handle", handle.Handle != IntPtr.Zero, "Handle for BaseApi, created through BaseApiCreate is required.");
            Guard.RequireNotNullOrEmpty("language", language);
            Guard.RequireNotNull("configFiles", configFiles);
            Guard.RequireNotNull("initialValues", initialValues);

            string[] configFilesArray = new List<string>(configFiles).ToArray();

            string[] varNames = new string[initialValues.Count];
            string[] varValues = new string[initialValues.Count];
            int i = 0;
            foreach (var pair in initialValues) {
                Guard.Require("initialValues", !String.IsNullOrEmpty(pair.Key), "Variable must have a name.");

                Guard.Require("initialValues", pair.Value != null, "Variable '{0}': The type '{1}' is not supported.", pair.Key, pair.Value.GetType());
                varNames[i] = pair.Key;
                string varValue;
                if (TessConvert.TryToString(pair.Value, out varValue)) {
                    varValues[i] = varValue;
                } else {
                    throw new ArgumentException(
                        string.Format("Variable '{0}': The type '{1}' is not supported.", pair.Key, pair.Value.GetType()),
                        "initialValues"
                    );
                }
                i++;
            }

            return TessApiSignatures.BaseApiInit(handle, datapath, language, mode,
                checkNonAscii(configFilesArray), configFilesArray.Length,
                checkNonAscii(varNames), checkNonAscii(varValues), (uint)varNames.Length, setOnlyNonDebugParams);
        }

        /// <summary>
        /// Check for any non-ascii characters in an array of strings. This is because .Net won't allow an array to be marshalled as a subtype of UTF8string.
        /// Since the tesseract functions can't handle w_char types, we'll need to check that all strings are asicc (which is in UTF8).
        /// </summary>
        /// <param name="arr"></param>
        private static string[] checkNonAscii(string[] arr)
        {
            Regex asciiRegex = new Regex(@"^\p{IsBasicLatin}*$");
            foreach (string s in arr)
            {
                if (0 == asciiRegex.Matches(s).Count) throw new ArgumentException("Cannot handle non-ascii arguments due to C# interop limitations");
            }
            return arr;
        }
    }
}
