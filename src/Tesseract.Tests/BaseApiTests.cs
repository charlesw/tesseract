
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
				
	}
}
