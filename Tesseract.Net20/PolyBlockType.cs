using System;
using System.Collections.Generic;
using System.Text;

namespace Tesseract
{
    public enum PolyBlockType : int
    {
        Unknown,
        FlowingText,
        HeadingText,
        PullOutText,
        Table,
        VerticalText,
        CaptionText,
        FlowingImage, 
        HeadingImage,
        PullOutImage,
        HorizontalLine, 
        VerticalLine,
        Noise,
        Count
    }
}
