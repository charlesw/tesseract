## Version 1 Series

...

### Version 1.10

* Added support for uzn files - [Issue 66](https://github.com/charlesw/tesseract/issues/66)

### Version 1.11

* Allow changing the current region of interest without having to reload the entire image (Page.RegionOfInterest)
* Fixed loader for ASP.NET [Issue 97](https://github.com/charlesw/tesseract/issues/97)

### Version 1.12

* Automatically strip '\' and '/' characters of path and remove tessdata prefix.
* Fixed bug introduced in previous region of interest
* Don't dispose of Pix generated when processing a Bitmap till the Page is disposed off.

## Version 2 Series

### Version 2.0.0 (planned)

#### Breaking changes from 1.0

* Tesseract.Interop is now internal which means we can make as many interop changes as we like as long as the public version doesn't change
* TesseractEngine.Handle, Pix.Handle, and PixColormap.Handle are now internal
* TesseractEngine now ignores the ``TESSDATAPREFIX`` environment variable if the directory is specified.
* Logging is done to the ``Tesseract`` source, not ``Default``.
* Implemented as a Portable Library, this means the min .net framework is now .NET 4 (TODO)

#### New features

* Support for Tesseract 3.03 and Leptonica 1.7
* PDF output support (TODO)
* Support for multi-page tiffs.

