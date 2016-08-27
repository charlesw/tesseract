using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tesseract
{
    public interface IResultRenderer : IDisposable
    {
        IDisposable BeginDocument(string name);

        void AddPage(Page page);
    }
}
