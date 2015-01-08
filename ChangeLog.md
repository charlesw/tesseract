
### Version 2.1.2

* Improved error message when dll failed to load - [Issue 141](https://github.com/charlesw/tesseract/issues/141)

### Version 2.1.1

* Bug fix - Added null ptr checks to PageIterator and ResultIterator

### Version 2.1.0

* Support for loading config files
* Support for loading Pix from memory

### Version 2.0.0

*Note:* Version 2 was initially going to introduce support for Tesseract 3.03 however as that hasn't been released yet and we have a few minor breaking changes
due to Mono support which require a version incremment (we use semantic versioning). Once the next version of tesseract is released we'll add it.

#### Breaking changes from 1.0

* Tesseract.Interop is now internal which means we can make as many interop changes as we like as long as the public version doesn't change
* TesseractEngine.Handle, Pix.Handle, and PixColormap.Handle are now internal
* Logging is done to the ``Tesseract`` source, not ``Default``.

#### New features

* Support for multi-page tiffs [Issue 50](https://github.com/charlesw/tesseract/issues/50)
* Support for linux\mono [Issue 23](https://github.com/charlesw/tesseract/issues/23)

#### Bug fixes

* Fixed UTF8 handling for SetVariable (support for non-english languages) [Issue 120](https://github.com/charlesw/tesseract/issues/120) & [Issue 68](https://github.com/charlesw/tesseract/issues/68)

### Version 1.12

* Automatically strip '\' and '/' characters of path and remove tessdata prefix.
* Fixed bug introduced in previous region of interest
* Don't dispose of Pix generated when processing a Bitmap till the Page is disposed off.

### Version 1.11

* Allow changing the current region of interest without having to reload the entire image (Page.RegionOfInterest)
* Fixed loader for ASP.NET [Issue 97](https://github.com/charlesw/tesseract/issues/97)


### Version 1.10

* Added support for uzn files - [Issue 66](https://github.com/charlesw/tesseract/issues/66)

