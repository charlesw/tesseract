using System;
using System.Collections.Generic;
using System.Text;

namespace Tesseract
{
    public enum PageIteratorLevel : int
    {
        Block,
        Para, 
        TextLine, 
        Word, 
        Symbol
    }
}
