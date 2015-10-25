using System;

namespace Tesseract.Tests.Console
{
    internal class Program
    {
        private const int DefaultIterations = 10000;

        public static void Main(string[] args)
        {
            System.Console.WriteLine("Is 64bit process: {0}", Environment.Is64BitProcess);

            try {
                var program = new Program();

                using (var pix = Pix.LoadFromFile("./phototest2.png")) {
                    // Convert RGB to grayscale (required for binarize operations).
                    using (Pix grayPix = pix.Depth == 32 ? pix.ConvertRGBToGray() : pix.Clone()) {
                        //program.MemoryLeakDetector(grayPix, tempPix => tempPix.BinarizeSauvolaTiled(50, 0.35f, 8, 8));
                        //program.MemoryLeakDetector(grayPix, tempPix => tempPix.BinarizeSauvola(50, 0.35f, true));
                        //program.MemoryLeakDetector(grayPix, tempPix => tempPix.BinarizeOtsuAdaptiveThreshold(50, 50, 5, 5, 0.1f));

                        // Test clone
                        program.MemoryLeakDetector(grayPix, tempPix => tempPix.Clone());
                    }
                }
            } catch (Exception e) {
                System.Console.WriteLine("Unhandled exception occured: \r\n{0}", e);
            }
            System.Console.WriteLine("Press any key to exit.");
            System.Console.ReadKey();
        }

        public void MemoryLeakDetector(Pix pix, Func<Pix, Pix> op, int iterations = DefaultIterations)
        {
            for (int i = 0; i < iterations; i++) {
                using (Pix result = op(pix)) {
                    //using (Pix binaryPix = grayPix.BinarizeOtsuAdaptiveThreshold(50, 50, 5, 5, 0.1f)) {
                    System.Console.WriteLine("Memory: {0} MB", System.Diagnostics.Process.GetCurrentProcess().WorkingSet64 / (1024 * 1024));
                }

                GC.Collect();
            }
        }
    }
}