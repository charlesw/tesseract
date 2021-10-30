#region Using directives

using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#endregion

// Assembly specific attributes are now autogrenerated from project settings

// This sets the default COM visibility of types in the assembly to invisible.
// If you need to expose a type to COM, use [ComVisible(true)] on that type.
[assembly: ComVisible(false)]

// Ensure internals are visible to the test assembly so we can test them too.
[assembly: InternalsVisibleTo("Tesseract.Net48Tests")]
[assembly: InternalsVisibleTo("Tesseract.NetCore31Tests")]

