﻿### Version 3.3.0

* Updated to Tesseract 3.05.2
* Updated to Leptonica 1.75.3
* Added support for setting Pix resolution - [Issue 366](https://github.com/charlesw/tesseract/issues/366)

### Version 3.2.0

* Support for .Net Standard 2.0 (.net Core 2) - [Issue 298](https://github.com/charlesw/tesseract/issues/298)
* Removed support for TESSDATA environment variable 
* Added support for the renderer api (generation of PDF, Text, etc) - [Issue 193](https://github.com/charlesw/tesseract/issues/193) 
* Added support for the multiple renderers - [Issue 297](https://github.com/charlesw/tesseract/issues/297) 
* Updated to Tesseract 3.05.1 - [Issue #340](https://github.com/charlesw/tesseract/issues/340)
* Added support Adding pix to, Removing pix from, and clearing PixA - [Issue #340](https://github.com/charlesw/tesseract/issues/340)
* Fixed PolyBlockType definition - [Issue #280](https://github.com/charlesw/tesseract/issues/280)
* Added support for Font attributes to Result Iterator - [Issue #9](https://github.com/charlesw/tesseract/issues/9)

### Version 3.1.0

* Support for printing list of available variables - [Issue 256](https://github.com/charlesw/tesseract/issues/256)
* Support for line removal - [Issue 268](https://github.com/charlesw/tesseract/issues/256)

### Version 3.0.2

* Fixed intermittent crash on initialisation - [Issue 231](https://github.com/charlesw/tesseract/issues/231)
* Upgraded native tesseract libraries to those provided by https://github.com/charlesw/tesseract-vs (Visual Studio 2015)

### Version 3.0.1

* Fixed 64 bit support - [Issue 232](https://github.com/charlesw/tesseract/issues/232)

### Version 3.0.0 (Tesseract 3.04)

#### Breaking Changes

* Requires VS 2013 runtime as the included Tesseract binaries are now compiled with VS 2013.

#### Other Changes

* Update Tesseract binaries to 3.04 - [Issue 168](https://github.com/charlesw/tesseract/issues/168)
* Iterator.GetImage throws ArgumentException: Pix handle must not be zero  - [Issue 206](https://github.com/charlesw/tesseract/issues/206)
* Support for ChoiceIterator - [Issue 129](https://github.com/charlesw/tesseract/issues/129)
* Support for classify_bln_numeric_mode - [Issue 52](https://github.com/charlesw/tesseract/issues/52)
* Support for setting Init only variables - [Issue 70](https://github.com/charlesw/tesseract/issues/70)
* Support for copying\cloning an iterator - [Issue 25](https://github.com/charlesw/tesseract/issues/25)

### Version 2.4.1

* Fixed memory leak in Pix.BinarizeOtsuAdaptiveThreshold, Pix.BinarizeSauvola, and Pix.BinarizeSauvolaTiled - [Issue 218](https://github.com/charlesw/tesseract/issues/218)

### Version 2.4.0

* Support for scaling images - [Issue 183](https://github.com/charlesw/tesseract/issues/183)
* Provide meaningful defaults for Pix.ConvertToGrayscale  - [Issue 184](https://github.com/charlesw/tesseract/issues/184)
* Make TesseractException serialisable - [Issue 194](https://github.com/charlesw/tesseract/issues/194)

### Version 2.3.0

* Support for OsdOnly mode - [Issue 156](https://github.com/charlesw/tesseract/issues/156)
* Support for saving the thresholded image to tessinput.tif when tessedit_write_images is set - [Issue 160](https://github.com/charlesw/tesseract/issues/160)
* Support for specifying custom search directory for dlls and improved logging - [Issue 157](https://github.com/charlesw/tesseract/issues/157)

### Version 2.2.0

* Improved error message when dll failed to load - [Issue 141](https://github.com/charlesw/tesseract/issues/141)
* Changed TesseractEngine's constructors to use overloading rather than default parameters - [Issue 146](https://github.com/charlesw/tesseract/issues/146)
* Added support for Sauvola Binarization.

### Version 2.1.1

* Bug fix - Added null ptr checks to PageIterator and ResultIterator

### Version 2.1.0

* Support for loading config files
* Support for loading Pix from memory

### Version 2.0.0

*Note:* Version 2 was initially going to introduce support for Tesseract 3.03 however as that hasn't been released yet and we have a few minor breaking changes
due to Mono support which require a version increment (we use semantic versioning). Once the next version of tesseract is released we'll add it.

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

