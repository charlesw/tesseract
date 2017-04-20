using System;
using System.Collections.Generic;

namespace Tesseract
{
    // The .NET equivalent of the ccstruct/fontinfo.h
    // FontInfo struct. It's missing spacing info
    // since we don't have any way of getting it (and
    // it's probably not all that useful anyway)
    public class FontInfo
    {
        private FontInfo(
            string name, int id,
            bool isItalic, bool isBold, bool isFixedPitch,
            bool isSerif, bool isFraktur = false
        )
        {
            Name = name;
            Id = id;

            IsItalic     = isItalic;
            IsBold       = isBold;
            IsFixedPitch = isFixedPitch;
            IsSerif      = isSerif;
            IsFraktur    = isFraktur;
        }

        public string Name { get; private set; }

        public int  Id           { get; private set; }
        public bool IsItalic     { get; private set; }
        public bool IsBold       { get; private set; }
        public bool IsFixedPitch { get; private set; }
        public bool IsSerif      { get; private set; }
        public bool IsFraktur    { get; private set; }

        private static Dictionary<int, FontInfo> _cache = new Dictionary<int, FontInfo>();

        public static FontInfo GetById(int id) {
            if (_cache.ContainsKey(id)) {
                return _cache[id];
            }
            return null;
        }

        public static object _cacheLock = new object();
        public static FontInfo GetOrCreate(
            string name, int id,
            bool isItalic, bool isBold, bool isFixedPitch,
            bool isSerif, bool isFraktur = false
        )
        {
            lock (_cacheLock) {
                if (_cache.ContainsKey(id)) {
                    return _cache[id];
                }

                var newFont = new FontInfo(name, id, isItalic, isBold, isFixedPitch, isSerif, isFraktur);
                _cache.Add(id, newFont);

                return newFont;
            }
        }
    }
}
