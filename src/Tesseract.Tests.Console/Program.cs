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
            System.Console.WriteLine("Is 64bit process: {0}", Environment.Is64BitProcess);

            try {
                var testFixture = new Tesseract.Tests.EngineTests();
                testFixture.Initialise_CanLoadConfigFile();
            } catch (Exception e) {
                System.Console.WriteLine("Unhandled exception occured: \r\n{0}", e);
            }
            System.Console.WriteLine("Press any key to exit.");
            System.Console.ReadKey();
        }
    }
}
