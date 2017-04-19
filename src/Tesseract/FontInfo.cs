using System;
using System.Collections.Generic;

namespace Tesseract
{
    // See github:tesseract-ocr/tesseract#a75ab45
    // ccstruct/fontinfo.h:111 to see where these
    // values came from
    [Flags]
    public enum FontProperties
    {
        None       = 0,
        Italic     = 1 << 0,
        Bold       = 1 << 1,
        FixedPitch = 1 << 2,
        Serif      = 1 << 3,
        Fraktur    = 1 << 4
    }

    // The .NET equivalent of the ccstruct/fontinfo.h
    // FontInfo struct. It's missing spacing info
    // since we don't have any way of getting it (and
    // it's probably not all that useful anyway)
    public class FontInfo
    {
        private FontInfo(string name, int id, FontProperties properties)
        {
            Name = name;
            Id = id;
            Properties = properties;
        }

        private T GetFlag<T>(bool isSet, T flagValue)
        {
            return isSet ? flagValue : default(T);
        }

        // .NET < 4 doesn't have Enum.HasFlag, so for compatibility
        // we'll use our own.
        private bool HasFlag<T>(T value, T flag)
        {
            var val = Convert.ToInt32(value);
            var f   = Convert.ToInt32(flag);

            return (val & f) == f;
        }

        private FontInfo(
            string name, int id,
            bool isItalic, bool isBold, bool isFixedPitch,
            bool isSerif, bool isFraktur = false
        )
        {
            Name = name;
            Id = id;
            Properties =
                GetFlag(isItalic, FontProperties.Italic)
                    | GetFlag(isBold, FontProperties.Bold)
                    | GetFlag(isFixedPitch, FontProperties.FixedPitch)
                    | GetFlag(isSerif, FontProperties.Serif)
                    | GetFlag(isFraktur, FontProperties.Fraktur);
        }

        public string Name { get; private set; }
        public int    Id   { get; private set; }

        public FontProperties Properties { get; private set; }

        public bool IsItalic     { get { return HasFlag(Properties, FontProperties.Italic);     } }
        public bool IsBold       { get { return HasFlag(Properties, FontProperties.Bold);       } }
        public bool IsFixedPitch { get { return HasFlag(Properties, FontProperties.FixedPitch); } }
        public bool IsSerif      { get { return HasFlag(Properties, FontProperties.Serif);      } }
        public bool IsFraktur    { get { return HasFlag(Properties, FontProperties.Fraktur);    } }

        private static Dictionary<int, FontInfo> _cache = new Dictionary<int, FontInfo>();

        public static FontInfo GetById(int id) {
            if (_cache.ContainsKey(id)) {
                return _cache[id];
            }
            return null;
        }

        public static FontInfo GetOrCreate(
            string name, int id,
            bool isItalic, bool isBold, bool isFixedPitch,
            bool isSerif, bool isFraktur = false
        )
        {
            if (_cache.ContainsKey(id)) {
                return _cache[id];
            }

            var newFont = new FontInfo(name, id, isItalic, isBold, isFixedPitch, isSerif, isFraktur);
            _cache.Add(id, newFont);

            return newFont;
        }
    }
}
