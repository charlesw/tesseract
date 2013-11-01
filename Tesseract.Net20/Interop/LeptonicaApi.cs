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
		    Initialize(TessApi.Is64bit);
	    }

	    private static LeptonicaImplementation _implementation;
	    internal static void Initialize(bool is64Bit)
	    {
		    if (_implementation != null)
		    {
			    return;
		    }

			if (is64Bit)
			{
				_implementation = LeptonicaImports64.Initialize();
			}
			else
			{
				_implementation = LeptonicaImports32.Initialize();
			}		    
	    }
        #region Pix

        public static IntPtr pixCreate(int width, int height, int depth)
		{
			return _implementation.pixCreate(width, height, depth);
		}
        
        public static void pixDestroy(ref IntPtr pix)
		{
			_implementation.pixDestroy(ref pix);
		}

        public static int pixGetWidth(IntPtr pix)
		{
			return _implementation.pixGetWidth(pix);
		}

        public static int pixGetHeight(IntPtr pix)
		{
			return _implementation.pixGetHeight(pix);
		}


        public static int pixGetDepth(IntPtr pix)
		{
			return _implementation.pixGetDepth(pix);
		}
                
        public static int pixGetXRes(IntPtr pix)
		{
			return _implementation.pixGetXRes(pix);
		}

        public static int pixGetYRes(IntPtr pix)
		{
			return _implementation.pixGetYRes(pix);
		}

        public static int pixGetResolution(IntPtr pix, out int xres, out int yres)
		{
			return _implementation.pixGetResolution(pix, out xres, out yres);
		}

        public static int pixGetWpl(IntPtr pix)
		{
			return _implementation.pixGetWpl(pix);
		}

        public static int pixSetXRes(IntPtr pix, int xres)
		{
			return _implementation.pixSetXRes(pix, xres);
		}

        public static int pixSetYRes(IntPtr pix, int yres)
		{
			return _implementation.pixSetYRes(pix, yres);
		}

        public static int pixSetResolution(IntPtr pix, int xres, int yres)
		{
			return _implementation.pixSetResolution(pix, xres, yres);
		}


        public static int pixScaleResolution(IntPtr pix, float xscale, float yscale)
		{
			return _implementation.pixScaleResolution(pix, xscale, yscale);
		}

        public static IntPtr pixGetData(IntPtr pix)
		{
			return _implementation.pixGetData(pix);
		}


        public static ImageFormat pixGetInputFormat(IntPtr pix)
		{
			return _implementation.pixGetInputFormat(pix);
		}

        public static int pixSetInputFormat(IntPtr pix, ImageFormat inputFormat)
		{
			return _implementation.pixSetInputFormat(pix, inputFormat);
		}
        
        public static int pixEndianByteSwap(IntPtr pix)
		{
			return _implementation.pixEndianByteSwap(pix);
		}

        public static IntPtr pixRead(string filename)
		{
			return _implementation.pixRead(filename);
		}

        public static int pixWrite(string filename, IntPtr handle, ImageFormat format)
		{
			return _implementation.pixWrite(filename, handle, format);
		}
        
        public static IntPtr pixGetColormap(IntPtr pix)
		{
			return _implementation.pixGetColormap(pix);
		}

        public static int pixSetColormap(IntPtr pix, IntPtr pixCmap)
		{
			return _implementation.pixSetColormap(pix, pixCmap);
		}
        
        public static int pixDestroyColormap(IntPtr pix)
		{
			return _implementation.pixDestroyColormap(pix);
		}

        // pixconv.h functions

        public static IntPtr pixConvertRGBToGray(IntPtr pix, float rwt, float gwt, float bwt)
		{
			return _implementation.pixConvertRGBToGray(pix, rwt, gwt, bwt);
		}


        // image analysis and manipulation functions

        // skew

        public static IntPtr pixDeskewGeneral(IntPtr pix, int redSweep, float sweepRange, float sweepDelta, int redSearch, int thresh, out float pAngle, out float pConf)
		{
			return _implementation.pixDeskewGeneral(pix, redSweep, sweepRange, sweepDelta, redSearch, thresh, out pAngle, out pConf);
		}
        
        // Binarization - src/binarize.c

        public static int pixOtsuAdaptiveThreshold(IntPtr pix, int sx, float sy, float smoothx, int smoothy, float scorefract, out IntPtr ppixth, out IntPtr ppixd)
		{
			return _implementation.pixOtsuAdaptiveThreshold(pix, sx, sy, smoothx, smoothy, scorefract, out ppixth, out ppixd);
		}

        #endregion

        #region Color map

        // Color map creation and deletion

        /// <summary>
        /// Creates a new colormap with the specified <paramref name="depth"/>.
        /// </summary>
        /// <param name="depth">The depth of the pix in bpp, can be 2, 4, or 8</param>
        /// <returns>The pointer to the color map, or null on error.</returns>
        public static IntPtr pixcmapCreate(int depth)
		{
			return _implementation.pixcmapCreate(depth);
		}

        /// <summary>
        /// Creates a new colormap of the specified <paramref name="depth"/> with random colors where the first color can optionally be set to black, and the last optionally set to white.
        /// </summary>
        /// <param name="depth">The depth of the pix in bpp, can be 2, 4, or 8</param>
        /// <param name="hasBlack">If set to 1 the first color will be black.</param>
        /// <param name="hasWhite">If set to 1 the last color will be white.</param>
        /// <returns>The pointer to the color map, or null on error.</returns>
        public static IntPtr pixcmapCreateRandom(int depth, int hasBlack, int hasWhite)
		{
			return _implementation.pixcmapCreateRandom(depth, hasBlack, hasWhite);
		}
        
        /// <summary>
        /// Creates a new colormap of the specified <paramref name="depth"/> with equally spaced gray color values. 
        /// </summary>
        /// <param name="depth">The depth of the pix in bpp, can be 2, 4, or 8</param>
        /// <param name="levels">The number of levels (must be between 2 and 2^<paramref name="depth"/></param>
        /// <returns>The pointer to the colormap, or null on error.</returns>
        public static IntPtr pixcmapCreateLinear(int depth, int levels)
		{
			return _implementation.pixcmapCreateLinear(depth, levels);
		}

        /// <summary>
        /// Performs a deep copy of the color map.
        /// </summary>
        /// <param name="cmap">The pointer to the colormap instance.</param>
        /// <returns>The pointer to the colormap, or null on error.</returns>
        public static IntPtr pixcmapCopy(IntPtr cmaps)
		{
			return _implementation.pixcmapCopy(cmaps);
		}

        /// <summary>
        /// Destorys and cleans up any memory used by the color map.
        /// </summary>
        /// <param name="cmap">The pointer to the colormap instance, set to null on success.</param>
        public static void pixcmapDestroy(ref IntPtr cmap)
		{
			_implementation.pixcmapDestroy(ref cmap);
		}
        
        // colormap metadata (depth, count, etc)

        /// <summary>
        /// Gets the number of color entries in the color map.
        /// </summary>
        /// <param name="cmap">The pointer to the colormap instance.</param>
        /// <returns>Returns the number of color entries in the color map, or 0 on error.</returns>
        public static int pixcmapGetCount(IntPtr cmap)
		{
			return _implementation.pixcmapGetCount(cmap);
		}

        /// <summary>
        /// Gets the number of free color entries in the color map.
        /// </summary>
        /// <param name="cmap">The pointer to the colormap instance.</param>
        /// <returns>Returns the number of free color entries in the color map, or 0 on error.</returns>
        public static int pixcmapGetFreeCount(IntPtr cmap)
		{
			return _implementation.pixcmapGetFreeCount(cmap);
		}
        

        /// <returns>Returns color maps depth, or 0 on error.</returns>
        public static int pixcmapGetDepth(IntPtr cmap)
		{
			return _implementation.pixcmapGetDepth(cmap);
		}

        /// <summary>
        /// Gets the minimum pix depth required to support the color map.
        /// </summary>
        /// <param name="cmap">The pointer to the colormap instance.</param>
        /// <param name="minDepth">Returns the minimum depth to support the colormap</param>
        /// <returns>Returns 0 if OK, 1 on error.</returns>
        public static int pixcmapGetMinDepth(IntPtr cmap, out int minDepth)
		{
			return _implementation.pixcmapGetMinDepth(cmap, out minDepth);
		}
        
        // colormap - color addition\clearing

        /// <summary>
        /// Removes all colors from the color map by setting the count to zero.
        /// </summary>
        /// <param name="cmap">The pointer to the colormap instance.</param>
        /// <returns>Returns 0 if OK, 1 on error.</returns>
        public static int pixcmapClear(IntPtr cmap)
		{
			return _implementation.pixcmapClear(cmap);
		}

        /// <summary>
        /// Adds the color to the pix color map if their is room.
        /// </summary>
        /// <returns>Returns 0 if OK, 1 on error.</returns>
        public static int pixcmapAddColor(IntPtr cmap, int redValue, int greenValue, int blueValue)
		{
			return _implementation.pixcmapAddColor(cmap, redValue, greenValue, blueValue);
		}

        /// <summary>
        /// Adds the specified color if it doesn't already exist, returning the colors index in the data array.
        /// </summary>
        /// <param name="cmap">The pointer to the colormap instance.</param>
        /// <param name="redValue">The red value</param>
        /// <param name="greenValue">The green value</param>
        /// <param name="blueValue">The blue value</param>
        /// <param name="colorIndex">The index of the new color if it was added, or the existing color if it already existed.</param>
        /// <returns>Returns 0 for success, 1 for error, 2 for not enough space.</returns>
        public static int pixcmapAddNewColor(IntPtr cmap, int redValue, int greenValue, int blueValue, out int colorIndex)
		{
			return _implementation.pixcmapAddNewColor(cmap, redValue, greenValue, blueValue, out colorIndex);
		}

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
        public static int pixcmapAddNearestColor(IntPtr cmap, int redValue, int greenValue, int blueValue, out int colorIndex)
		{
			return _implementation.pixcmapAddNearestColor(cmap, redValue, greenValue, blueValue, out colorIndex);
		}

        /// <summary>
        /// Checks if the color already exists or if their is enough room to add it.
        /// </summary>
        /// <param name="cmap">The pointer to the colormap instance.</param>
        /// <param name="redValue">The red value</param>
        /// <param name="greenValue">The green value</param>
        /// <param name="blueValue">The blue value</param>
        /// <param name="usable">Returns 1 if usable; 0 if not.</param>
        /// <returns>Returns 0 if OK, 1 on error.</returns>
        public static int pixcmapUsableColor(IntPtr cmap, int redValue, int greenValue, int blueValue, out int usable)
		{
			return _implementation.pixcmapUsableColor(cmap, redValue, greenValue, blueValue, out usable);
		}

        /// <summary>
        /// Adds a color (black\white) if not already there returning it's index through <paramref name="index"/>.
        /// </summary>
        /// <param name="cmap">The pointer to the colormap instance.</param>
        /// <param name="color">The color to add (0 for black; 1 for white)</param>
        /// <param name="index">The index of the color.</param>
        /// <returns>Returns 0 if OK; 1 on error.</returns>
        public static int pixcmapAddBlackOrWhite(IntPtr cmap, int color, out int index)
		{
			return _implementation.pixcmapAddBlackOrWhite(cmap, color, out index);
		}

        /// <summary>
        /// Sets the darkest color in the colormap to black, if <paramref name="setBlack"/> is 1. 
        /// Sets the lightest color in the colormap to white if <paramref name="setWhite"/> is 1. 
        /// </summary>
        /// <param name="cmap">The pointer to the colormap instance.</param>
        /// <param name="setBlack">0 for no operation; 1 to set darket color to black</param>
        /// <param name="setBlack">0 for no operation; 1 to set lightest color to white</param>
        /// <returns>Returns 0 if OK; 1 on error.</returns>
        public static int pixcmapSetBlackAndWhite(IntPtr cmap, int setBlack, int setWhite)
		{
			return _implementation.pixcmapSetBlackAndWhite(cmap, setBlack, setWhite);
		}

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
        public static int pixcmapGetColor(IntPtr cmap, int index, out int redValue, out int blueValue, out int greenValue)
		{
			return _implementation.pixcmapGetColor(cmap, index, out redValue, out blueValue, out greenValue);
		}

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
        public static int pixcmapGetColor32(IntPtr cmap, int index, out int color)
		{
			return _implementation.pixcmapGetColor32(cmap, index, out color);
		}

        /// <summary>
        /// Sets a previously allocated color entry.
        /// </summary>
        /// <param name="cmap">The pointer to the colormap instance.</param>
        /// <param name="index">The index of the colormap entry</param>
        /// <param name="redValue"></param>
        /// <param name="blueValue"></param>
        /// <param name="greenValue"></param>
        /// <returns>Returns 0 if OK; 1 if not accessable (caller should check).</returns>
        public static int pixcmapResetColor(IntPtr cmap, int index, int redValue, int blueValue, int greenValue)
		{
			return _implementation.pixcmapResetColor(cmap, index, redValue, blueValue, greenValue);
		}

        /// <summary>
        /// Gets the index of the color entry with the specified color, return 0 if found; 1 if not.
        /// </summary>
        public static int pixcmapGetIndex(IntPtr cmap, int redValue, int blueValue, int greenValue, out int index)
		{
			return _implementation.pixcmapGetIndex(cmap, redValue, blueValue, greenValue, out index);
		}


        /// <summary>
        /// Returns 0 if the color exists in the color map; otherwise 1.
        /// </summary>
        /// <returns>Returns 0 if OK; 1 on error.</returns>
        public static int pixcmapHasColor(IntPtr cmap, int color)
		{
			return _implementation.pixcmapHasColor(cmap, color);
		}


        /// <summary>
        /// Returns the number of unique grey colors including black and white.
        /// </summary>
        /// <returns>Returns 0 if OK; 1 on error.</returns>
        public static int pixcmapCountGrayColors(IntPtr cmap, out int ngray)
		{
			return _implementation.pixcmapCountGrayColors(cmap, out ngray);
		}
        
        /// <summary>
        /// Finds the index of the color entry with the rank intensity.
        /// </summary>
        /// <returns>Returns 0 if OK; 1 on error.</returns>
        public static int pixcmapGetRankIntensity(IntPtr cmap, float rankVal, out int index)
		{
			return _implementation.pixcmapGetRankIntensity(cmap, rankVal, out index);
		}


        /// <summary>
        /// Finds the index of the color entry closest to the specified color.
        /// </summary>
        /// <returns>Returns 0 if OK; 1 on error.</returns>
        public static int pixcmapGetNearestIndex(IntPtr cmap, int rVal, int bVal, int gVal, out int index)
		{
			return _implementation.pixcmapGetNearestIndex(cmap, rVal, bVal, gVal, out index);
		}

        /// <summary>
        /// Finds the index of the color entry closest to the specified color.
        /// </summary>
        /// <remarks>
        /// Should only be used on gray colormaps.
        /// </remarks>
        /// <returns>Returns 0 if OK; 1 on error.</returns>
        public static int pixcmapGetNearestGrayIndex(IntPtr cmap, int val, out int index)
		{
			return _implementation.pixcmapGetNearestGrayIndex(cmap, val, out index);
		}

        public static int pixcmapGetComponentRange(IntPtr cmap, int component, out int minVal, out int maxVal)
		{
			return _implementation.pixcmapGetComponentRange(cmap, component, out minVal, out maxVal);
		}

        public static int pixcmapGetExtremeValue(IntPtr cmap, int type, out int rVal, out int gVal, out int bVal)
		{
			return _implementation.pixcmapGetExtremeValue(cmap, type, out rVal, out gVal, out bVal);
		}

        // color map conversion

        public static IntPtr pixcmapGrayToColor(int color)
		{
			return _implementation.pixcmapGrayToColor(color);
		}


        public static IntPtr pixcmapColorToGray(IntPtr cmaps, float redWeight, float greenWeight, float blueWeight)
		{
			return _implementation.pixcmapColorToGray(cmaps, redWeight, greenWeight, blueWeight);
		}

        // colormap serialization

        public static int pixcmapToArrays(IntPtr cmap,  out IntPtr redMap, out IntPtr blueMap, out IntPtr greenMap)
		{
			return _implementation.pixcmapToArrays(cmap, out redMap, out blueMap, out greenMap);
		}


        public static int pixcmapToRGBTable(IntPtr cmap, out IntPtr colorTable, out int colorCount)
		{
			return _implementation.pixcmapToRGBTable(cmap, out colorTable, out colorCount);
		}


        public static int pixcmapSerializeToMemory(IntPtr cmap, out int components, out int colorCount, out IntPtr colorData, out int colorDataLength)
		{
			return _implementation.pixcmapSerializeToMemory(cmap, out components, out colorCount, out colorData, out colorDataLength);
		}


        public static IntPtr pixcmapSerializeToMemory(IntPtr colorData, int colorCount, int colorDataLength)
		{
			return _implementation.pixcmapSerializeToMemory2(colorData, colorCount, colorDataLength);
		}
        
        // colormap transformations 

        public static int pixcmapGammaTRC(IntPtr cmap, float gamma, int minVal, int maxVal)
		{
			return _implementation.pixcmapGammaTRC(cmap, gamma, minVal, maxVal);
		}


        public static int pixcmapContrastTRC(IntPtr cmap, float factor)
		{
			return _implementation.pixcmapContrastTRC(cmap, factor);
		}
                        
        public static int pixcmapShiftIntensity(IntPtr cmap, float fraction)
		{
			return _implementation.pixcmapShiftIntensity(cmap, fraction);
		}


        #endregion
    }
}
