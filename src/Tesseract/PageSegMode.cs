using System;
using System.Collections.Generic;
using System.Text;

namespace Tesseract
{
    public enum PageSegMode : int
    {
        OsdOnly,
        AutoOsd, 
        AutoOnly, 
        Auto,
        SingleColumn, 
        SingleBlockVertText,
        SingleBlock, 
        SingleLine, 
        SingleWord, 
        CircleWord, 
        SingleChar, 
        Count
    }
}
