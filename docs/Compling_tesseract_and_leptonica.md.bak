# Compling tesseract and leptonica.md
* [Index](./ReadMe.md)

## Notes
Build instructions for Tesseract 4.1.1 and leptonica 1.80.0. Please note that build systems do change so while the following
has been tested with the listed versions building against any other versions including master may not work as expected and
aren't supported.

The following also differ from [[Compiling-Tesseract-and-Leptonica]] in that they use vcpkg to manage the dependencies. 
The main benefit of this is that it's possible to compile tesseract against the leptonica dll rather than statically 
linking leptonica into tesseract which increases file size (since the leptonica dll is still required). 

1. Install Visual Studio 2022 
2. Install CMake (ensure it's on your path)
3. Install [vcpkg](https://github.com/Microsoft/vcpkg/) 
	* Note: I also set an environment variable VCPKG_HOME to this directory and added it to path for convenience
	
4. Build Leptonica:

	```
	vcpkg install giflib:x86-windows-static libjpeg-turbo:x86-windows-static liblzma:x86-windows-static libpng:x86-windows-static tiff:x86-windows-static zlib:x86-windows-static
	vcpkg install giflib:x64-windows-static libjpeg-turbo:x64-windows-static liblzma:x64-windows-static libpng:x64-windows-static tiff:x64-windows-static zlib:x64-windows-static
	git clone https://github.com/DanBloomberg/leptonica.git & cd leptonica	
	git checkout -b 1.82.0 1.82.0
	mkdir vs16-x86 & cd vs16-x86
	cmake .. -G "Visual Studio 17 2022" -A Win32 -DSW_BUILD=OFF -DBUILD_SHARED_LIBS=ON -DCMAKE_TOOLCHAIN_FILE=%VCPKG_HOME%\scripts\buildsystems\vcpkg.cmake -DVCPKG_TARGET_TRIPLET=x86-windows-static -DCMAKE_INSTALL_PREFIX=..\..\build\x86
	cmake --build . --config Release --target install
	cd ..
	mkdir vs16-x64 & cd vs16-x64
	cmake .. -G "Visual Studio 17 2022" -A x64 -DSW_BUILD=OFF -DBUILD_SHARED_LIBS=ON  -DCMAKE_TOOLCHAIN_FILE=%VCPKG_HOME%\scripts\buildsystems\vcpkg.cmake -DVCPKG_TARGET_TRIPLET=x64-windows-static -DCMAKE_INSTALL_PREFIX=..\..\build\x64
	cmake --build . --config Release --target install
	```
4. Build Tesseract:
	
	
	```	
	git clone https://github.com/tesseract-ocr/tesseract.git
	cd tesserct
	git checkout -b 5.2.0 5.2.0
	mkdir vs17-x86 & cd vs17-x86
	cmake .. -G "Visual Studio 17 2022" -A Win32 -DAUTO_OPTIMIZE=OFF -DSW_BUILD=OFF -DBUILD_TRAINING_TOOLS=OFF -DCMAKE_INSTALL_PREFIX=..\..\build\x86
	cmake --build . --config Release --target install
	cd ..
	mkdir vs17-x64 & cd vs17-x64
	cmake .. -G "Visual Studio 17 2022" -A x64   -DAUTO_OPTIMIZE=OFF -DSW_BUILD=OFF -DBUILD_TRAINING_TOOLS=OFF -DCMAKE_INSTALL_PREFIX=..\..\build\x64
	cmake --build . --config Release --target install
	```

### Leptonica Notes:

* Leptonica now needs to be built to use shared libraries (dlls) explicitly, this is accomplished by setting the ``BUILD_SHARED_LIBS`` to ``ON`` (``-DBUILD_SHARED_LIBS=ON``)
* Using [Self build](https://github.com/SoftwareNetwork/sw)  hasn't been tested and is disabled using ``SW_BUILD=OFF``.
	
### Tesseract Notes:

* For portability architecture optimizations have been disabled using ``-DAUTO_OPTIMIZE=OFF`. 
  This however will disable platform specific optimizations (AVX, SSE4.1, etc) which would likely
  result in better performance if your guarantied they will be available.
* Like leptonica Self Build has also been disabled using ``-DSW_BUILD=OFF``.
