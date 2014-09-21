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
* Logging is done to the ``Tesseract`` source, not ``Default``.

#### New features

* Support for multi-page tiffs.
