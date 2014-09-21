
using System;
using System.Diagnostics;
using NUnit.Framework;

namespace Tesseract.Tests
{
	[TestFixture]
	public class BaseApiTests
	{
		[Test,
		 Ignore("GetVersion doesn't work for x64, probably compilation related.")]
		public void GetVersion_Is302() 
		{
            var version = Interop.TessApi.Native.GetVersion();
			Assert.That(version, Is.EqualTo("3.02"));
		}
	}
}
