using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Tesseract.Tests
{
    [DataContract]
    public class TesseractResultSet
    {
        [DataContract]
        public class Page
        {
            public Rect Region { get; set; }
            public List<Block> Blocks { get; set; }
        }

        [DataContract]
        public class Block
        {
            public Rect Region { get; set; }
            public float Confidence { get; set; }
            public string Text { get; set; }

            public List<Line> Lines { get; set; }
        }

        [DataContract]
        public class Line
        {
            public Rect Region { get; set; }
            public float Confidence { get; set; }
            public string Text { get; set; }

            public List<Word> Words { get; set; }
        }

        [DataContract]
        public class Word
        {
            public Rect Region { get; set; }
            public float Confidence { get; set; }
            public string Text { get; set; }
            
            public List<Symbol> Words { get; set; }
        }

        [DataContract]
        public class Symbol
        {
            public Rect Region { get; set; }
            public float Confidence { get; set; }
            public char Char { get; set; }
        }
    }
}
