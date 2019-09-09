using System;
using System.Collections.Generic;
using System.Text;

namespace Tesseract
{
    public enum PageIteratorLevel : int
    {
        Block = 0,
        Para = 1,
        TextLine = 2,
        Word = 3,
        Symbol = 4
    }
}