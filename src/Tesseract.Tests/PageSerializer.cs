using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace Tesseract.Tests
{
    /// <summary>
    /// Serialise the OCR results to a string using the iterator api
    /// </summary>
    class PageSerializer
    {
        public static string Serialize(Page page, bool outputChoices)
        {
            var output = new StringBuilder();
            using (var iter = page.GetIterator())
            {
                iter.Begin();
                do
                {
                    do
                    {
                        do
                        {
                            do
                            {
                                do
                                {
                                    if (iter.IsAtBeginningOf(PageIteratorLevel.Block))
                                    {
                                        var confidence = iter.GetConfidence(PageIteratorLevel.Block) / 100;
                                        Rect bounds;
                                        if (iter.TryGetBoundingBox(PageIteratorLevel.Block, out bounds))
                                        {
                                            output.AppendFormat(CultureInfo.InvariantCulture, "<block confidence=\"{0:P}\" bounds=\"{1}, {2}, {3}, {4}\">", confidence, bounds.X1, bounds.Y1, bounds.X2, bounds.Y2);
                                        }
                                        else
                                        {
                                            output.AppendFormat(CultureInfo.InvariantCulture, "<block confidence=\"{0:P}\">", confidence);
                                        }
                                        output.AppendLine();
                                    }
                                    if (iter.IsAtBeginningOf(PageIteratorLevel.Para))
                                    {
                                        var confidence = iter.GetConfidence(PageIteratorLevel.Para) / 100;
                                        Rect bounds;
                                        if (iter.TryGetBoundingBox(PageIteratorLevel.Para, out bounds))
                                        {
                                            output.AppendFormat(CultureInfo.InvariantCulture, "<para confidence=\"{0:P}\" bounds=\"{1}, {2}, {3}, {4}\">", confidence, bounds.X1, bounds.Y1, bounds.X2, bounds.Y2);
                                        }
                                        else
                                        {
                                            output.AppendFormat(CultureInfo.InvariantCulture, "<para confidence=\"{0:P}\">", confidence);
                                        }
                                        output.AppendLine();
                                    }
                                    if (iter.IsAtBeginningOf(PageIteratorLevel.TextLine))
                                    {
                                        var confidence = iter.GetConfidence(PageIteratorLevel.TextLine) / 100;
                                        Rect bounds;
                                        if (iter.TryGetBoundingBox(PageIteratorLevel.TextLine, out bounds))
                                        {
                                            output.AppendFormat(CultureInfo.InvariantCulture, "<line confidence=\"{0:P}\" bounds=\"{1}, {2}, {3}, {4}\">", confidence, bounds.X1, bounds.Y1, bounds.X2, bounds.Y2);
                                        }
                                        else
                                        {
                                            output.AppendFormat(CultureInfo.InvariantCulture, "<line confidence=\"{0:P}\">", confidence);
                                        }
                                    }
                                    if (iter.IsAtBeginningOf(PageIteratorLevel.Word))
                                    {
                                        var confidence = iter.GetConfidence(PageIteratorLevel.Word) / 100;
                                        Rect bounds;
                                        if (iter.TryGetBoundingBox(PageIteratorLevel.Word, out bounds))
                                        {
                                            output.AppendFormat(CultureInfo.InvariantCulture, "<word confidence=\"{0:P}\" bounds=\"{1}, {2}, {3}, {4}\">", confidence, bounds.X1, bounds.Y1, bounds.X2, bounds.Y2);
                                        }
                                        else
                                        {
                                            output.AppendFormat(CultureInfo.InvariantCulture, "<word confidence=\"{0:P}\">", confidence);
                                        }
                                    }

                                    // symbol and choices
                                    if (outputChoices)
                                    {
                                        using (var choiceIter = iter.GetChoiceIterator())
                                        {
                                            var symbolConfidence = iter.GetConfidence(PageIteratorLevel.Symbol) / 100;
                                            if (choiceIter != null)
                                            {
                                                output.AppendFormat(CultureInfo.InvariantCulture, "<symbol text=\"{0}\" confidence=\"{1:P}\">", iter.GetText(PageIteratorLevel.Symbol), symbolConfidence);
                                                output.Append("<choices>");
                                                do
                                                {
                                                    var choiceConfidence = choiceIter.GetConfidence() / 100;
                                                    output.AppendFormat(CultureInfo.InvariantCulture, "<choice text=\"{0}\" confidence\"{1:P}\"/>", choiceIter.GetText(), choiceConfidence);

                                                } while (choiceIter.Next());
                                                output.Append("</choices>");
                                                output.Append("</symbol>");
                                            }
                                            else
                                            {
                                                output.AppendFormat(CultureInfo.InvariantCulture, "<symbol text=\"{0}\" confidence=\"{1:P}\"/>", iter.GetText(PageIteratorLevel.Symbol), symbolConfidence);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        output.Append(iter.GetText(PageIteratorLevel.Symbol));
                                    }
                                    if (iter.IsAtFinalOf(PageIteratorLevel.Word, PageIteratorLevel.Symbol))
                                    {
                                        output.Append("</word>");
                                    }
                                } while (iter.Next(PageIteratorLevel.Word, PageIteratorLevel.Symbol));

                                if (iter.IsAtFinalOf(PageIteratorLevel.TextLine, PageIteratorLevel.Word))
                                {
                                    output.AppendLine("</line>");
                                }
                            } while (iter.Next(PageIteratorLevel.TextLine, PageIteratorLevel.Word));
                            if (iter.IsAtFinalOf(PageIteratorLevel.Para, PageIteratorLevel.TextLine))
                            {
                                output.AppendLine("</para>");
                            }
                        } while (iter.Next(PageIteratorLevel.Para, PageIteratorLevel.TextLine));
                    } while (iter.Next(PageIteratorLevel.Block, PageIteratorLevel.Para));
                    output.AppendLine("</block>");
                } while (iter.Next(PageIteratorLevel.Block));
            }
            return TestUtils.NormaliseNewLine(output.ToString());
        }
    }
}
