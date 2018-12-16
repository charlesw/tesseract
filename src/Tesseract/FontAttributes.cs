using System;

namespace Tesseract
{
    // This class is the return type of
    // ResultIterator.GetWordFontAttributes().  We can't
    // use FontInfo directly because there are properties
    // here that are not accounted for in FontInfo
    // (smallcaps, underline, etc.)  Because of the caching
    // scheme we're using for FontInfo objects, we can't simply
    // augment that class since these extra properties are not
    // accounted for by the FontInfo's unique ID.
    public class FontAttributes
    {
        public FontInfo FontInfo { get; private set; }

        public bool IsUnderlined { get; private set; }
        public bool IsSmallCaps { get; private set; }
        public int PointSize { get; private set; }

        public FontAttributes(
            FontInfo fontInfo, bool isUnderlined, bool isSmallCaps, int pointSize)
        {
            FontInfo = fontInfo;
            IsUnderlined = isUnderlined;
            IsSmallCaps = isSmallCaps;
            PointSize = pointSize;
        }
    }
}
