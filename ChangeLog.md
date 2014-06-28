### Version 1.10

* Added support for uzn files - [Issue 66](https://github.com/charlesw/tesseract/issues/66)

### Version 1.11

* Allow changing the current region of interest without having to reload the entire image (Page.RegionOfInterest)
* Fixed loader for ASP.NET [Issue 97](https://github.com/charlesw/tesseract/issues/97)

### Version 1.12

* Automatically strip '\' and '/' characters of path and remove tessdata prefix.
* Fixed bug introduced in previous region of interest
* Don't dispose of Pix generated when processing a Bitmap till the Page is disposed off.