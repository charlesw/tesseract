using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tesseract.Tests
{
    /// <summary>
	/// Represents a test run.
	/// </summary>
	public class TestRun
    {
        private TestRun()
        {
            StartedAt = DateTime.Now;
        }

        public DateTime StartedAt { get; private set; }

        public static readonly TestRun Current = new TestRun();
    }
}
