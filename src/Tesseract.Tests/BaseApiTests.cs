
using System;
using System.Diagnostics;
using NUnit.Framework;

namespace Tesseract.Tests
{
	[TestFixture]
	public class BaseApiTests
	{
		[Test]
		public void GetVersion_Is302() 
		{
            var version = Interop.TessApi.GetVersion();
			Assert.That(version, Is.EqualTo("3.02"));
		}
	}
}
