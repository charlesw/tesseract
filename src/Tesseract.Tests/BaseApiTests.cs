
using System;
using System.Diagnostics;
using NUnit.Framework;

namespace Tesseract.Tests
{
	[TestFixture]
	public class BaseApiTests
	{
		[Test]
		public void GetVersion_Is303() 
		{
            var version = Interop.TessApi.Native.GetVersion();
			Assert.That(version, Is.EqualTo("3.03"));
		}
				
	}
}
