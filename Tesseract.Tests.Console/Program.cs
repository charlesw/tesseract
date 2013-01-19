using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tesseract.Tests.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var testFixture = new Tesseract.Tests.Leptonica.ConvertBitmapToPixTests();
            testFixture.Convert_PixToBitmap("photo_8.bmp", true);
        }
    }
}
