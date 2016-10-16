using System;

namespace Tesseract
{
    public interface IResultRenderer : IDisposable
    {
        /// <summary>
        /// Begins a new document with the specified <paramref name="title"/>.
        /// </summary>
        /// <param name="title">The title of the new document.</param>
        /// <returns>A handle that when disposed of ends the current document.</returns>
        IDisposable BeginDocument(string title);

        /// <summary>
        /// Add the page to the current document.
        /// </summary>
        /// <param name="page"></param>
        /// <returns><c>True</c> if the page was successfully added to the result renderer; otherwise false.</returns>
        bool AddPage(Page page);

        /// <summary>
        /// Gets the current page number; returning -1 if no page has yet been added otherwise the number
        /// of the last added page (starting from 0).
        /// </summary>
        int PageNumber { get; }
    }
}