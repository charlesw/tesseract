
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Tesseract;

namespace Tesseract.ConsoleDemo
{
	class Program
	{
		public static void Main(string[] args)
		{
            var testImagePath = "./phototest.tif";
            if (args.Length > 0) {
                testImagePath = args[0];
            }

			try {
                var logger = new FormattedConsoleLogger();
                var resultPrinter = new ResultPrinter(logger);
                using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default)) {
                    using (var img = Pix.LoadFromFile(testImagePath)) {
                        using (logger.Begin("Process image")) {
                            using (var page = engine.Process(img)) {
                                var text = page.GetText();
                                logger.Log("Text: {0}", text);
                                logger.Log("Mean confidence: {0}", page.GetMeanConfidence());

                                using (var iter = page.GetIterator()) {
                                    iter.Begin();
									do {
	                                    do {
                                            do {
												do {
                                                    if (iter.IsAtBeginningOf(PageIteratorLevel.Block)) {
                                                        logger.Log("New block");
                                                    }
                                                    if (iter.IsAtBeginningOf(PageIteratorLevel.Para)) {
                                                        logger.Log("New paragraph");
                                                    }
                                                    if (iter.IsAtBeginningOf(PageIteratorLevel.TextLine)) {
                                                        logger.Log("New line");
                                                    }
													if (iter.IsAtBeginningOf(PageIteratorLevel.Word)) {
                                                        logger.Log("New word");
                                                    }
                                                    logger.Log(iter.GetText(PageIteratorLevel.Symbol));
												} while (iter.Next(PageIteratorLevel.Word, PageIteratorLevel.Symbol));
                                            } while (iter.Next(PageIteratorLevel.TextLine, PageIteratorLevel.Word));
	                                    } while (iter.Next(PageIteratorLevel.Para, PageIteratorLevel.TextLine));
									} while(iter.Next(PageIteratorLevel.Block, PageIteratorLevel.Para));									
                                }
                            }
                        }
                    }
                }
			} catch (Exception e) {
				Trace.TraceError(e.ToString());
				Console.WriteLine("Unexpected Error: " + e.Message);
				Console.WriteLine("Details: ");
				Console.WriteLine(e.ToString());
			}
			Console.Write("Press any key to continue . . . ");
			Console.ReadKey(true);
		}



        private class ResultPrinter
        {
            private FormattedConsoleLogger logger;

            public ResultPrinter(FormattedConsoleLogger logger)
            {
                this.logger = logger;
            }

            public void Print(ResultIterator iter)
            {
                logger.Log("Is beginning of block: {0}", iter.IsAtBeginningOf(PageIteratorLevel.Block));
                logger.Log("Is beginning of para: {0}", iter.IsAtBeginningOf(PageIteratorLevel.Para));
                logger.Log("Is beginning of text line: {0}", iter.IsAtBeginningOf(PageIteratorLevel.TextLine));
                logger.Log("Is beginning of word: {0}", iter.IsAtBeginningOf(PageIteratorLevel.Word));
                logger.Log("Is beginning of symbol: {0}", iter.IsAtBeginningOf(PageIteratorLevel.Symbol));

                logger.Log("Block text: \"{0}\"", iter.GetText(PageIteratorLevel.Block));
                logger.Log("Para text: \"{0}\"", iter.GetText(PageIteratorLevel.Para));
                logger.Log("TextLine text: \"{0}\"", iter.GetText(PageIteratorLevel.TextLine));
                logger.Log("Word text: \"{0}\"", iter.GetText(PageIteratorLevel.Word));
                logger.Log("Symbol text: \"{0}\"", iter.GetText(PageIteratorLevel.Symbol));
            }
        }
	}
}