using System;
using System.Collections.Generic;
using System.Text;

namespace Tesseract
{
    public enum ImageFormat : int
    {
        Unknown = 0,
        Bmp = 1,
        JfifJpeg = 2,
        Png = 3,
        Tiff = 4,
        TiffPackBits = 5,
        TiffRle = 6,
        TiffG3 = 7,
        TiffG4 = 8,
        TiffLzw =  9,
        TifZip = 10,
        Pnm = 11,
        Ps = 12,
        Gif = 13,
        Jp2 = 14,
        WebP = 15,
        Lpdf = 16,
        Default = 17,
        Spix = 18
    }
}
