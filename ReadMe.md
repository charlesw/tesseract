A .NET wrapper for [tesseract-ocr](http://code.google.com/p/tesseract-ocr/).

## Dependencies

### Visual Studio 2012 x86 and x64 Runtimes 

Since tesseract and leptonica binaries are compiled with Visual Studio 2012 you'll need to ensure you have the 
Visual Studio 2012 Runtime installed. This can be found [here](http://www.microsoft.com/en-us/download/details.aspx?id=30679).

### Tesseract language data

You will also need to download the language data files for tesseract 3.02 from [tesseract-ocr].

## Getting started quickly

1. Add the ``Tesseract`` NuGet Package by running ``Install-Package Tesseract`` from the [Package Manager Console](http://docs.nuget.org/docs/start-here/using-the-package-manager-console).
2. Ensure you have Visual Studio 2012 x86 & x64 runtimes installed (see note above).
3. Download language data files for tesseract 3.02 from [tesseract-ocr] and add them to your project, 
   ensure 'Copy to output directory' is set to Always.
4. Check out the Samples solution ``~/Samples/Tesseract.Samples.sln`` for a working example

If you run into any issues please check out [this](https://github.com/charlesw/tesseract/wiki/Errors) wiki page which details a number common issues and some potential solutions.

## Note for contributors

Please create your pull requests to target the "develop" branch. "Master" is only for released code. Thank you.

## License

Copyright 2012-2014 Charles Weld.

Licensed under the [Apache License, Version 2.0][apache2] (the "License"); you
may not use this software except in compliance with the License. You may obtain
a copy of the License at:

[http://www.apache.org/licenses/LICENSE-2.0][apache2]

Unless required by applicable law or agreed to in writing, software distributed
under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR
CONDITIONS OF ANY KIND, either express or implied. See the License for the
specific language governing permissions and limitations under the License.

### InteropDotNet

Copyright 2014 Andrey Akinshin
Project URL: https://github.com/AndreyAkinshin/InteropDotNet
 Distributed under the MIT License: http://opensource.org/licenses/MIT

## Core Team

* [charlesw](https://github.com/charlesw) (Charles Weld)

## Contributors

A big thanks to GitHub and all of Tesseract's contributors:

* [AndreyAkinshin](https://github.com/AndreyAkinshin)
* [jakesays](https://github.com/jakesays)
* [peters](https://github.com/peters)
* [nguyenq](https://github.com/nguyenq)
* [Sojin1989](https://github.com/Sojin1989)
* [jeschergui](https://github.com/jeschergui)

Also thanks to the following projects\resources without which this project would not exist in it's current form:

* [InteropDotNet](https://github.com/AndreyAkinshin/InteropDotNet) - For developing a dynamic interop system that allows tesseract to be used from both mono and .net.
* [Reactive Extensions](http://rx.codeplex.com/) - The basic idea from which the build\packaging system is built on.
* [TwainDotNet](https://github.com/tmyroadctfig/twaindotnet) - Batch build script
* [Tesseract-dot-net](https://code.google.com/p/tesseractdotnet) - The origianl dot net wrapper that started all this.
* [Interop with Native Libraries](http://www.mono-project.com/Interop_with_Native_Libraries) - Stacks of useful information about c# P/Invoke and Marshaling

[apache2]: http://www.apache.org/licenses/LICENSE-2.0
[tesseract-ocr]: https://github.com/tesseract-ocr/tesseract
