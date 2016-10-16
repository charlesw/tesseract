using System;

namespace Tesseract
{
    public interface IResultRenderer : IDisposable
    {
        IDisposable BeginDocument(string name);

        void AddPage(Page page);

        /// <summary>
        /// Gets the current page number; returning -1 if no page has yet been added otherwise the number 
        /// of the last added page (starting from 0).
        /// </summary>
        int PageNumber { get; }
    }
}
