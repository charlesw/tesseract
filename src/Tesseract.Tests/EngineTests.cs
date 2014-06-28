
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using NUnit.Framework;

namespace Tesseract.Tests
{
	[TestFixture]
	public class EngineTests
	{
		private IEnumerable<string> DataPaths()
		{
			return new string[] {
				null,
				@".",
				@".\",
				@"./",
				@"./tessdata",
				@"./tessdata/",
				@".\tessdata\",
				Path.Combine(Environment.CurrentDirectory, @"tessdata"),
				Path.Combine(Environment.CurrentDirectory, @"tessdata\"),
				Path.Combine(Environment.CurrentDirectory, @"tessdata/"),
				Environment.CurrentDirectory
			};
		}
		
		[Test]		
		public void Initialise_ShouldStartEngine(
			[ValueSource("DataPaths")] string datapath)
		{
			using (var engine = new TesseractEngine(datapath, "eng", EngineMode.Default)) {
				
				
			}
		}
		
		[Test]
		public void Initialise_WithTessDataPrefixSet()
		{
			Environment.SetEnvironmentVariable("TESSDATA_PREFIX", Environment.CurrentDirectory);
			using (var engine = new TesseractEngine(null, "eng", EngineMode.Default)) {				
				
			}
		}
		
		[Test, Ignore("Missing russian language data")]
		public void Initialise_Rus_ShouldStartEngine()
		{
			using (var engine = new TesseractEngine(@"./tessdata", "rus", EngineMode.Default)) {
				
				
			}
		}
		[Test]
		public void Initialise_ShouldThrowErrorIfDatapathNotCorrect()
		{
			Assert.That(() => {
				using (var engine = new TesseractEngine(@"./IDontExist", "eng", EngineMode.Default)) {
					
					
				}			            
			}, Throws.InstanceOf(typeof(TesseractException)));
		}

		[Test]
		public void CanProcessPix()
		{
			using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default)) {
				using (var img = Pix.LoadFromFile("./phototest.tif")) {
					using (var page = engine.Process(img)) {
						var text = page.GetText();

						const string expectedText =
							"This is a lot of 12 point text to test the\nocr code and see if it works on all types\nof file format.\n\nThe quick brown dog jumped over the\nlazy fox. The quick brown dog jumped\nover the lazy fox. The quick brown dog\njumped over the lazy fox. The quick\nbrown dog jumped over the lazy fox.\n\n";

						Assert.That(text, Is.EqualTo(expectedText));
					}
				}
			}
		}
        
		[Test]
		public void CanProcessMultiplePixs()
		{
			using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default)) {
				for (int i = 0; i < 3; i++) {
					using (var img = Pix.LoadFromFile("./phototest.tif")) {
						using (var page = engine.Process(img)) {
							var text = page.GetText();

							const string expectedText =
								"This is a lot of 12 point text to test the\nocr code and see if it works on all types\nof file format.\n\nThe quick brown dog jumped over the\nlazy fox. The quick brown dog jumped\nover the lazy fox. The quick brown dog\njumped over the lazy fox. The quick\nbrown dog jumped over the lazy fox.\n\n";

							Assert.That(text, Is.EqualTo(expectedText));
						}
					}
                    
				}
			}
		}
        
		[Test]
		public void CanProcessBitmap()
		{
			using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default)) {
				using (var img = new Bitmap("./phototest.tif")) {
					using (var page = engine.Process(img)) {
						var text = page.GetText();

						const string expectedText =
							"This is a lot of 12 point text to test the\nocr code and see if it works on all types\nof file format.\n\nThe quick brown dog jumped over the\nlazy fox. The quick brown dog jumped\nover the lazy fox. The quick brown dog\njumped over the lazy fox. The quick\nbrown dog jumped over the lazy fox.\n\n";

						Assert.That(text, Is.EqualTo(expectedText));
					}
				}
			}
		}
				
		[Test]
		public void CanProcessDifferentRegionsInSameImage()
		{
			using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default)) {
				using (var img = Pix.LoadFromFile("./phototest.tif")) {
					using (var page = engine.Process(img, Rect.FromCoords(0, 0, img.Width, 188))) {
						var region1Text = page.GetText();
						
						const string expectedTextRegion1 =
							"This is a lot of 12 point text to test the\nocr code and see if it works on all types\nof file format.\n\n";
						
						Assert.That(region1Text, Is.EqualTo(expectedTextRegion1));
						
						page.RegionOfInterest = Rect.FromCoords(0, 188, img.Width, img.Height);
						
						var region2Text = page.GetText();
						const string expectedTextRegion2 =
							"The quick brown dog jumped over the\nlazy fox. The quick brown dog jumped\nover the lazy fox. The quick brown dog\njumped over the lazy fox. The quick\nbrown dog jumped over the lazy fox.\n\n";
						
						Assert.That(region2Text, Is.EqualTo(expectedTextRegion2));
					}
				}
			}
		}
        
		[Test]
		public void CanParseUznFile()
		{
			using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default)) {
				var inputFilename = TestFilePath(@"Ocr\uzn-test.png");
				using (var img = Pix.LoadFromFile(inputFilename)) {
					using (var page = engine.Process(img, inputFilename, PageSegMode.SingleLine)) {
						var text = page.GetText();

						const string expectedText =
							"This is another test\n\n";

						Assert.That(text, Is.EqualTo(expectedText));
					}
				}
			}
		}
        
		#region Variable set\get
        
		[Test]
		[TestCase(false)]
		[TestCase(true)]
		public void CanSetBooleanVariable(bool variableValue)
		{
			const string VariableName = "classify_enable_learning";
			using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default)) {
				var variableWasSet = engine.SetVariable(VariableName, variableValue);
				Assert.That(variableWasSet, Is.True, "Failed to set variable '{0}'.", VariableName);
				bool result;
				if (engine.TryGetBoolVariable(VariableName, out result)) {
					Assert.That(result, Is.EqualTo(variableValue));
				} else {
					Assert.Fail("Failed to retrieve value for '{0}'.", VariableName);
				}
			}
		}
        
		[Test]
		[TestCase("edges_children_count_limit", 45)]
		[TestCase("edges_children_count_limit", 20)]
		[TestCase("textord_testregion_left", 20)]
		[TestCase("textord_testregion_left", -20)]
		public void CanSetIntegerVariable(string variableName, int variableValue)
		{
			using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default)) {
				var variableWasSet = engine.SetVariable(variableName, variableValue);
				Assert.That(variableWasSet, Is.True, "Failed to set variable '{0}'.", variableName);
				int result;
				if (engine.TryGetIntVariable(variableName, out result)) {
					Assert.That(result, Is.EqualTo(variableValue));
				} else {
					Assert.Fail("Failed to retrieve value for '{0}'.", variableName);
				}
			}
		}
                
		[Test]
		[TestCase("edges_boxarea", 0.875)]
		[TestCase("edges_boxarea", 0.9)]
		[TestCase("edges_boxarea", -0.9)]
		public void CanSetDoubleVariable(string variableName, double variableValue)
		{
			using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default)) {
				var variableWasSet = engine.SetVariable(variableName, variableValue);
				Assert.That(variableWasSet, Is.True, "Failed to set variable '{0}'.", variableName);
				double result;
				if (engine.TryGetDoubleVariable(variableName, out result)) {
					Assert.That(result, Is.EqualTo(variableValue));
				} else {
					Assert.Fail("Failed to retrieve value for '{0}'.", variableName);
				}
			}
		}
        
                
		[Test]
		[TestCase("tessedit_char_whitelist", "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ")]
		[TestCase("tessedit_char_whitelist", "")]
		[TestCase("tessedit_char_whitelist", "Test")]
		public void CanSetStringVariable(string variableName, string variableValue)
		{
			using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default)) {
				var variableWasSet = engine.SetVariable(variableName, variableValue);
				Assert.That(variableWasSet, Is.True, "Failed to set variable '{0}'.", variableName);
				string result;
				if (engine.TryGetStringVariable(variableName, out result)) {
					Assert.That(result, Is.EqualTo(variableValue));
				} else {
					Assert.Fail("Failed to retrieve value for '{0}'.", variableName);
				}
			}
		}
        
		#endregion
        
		#region File Helpers
        
		private string TestFilePath(string path)
		{
			var basePath = Path.Combine(Environment.CurrentDirectory, "Data");
			return Path.Combine(basePath, path);
		}
        
		#endregion
	}
}
