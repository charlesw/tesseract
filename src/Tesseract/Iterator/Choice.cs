using System;
using System.Collections.Generic;
using System.Text;

namespace Tesseract
{
    public struct Choice
    {
        /// <summary>
        /// Returns the confidence of the choice.        
        /// </summary>
        /// <remarks>
        /// The number should be interpreted as a percent probability. (0.0f-100.0f)
        /// </remarks>
        public readonly float Confidence;

        /// <summary>
        /// Returns the text string for the choice.
        /// </summary>
        public readonly string Text;

        internal Choice(string text, float confidence)
        {
            Text = text;
            Confidence = confidence;
        }
    }
}
