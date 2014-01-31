
using System;
using System.Diagnostics;
using NUnit.Framework;

namespace Tesseract.Tests
{
	[TestFixture]
	public class BaseApiTests
	{
		[Test]
		public void CanGetVersion() 
		{
            var version = Interop.TessApi.GetVersion();
			Assert.That(version, Is.EqualTo("3.03"));
		}
		
		[Test]
		public void CanCreateAndSetStringObject()
		{
			var strPtr = Interop.TessApi.TessStringCreate("test");
			var testString = Interop.TessApi.TessStringGetCStr(strPtr);
			Assert.That(testString, Is.EqualTo("test"));
		}
	}
}
