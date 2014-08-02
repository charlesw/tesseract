using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;

using NUnit.Framework;

namespace Tesseract.Tests.Leptonica
{
	[TestFixture]
	public class LeptonicaPerformanceTests
	{
		/// <summary>
		/// 
		/// </summary>
		[Test, Ignore]
		public void ConvertToBitmap()
		{
			const double BaseRunTime = 793.382;
			const int Runs = 1000;
			
            var sourceFilePath = Path.Combine("./Data/Conversion", "photo_palette_8bpp.tif");
            using(var bmp = new Bitmap(sourceFilePath)) {
            	// Don't include the first conversion since it will also handle loading the library etc (upfront costs).
            	using(var pix = PixConverter.ToPix(bmp)) {            		
            	}
            	
            	// copy 100 times take the average
            	Stopwatch watch = new Stopwatch();
            	watch.Start();
            	for (int i = 0; i < Runs; i++) {	            		
	            	using(var pix = PixConverter.ToPix(bmp)) {            		
	            	}
            	}
            	watch.Stop();
            	
            	var delta = watch.ElapsedTicks / (BaseRunTime * Runs);
            	Console.WriteLine("Delta: {0}", delta);
            	Console.WriteLine("Elapsed Ticks: {0}", watch.ElapsedTicks);
            	Console.WriteLine("Elapsed Time: {0}ms", watch.ElapsedMilliseconds);
            	Console.WriteLine("Average Time: {0}ms", (double)watch.ElapsedMilliseconds/Runs);
            	
            	Assert.That(delta, Is.EqualTo(1.0).Within(0.25));
            }
		}
	}
}
