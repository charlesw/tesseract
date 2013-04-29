A .NET wrapper for [tesseract-ocr](http://code.google.com/p/tesseract-ocr/).

Code License: [Apache License 2.0](http://www.apache.org/licenses/LICENSE-2.0)  
Site Content License (Documentation etc): [Creative Commons Attribution 3.0 Unported License](license" href="http://creativecommons.org/licenses/by/3.0/)

## Warning - Prerelease software (alpha)

This is currently prerelease software as such the public API is subject to change.

## Request for comment

I've been considering splitting Tesseract and Leptonica into two seperate projects\dlls, please comment on [Issue #22](https://github.com/charlesw/tesseract/issues/22).

## Getting started quickly

Note: Compiling the project requires at least MS Visual Studio 11 Express for Desktop or SharpDevelop 4.2.

1. Fork this project (see: https://help.github.com/articles/fork-a-repo)
2. Download language data files for tesseract 3.02 from http://code.google.com/p/tesseract-ocr/
3. Build BaseApiTester project
4. Copy language files into ``BaseApiTester\bin\[config]\tessdata``
5. Run BaseApiTester for example (work in progress)

## To-do

Please help yourselves to one of the following tasks (or create a new one). Please leave a comment in the corresponding task to
avoid duplication of work.

Task Num	| Task													| Status
--------|---------------------------------------------------------------|------------------------------------------
	| Tesseract - Core Apis 					| Active
	| Tesseract - Regression test infrastructure			| Pending
	| Leptonica - Scanned image preparation functions		| Pending
	| Tesseract - API Cleanup and improvements			| Pending
	| Tesseract - Create sample app					| Pending
	| Tesseract - Dictionary lookup [not sure about this]		| Pending
