#region Using directives

using System;
using System.Reflection;
using System.Runtime.InteropServices;

#endregion

// Tesseract is versioned using semantic version (http://semver.org/) and takes form:
//
// Major.Minor.Patch

[assembly: AssemblyVersion("$(Version)")]
[assembly: AssemblyFileVersion("$(Version)")]