using System;
namespace Tesseract.Interop
{

	internal class LeptonicaImplementation
	{
		internal delegate IntPtr pixCreateFn(int width, int height, int depth);
		internal pixCreateFn pixCreate;

		internal delegate void pixDestroyFn(ref IntPtr pix);
		internal pixDestroyFn pixDestroy;

		internal delegate int pixGetWidthFn(IntPtr pix);
		internal pixGetWidthFn pixGetWidth;

		internal delegate int pixGetHeightFn(IntPtr pix);
		internal pixGetHeightFn pixGetHeight;

		internal delegate int pixGetDepthFn(IntPtr pix);
		internal pixGetDepthFn pixGetDepth;

		internal delegate int pixGetXResFn(IntPtr pix);
		internal pixGetXResFn pixGetXRes;

		internal delegate int pixGetYResFn(IntPtr pix);
		internal pixGetYResFn pixGetYRes;

		internal delegate int pixGetResolutionFn(IntPtr pix, out int xres, out int yres);
		internal pixGetResolutionFn pixGetResolution;

		internal delegate int pixGetWplFn(IntPtr pix);
		internal pixGetWplFn pixGetWpl;

		internal delegate int pixSetXResFn(IntPtr pix, int xres);
		internal pixSetXResFn pixSetXRes;

		internal delegate int pixSetYResFn(IntPtr pix, int yres);
		internal pixSetYResFn pixSetYRes;

		internal delegate int pixSetResolutionFn(IntPtr pix, int xres, int yres);
		internal pixSetResolutionFn pixSetResolution;

		internal delegate int pixScaleResolutionFn(IntPtr pix, float xscale, float yscale);
		internal pixScaleResolutionFn pixScaleResolution;

		internal delegate IntPtr pixGetDataFn(IntPtr pix);
		internal pixGetDataFn pixGetData;

		internal delegate ImageFormat pixGetInputFormatFn(IntPtr pix);
		internal pixGetInputFormatFn pixGetInputFormat;

		internal delegate int pixSetInputFormatFn(IntPtr pix, ImageFormat inputFormat);
		internal pixSetInputFormatFn pixSetInputFormat;

		internal delegate int pixEndianByteSwapFn(IntPtr pix);
		internal pixEndianByteSwapFn pixEndianByteSwap;

		internal delegate IntPtr pixReadFn(string filename);
		internal pixReadFn pixRead;

		internal delegate int pixWriteFn(string filename, IntPtr handle, ImageFormat format);
		internal pixWriteFn pixWrite;

		internal delegate IntPtr pixGetColormapFn(IntPtr pix);
		internal pixGetColormapFn pixGetColormap;

		internal delegate int pixSetColormapFn(IntPtr pix, IntPtr pixCmap);
		internal pixSetColormapFn pixSetColormap;

		internal delegate int pixDestroyColormapFn(IntPtr pix);
		internal pixDestroyColormapFn pixDestroyColormap;

		internal delegate IntPtr pixConvertRGBToGrayFn(IntPtr pix, float rwt, float gwt, float bwt);
		internal pixConvertRGBToGrayFn pixConvertRGBToGray;

		internal delegate IntPtr pixDeskewGeneralFn(IntPtr pix, int redSweep, float sweepRange, float sweepDelta, int redSearch, int thresh, out float pAngle, out float pConf);
		internal pixDeskewGeneralFn pixDeskewGeneral;

		internal delegate int pixOtsuAdaptiveThresholdFn(IntPtr pix, int sx, float sy, float smoothx, int smoothy, float scorefract, out IntPtr ppixth, out IntPtr ppixd);
		internal pixOtsuAdaptiveThresholdFn pixOtsuAdaptiveThreshold;

		internal delegate IntPtr pixcmapCreateFn(int depth);
		internal pixcmapCreateFn pixcmapCreate;

		internal delegate IntPtr pixcmapCreateRandomFn(int depth, int hasBlack, int hasWhite);
		internal pixcmapCreateRandomFn pixcmapCreateRandom;

		internal delegate IntPtr pixcmapCreateLinearFn(int depth, int levels);
		internal pixcmapCreateLinearFn pixcmapCreateLinear;

		internal delegate IntPtr pixcmapCopyFn(IntPtr cmaps);
		internal pixcmapCopyFn pixcmapCopy;

		internal delegate void pixcmapDestroyFn(ref IntPtr cmap);
		internal pixcmapDestroyFn pixcmapDestroy;

		internal delegate int pixcmapGetCountFn(IntPtr cmap);
		internal pixcmapGetCountFn pixcmapGetCount;

		internal delegate int pixcmapGetFreeCountFn(IntPtr cmap);
		internal pixcmapGetFreeCountFn pixcmapGetFreeCount;

		internal delegate int pixcmapGetDepthFn(IntPtr cmap);
		internal pixcmapGetDepthFn pixcmapGetDepth;

		internal delegate int pixcmapGetMinDepthFn(IntPtr cmap, out int minDepth);
		internal pixcmapGetMinDepthFn pixcmapGetMinDepth;

		internal delegate int pixcmapClearFn(IntPtr cmap);
		internal pixcmapClearFn pixcmapClear;

		internal delegate int pixcmapAddColorFn(IntPtr cmap, int redValue, int greenValue, int blueValue);
		internal pixcmapAddColorFn pixcmapAddColor;

		internal delegate int pixcmapAddNewColorFn(IntPtr cmap, int redValue, int greenValue, int blueValue, out int colorIndex);
		internal pixcmapAddNewColorFn pixcmapAddNewColor;

		internal delegate int pixcmapAddNearestColorFn(IntPtr cmap, int redValue, int greenValue, int blueValue, out int colorIndex);
		internal pixcmapAddNearestColorFn pixcmapAddNearestColor;

		internal delegate int pixcmapUsableColorFn(IntPtr cmap, int redValue, int greenValue, int blueValue, out int usable);
		internal pixcmapUsableColorFn pixcmapUsableColor;

		internal delegate int pixcmapAddBlackOrWhiteFn(IntPtr cmap, int color, out int index);
		internal pixcmapAddBlackOrWhiteFn pixcmapAddBlackOrWhite;

		internal delegate int pixcmapSetBlackAndWhiteFn(IntPtr cmap, int setBlack, int setWhite);
		internal pixcmapSetBlackAndWhiteFn pixcmapSetBlackAndWhite;

		internal delegate int pixcmapGetColorFn(IntPtr cmap, int index, out int redValue, out int blueValue, out int greenValue);
		internal pixcmapGetColorFn pixcmapGetColor;

		internal delegate int pixcmapGetColor32Fn(IntPtr cmap, int index, out int color);
		internal pixcmapGetColor32Fn pixcmapGetColor32;

		internal delegate int pixcmapResetColorFn(IntPtr cmap, int index, int redValue, int blueValue, int greenValue);
		internal pixcmapResetColorFn pixcmapResetColor;

		internal delegate int pixcmapGetIndexFn(IntPtr cmap, int redValue, int blueValue, int greenValue, out int index);
		internal pixcmapGetIndexFn pixcmapGetIndex;

		internal delegate int pixcmapHasColorFn(IntPtr cmap, int color);
		internal pixcmapHasColorFn pixcmapHasColor;

		internal delegate int pixcmapCountGrayColorsFn(IntPtr cmap, out int ngray);
		internal pixcmapCountGrayColorsFn pixcmapCountGrayColors;

		internal delegate int pixcmapGetRankIntensityFn(IntPtr cmap, float rankVal, out int index);
		internal pixcmapGetRankIntensityFn pixcmapGetRankIntensity;

		internal delegate int pixcmapGetNearestIndexFn(IntPtr cmap, int rVal, int bVal, int gVal, out int index);
		internal pixcmapGetNearestIndexFn pixcmapGetNearestIndex;

		internal delegate int pixcmapGetNearestGrayIndexFn(IntPtr cmap, int val, out int index);
		internal pixcmapGetNearestGrayIndexFn pixcmapGetNearestGrayIndex;

		internal delegate int pixcmapGetComponentRangeFn(IntPtr cmap, int component, out int minVal, out int maxVal);
		internal pixcmapGetComponentRangeFn pixcmapGetComponentRange;

		internal delegate int pixcmapGetExtremeValueFn(IntPtr cmap, int type, out int rVal, out int gVal, out int bVal);
		internal pixcmapGetExtremeValueFn pixcmapGetExtremeValue;

		internal delegate IntPtr pixcmapGrayToColorFn(int color);
		internal pixcmapGrayToColorFn pixcmapGrayToColor;

		internal delegate IntPtr pixcmapColorToGrayFn(IntPtr cmaps, float redWeight, float greenWeight, float blueWeight);
		internal pixcmapColorToGrayFn pixcmapColorToGray;

		internal delegate int pixcmapToArraysFn(IntPtr cmap, out IntPtr redMap, out IntPtr blueMap, out IntPtr greenMap);
		internal pixcmapToArraysFn pixcmapToArrays;

		internal delegate int pixcmapToRGBTableFn(IntPtr cmap, out IntPtr colorTable, out int colorCount);
		internal pixcmapToRGBTableFn pixcmapToRGBTable;

		internal delegate int pixcmapSerializeToMemoryFn(IntPtr cmap, out int components, out int colorCount, out IntPtr colorData, out int colorDataLength);
		internal pixcmapSerializeToMemoryFn pixcmapSerializeToMemory;

		internal delegate IntPtr pixcmapSerializeToMemoryFn2(IntPtr colorData, int colorCount, int colorDataLength);
		internal pixcmapSerializeToMemoryFn2 pixcmapSerializeToMemory2;

		internal delegate int pixcmapGammaTRCFn(IntPtr cmap, float gamma, int minVal, int maxVal);
		internal pixcmapGammaTRCFn pixcmapGammaTRC;

		internal delegate int pixcmapContrastTRCFn(IntPtr cmap, float factor);
		internal pixcmapContrastTRCFn pixcmapContrastTRC;

		internal delegate int pixcmapShiftIntensityFn(IntPtr cmap, float fraction);
		internal pixcmapShiftIntensityFn pixcmapShiftIntensity;

	}
}
