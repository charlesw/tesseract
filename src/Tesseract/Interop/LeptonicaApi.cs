using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Tesseract.Interop
{
    public unsafe static class LeptonicaApi
    {
        static LeptonicaApi()
        {
            // This may have already been loaded by tesseract but that's fine (EmbeddedDllLoader won't try and load the dll again).
            WindowsLibraryLoader.Instance.LoadLibrary("liblept168.dll");
        }


        #region Pix

        [DllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixCreate")]
        public static unsafe extern IntPtr pixCreate(int width, int height, int depth);
        
        [DllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixDestroy")]
        public static extern void pixDestroy(ref IntPtr pix);

        [DllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixGetWidth")]
        public static extern int pixGetWidth(IntPtr pix);

        [DllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixGetHeight")]
        public static extern int pixGetHeight(IntPtr pix);


        [DllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixGetDepth")]
        public static extern int pixGetDepth(IntPtr pix);
                
        [DllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixGetXRes")]
        public static extern int pixGetXRes(IntPtr pix);

        [DllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixGetYRes")]
        public static extern int pixGetYRes(IntPtr pix);

        [DllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixGetResolution")]
        public static extern int pixGetResolution(IntPtr pix, out int xres, out int yres);

        [DllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixGetWpl")]
        public static extern int pixGetWpl(IntPtr pix);

        [DllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixSetXRes")]
        public static extern int pixSetXRes(IntPtr pix, int xres);

        [DllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixSetYRes")]
        public static extern int pixSetYRes(IntPtr pix, int yres);

        [DllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixSetResolution")]
        public static extern int pixSetResolution(IntPtr pix, int xres, int yres);


        [DllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixScaleResolution")]
        public static extern int pixScaleResolution(IntPtr pix, float xscale, float yscale);

        [DllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixGetData")]
        public static extern IntPtr pixGetData(IntPtr pix);


        [DllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixGetInputFormat")]
        public static extern ImageFormat pixGetInputFormat(IntPtr pix);

        [DllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixSetInputFormat")]
        public static extern int pixSetInputFormat(IntPtr pix, ImageFormat inputFormat);
        
        [DllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixEndianByteSwap")]
        public static extern int pixEndianByteSwap(IntPtr pix);

        [DllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixRead")]
        public static extern IntPtr pixRead(string filename);

        [DllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixWrite")]
        public static extern int pixWrite(string filename, IntPtr handle, ImageFormat format);
        
        [DllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixGetColormap")]
        public static extern IntPtr pixGetColormap(IntPtr pix);

        [DllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixSetColormap")]
        public static extern int pixSetColormap(IntPtr pix, IntPtr pixCmap);
        
        [DllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixDestroyColormap")]
        public static extern int pixDestroyColormap(IntPtr pix);

        // pixconv.h functions

        [DllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixConvertRGBToGray")]
        public static extern IntPtr pixConvertRGBToGray(IntPtr pix, float rwt, float gwt, float bwt);


        // image analysis and manipulation functions

        // skew

        [DllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixDeskewGeneral")]
        public static extern IntPtr pixDeskewGeneral(IntPtr pix, int redSweep, float sweepRange, float sweepDelta, int redSearch, int thresh, out float pAngle, out float pConf);
        
        // Binarization - src/binarize.c

        [DllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixOtsuAdaptiveThreshold")]
        public static extern int pixOtsuAdaptiveThreshold(IntPtr pix, int sx, float sy, float smoothx, int smoothy, float scorefract, out IntPtr ppixth, out IntPtr ppixd);

        #endregion

        #region Color map

        // Color map creation and deletion

        /// <summary>
        /// Creates a new colormap with the specified <paramref name="depth"/>.
        /// </summary>
        /// <param name="depth">The depth of the pix in bpp, can be 2, 4, or 8</param>
        /// <returns>The pointer to the color map, or null on error.</returns>
        [DllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapCreate")]
        public static extern IntPtr pixcmapCreate(int depth);

        /// <summary>
        /// Creates a new colormap of the specified <paramref name="depth"/> with random colors where the first color can optionally be set to black, and the last optionally set to white.
        /// </summary>
        /// <param name="depth">The depth of the pix in bpp, can be 2, 4, or 8</param>
        /// <param name="hasBlack">If set to 1 the first color will be black.</param>
        /// <param name="hasWhite">If set to 1 the last color will be white.</param>
        /// <returns>The pointer to the color map, or null on error.</returns>
        [DllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapCreateRandom")]
        public static extern IntPtr pixcmapCreateRandom(int depth, int hasBlack, int hasWhite);
        
        /// <summary>
        /// Creates a new colormap of the specified <paramref name="depth"/> with equally spaced gray color values. 
        /// </summary>
        /// <param name="depth">The depth of the pix in bpp, can be 2, 4, or 8</param>
        /// <param name="levels">The number of levels (must be between 2 and 2^<paramref name="depth"/></param>
        /// <returns>The pointer to the colormap, or null on error.</returns>
        [DllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapCreateLinear")]
        public static extern IntPtr pixcmapCreateLinear(int depth, int levels);

        /// <summary>
        /// Performs a deep copy of the color map.
        /// </summary>
        /// <param name="cmap">The pointer to the colormap instance.</param>
        /// <returns>The pointer to the colormap, or null on error.</returns>
        [DllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapCopy")]
        public static extern IntPtr pixcmapCopy(IntPtr cmaps);

        /// <summary>
        /// Destorys and cleans up any memory used by the color map.
        /// </summary>
        /// <param name="cmap">The pointer to the colormap instance, set to null on success.</param>
        [DllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapDestroy")]
        public static extern void pixcmapDestroy(ref IntPtr cmap);
        
        // colormap metadata (depth, count, etc)

        /// <summary>
        /// Gets the number of color entries in the color map.
        /// </summary>
        /// <param name="cmap">The pointer to the colormap instance.</param>
        /// <returns>Returns the number of color entries in the color map, or 0 on error.</returns>
        [DllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapGetCount")]
        public static extern int pixcmapGetCount(IntPtr cmap);

        /// <summary>
        /// Gets the number of free color entries in the color map.
        /// </summary>
        /// <param name="cmap">The pointer to the colormap instance.</param>
        /// <returns>Returns the number of free color entries in the color map, or 0 on error.</returns>
        [DllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapGetFreeCount")]
        public static extern int pixcmapGetFreeCount(IntPtr cmap);
        

        /// <returns>Returns color maps depth, or 0 on error.</returns>
        [DllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapGetDepth")]
        public static extern int pixcmapGetDepth(IntPtr cmap);

        /// <summary>
        /// Gets the minimum pix depth required to support the color map.
        /// </summary>
        /// <param name="cmap">The pointer to the colormap instance.</param>
        /// <param name="minDepth">Returns the minimum depth to support the colormap</param>
        /// <returns>Returns 0 if OK, 1 on error.</returns>
        [DllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapGetMinDepth")]
        public static extern int pixcmapGetMinDepth(IntPtr cmap, out int minDepth);
        
        // colormap - color addition\clearing

        /// <summary>
        /// Removes all colors from the color map by setting the count to zero.
        /// </summary>
        /// <param name="cmap">The pointer to the colormap instance.</param>
        /// <returns>Returns 0 if OK, 1 on error.</returns>
        [DllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapClear")]
        public static extern int pixcmapClear(IntPtr cmap);

        /// <summary>
        /// Adds the color to the pix color map if their is room.
        /// </summary>
        /// <returns>Returns 0 if OK, 1 on error.</returns>
        [DllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapAddColor")]
        public static extern int pixcmapAddColor(IntPtr cmap, int redValue, int greenValue, int blueValue);

        /// <summary>
        /// Adds the specified color if it doesn't already exist, returning the colors index in the data array.
        /// </summary>
        /// <param name="cmap">The pointer to the colormap instance.</param>
        /// <param name="redValue">The red value</param>
        /// <param name="greenValue">The green value</param>
        /// <param name="blueValue">The blue value</param>
        /// <param name="colorIndex">The index of the new color if it was added, or the existing color if it already existed.</param>
        /// <returns>Returns 0 for success, 1 for error, 2 for not enough space.</returns>
        [DllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapAddNewColor")]
        public static extern int pixcmapAddNewColor(IntPtr cmap, int redValue, int greenValue, int blueValue, out int colorIndex);

        /// <summary>
        /// Adds the specified color if it doesn't already exist, returning the color's index in the data array.
        /// </summary>
        /// <remarks>
        /// If the color doesn't exist and there is not enough room to add a new color return the nearest color.
        /// </remarks>
        /// <param name="cmap">The pointer to the colormap instance.</param>
        /// <param name="redValue">The red value</param>
        /// <param name="greenValue">The green value</param>
        /// <param name="blueValue">The blue value</param>
        /// <param name="colorIndex">The index of the new color if it was added, or the existing color if it already existed.</param>
        /// <returns>Returns 0 for success, 1 for error, 2 for not enough space.</returns>
        [DllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapAddNearestColor")]
        public static extern int pixcmapAddNearestColor(IntPtr cmap, int redValue, int greenValue, int blueValue, out int colorIndex);

        /// <summary>
        /// Checks if the color already exists or if their is enough room to add it.
        /// </summary>
        /// <param name="cmap">The pointer to the colormap instance.</param>
        /// <param name="redValue">The red value</param>
        /// <param name="greenValue">The green value</param>
        /// <param name="blueValue">The blue value</param>
        /// <param name="usable">Returns 1 if usable; 0 if not.</param>
        /// <returns>Returns 0 if OK, 1 on error.</returns>
        [DllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapUsableColor")]
        public static extern int pixcmapUsableColor(IntPtr cmap, int redValue, int greenValue, int blueValue, out int usable);

        /// <summary>
        /// Adds a color (black\white) if not already there returning it's index through <paramref name="index"/>.
        /// </summary>
        /// <param name="cmap">The pointer to the colormap instance.</param>
        /// <param name="color">The color to add (0 for black; 1 for white)</param>
        /// <param name="index">The index of the color.</param>
        /// <returns>Returns 0 if OK; 1 on error.</returns>
        [DllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapAddBlackOrWhite")]
        public static extern int pixcmapAddBlackOrWhite(IntPtr cmap, int color, out int index);

        /// <summary>
        /// Sets the darkest color in the colormap to black, if <paramref name="setBlack"/> is 1. 
        /// Sets the lightest color in the colormap to white if <paramref name="setWhite"/> is 1. 
        /// </summary>
        /// <param name="cmap">The pointer to the colormap instance.</param>
        /// <param name="setBlack">0 for no operation; 1 to set darket color to black</param>
        /// <param name="setBlack">0 for no operation; 1 to set lightest color to white</param>
        /// <returns>Returns 0 if OK; 1 on error.</returns>
        [DllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapSetBlackAndWhite")]
        public static extern int pixcmapSetBlackAndWhite(IntPtr cmap, int setBlack, int setWhite);

        // color access - color entry access

        /// <summary>
        /// Gets the color at the specified index.
        /// </summary>
        /// <param name="cmap">The pointer to the colormap instance.</param>
        /// <param name="index">The index of the color entry.</param>
        /// <param name="redValue">The color entry's red value.</param>
        /// <param name="blueValue">The color entry's blue value.</param>
        /// <param name="greenValue">The color entry's green value.</param>
        /// <returns>Returns 0 if OK; 1 if not accessable (caller should check).</returns>
        [DllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapGetColor")]
        public static extern int pixcmapGetColor(IntPtr cmap, int index, out int redValue, out int blueValue, out int greenValue);

        /// <summary>
        /// Gets the color at the specified index.
        /// </summary>
        /// <remarks>
        /// The alpha channel will always be zero as it is not used in Leptonica color maps.
        /// </remarks>
        /// <param name="cmap">The pointer to the colormap instance.</param>
        /// <param name="index">The index of the color entry.</param>
        /// <param name="color">The color entry as 32 bit value</param>
        /// <returns>Returns 0 if OK; 1 if not accessable (caller should check).</returns>
        [DllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapGetColor32")]
        public static extern int pixcmapGetColor32(IntPtr cmap, int index, out int color);

        /// <summary>
        /// Sets a previously allocated color entry.
        /// </summary>
        /// <param name="cmap">The pointer to the colormap instance.</param>
        /// <param name="index">The index of the colormap entry</param>
        /// <param name="redValue"></param>
        /// <param name="blueValue"></param>
        /// <param name="greenValue"></param>
        /// <returns>Returns 0 if OK; 1 if not accessable (caller should check).</returns>
        [DllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapResetColor")]
        public static extern int pixcmapResetColor(IntPtr cmap, int index, int redValue, int blueValue, int greenValue);

        /// <summary>
        /// Gets the index of the color entry with the specified color, return 0 if found; 1 if not.
        /// </summary>
        [DllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapGetIndex")]
        public static extern int pixcmapGetIndex(IntPtr cmap, int redValue, int blueValue, int greenValue, out int index);


        /// <summary>
        /// Returns 0 if the color exists in the color map; otherwise 1.
        /// </summary>
        /// <returns>Returns 0 if OK; 1 on error.</returns>
        [DllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapHasColor")]
        public static extern int pixcmapHasColor(IntPtr cmap, int color);


        /// <summary>
        /// Returns the number of unique grey colors including black and white.
        /// </summary>
        /// <returns>Returns 0 if OK; 1 on error.</returns>
        [DllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapCountGrayColors")]
        public static extern int pixcmapCountGrayColors(IntPtr cmap, out int ngray);
        
        /// <summary>
        /// Finds the index of the color entry with the rank intensity.
        /// </summary>
        /// <returns>Returns 0 if OK; 1 on error.</returns>
        [DllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapCountGrayColors")]
        public static extern int pixcmapGetRankIntensity(IntPtr cmap, float rankVal, out int index);


        /// <summary>
        /// Finds the index of the color entry closest to the specified color.
        /// </summary>
        /// <returns>Returns 0 if OK; 1 on error.</returns>
        [DllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapGetNearestIndex")]
        public static extern int pixcmapGetNearestIndex(IntPtr cmap, int rVal, int bVal, int gVal, out int index);

        /// <summary>
        /// Finds the index of the color entry closest to the specified color.
        /// </summary>
        /// <remarks>
        /// Should only be used on gray colormaps.
        /// </remarks>
        /// <returns>Returns 0 if OK; 1 on error.</returns>
        [DllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapGetNearestGrayIndex")]
        public static extern int pixcmapGetNearestGrayIndex(IntPtr cmap, int val, out int index);

        [DllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapGetComponentRange")]
        public static extern int pixcmapGetComponentRange(IntPtr cmap, int component, out int minVal, out int maxVal);

        [DllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapGetExtremeValue")]
        public static extern int pixcmapGetExtremeValue(IntPtr cmap, int type, out int rVal, out int gVal, out int bVal);

        // color map conversion

        [DllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapGrayToColor")]
        public static extern IntPtr pixcmapGrayToColor(int color);


        [DllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapColorToGray")]
        public static extern IntPtr pixcmapColorToGray(IntPtr cmaps, float redWeight, float greenWeight, float blueWeight);

        // colormap serialization

        [DllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapColorToGray")]
        public static extern int pixcmapToArrays(IntPtr cmap,  out IntPtr redMap, out IntPtr blueMap, out IntPtr greenMap);


        [DllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapToRGBTable")]
        public static extern int pixcmapToRGBTable(IntPtr cmap, out IntPtr colorTable, out int colorCount);


        [DllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapSerializeToMemory")]
        public static extern int pixcmapSerializeToMemory(IntPtr cmap, out int components, out int colorCount, out IntPtr colorData, out int colorDataLength);


        [DllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapSerializeToMemory")]
        public static extern IntPtr pixcmapSerializeToMemory(IntPtr colorData, int colorCount, int colorDataLength);
        
        // colormap transformations 

        [DllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapGammaTRC")]
        public static extern int pixcmapGammaTRC(IntPtr cmap, float gamma, int minVal, int maxVal);


        [DllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapContrastTRC")]
        public static extern int pixcmapContrastTRC(IntPtr cmap, float factor);
                        
        [DllImport(Constants.LeptonicaDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapShiftIntensity")]
        public static extern int pixcmapShiftIntensity(IntPtr cmap, float fraction);


        #endregion
    }
}
