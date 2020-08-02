The following differ from [[Compiling-Tesseract-and-Leptonica]] in that they use vcpkg to manage the dependencies. The main benefit of this is that it's possible to compile tesseract against the leptonica dll rather than statically linking leptonica into tesseract which increases file size (since the leptonica dll is still required). 


1. Install Visual Studio 2019 
2. Install CMake (ensure it's on your path)
3. Install [vcpkg](https://github.com/Microsoft/vcpkg/) 
	* Note: I also set an environment variable VCPKG_HOME to this directory and added it to path for convenience
	
4. Build Leptonica:
	```
	vcpkg install giflib:x86-windows-static icu:x86-windows-static libjpeg-turbo:x86-windows-static liblzma:x86-windows-static libpng:x86-windows-static tiff:x86-windows-static zlib:x86-windows-static
	vcpkg install giflib:x64-windows-static icu:x64-windows-static libjpeg-turbo:x64-windows-static liblzma:x64-windows-static libpng:x64-windows-static tiff:x64-windows-static zlib:x64-windows-static
	git clone https://github.com/DanBloomberg/leptonica.git & cd leptonica	
	git checkout -b 1.78.0 1.78.0
	mkdir vs16-x86 & cd vs16-x86
	cmake .. -G "Visual Studio 16 2019" -A Win32 -DCMAKE_TOOLCHAIN_FILE=%VCPKG_HOME%\scripts\buildsystems\vcpkg.cmake -DVCPKG_TARGET_TRIPLET=x86-windows-static -DCMAKE_INSTALL_PREFIX=..\..\build\x86
	cmake --build . --config Release --target install
	cd ..
	mkdir vs16-x64 & cd vs16-x64
	cmake .. -G "Visual Studio 16 2019" -A x64 -DCMAKE_TOOLCHAIN_FILE=%VCPKG_HOME%\scripts\buildsystems\vcpkg.cmake -DVCPKG_TARGET_TRIPLET=x64-windows-static -DCMAKE_INSTALL_PREFIX=..\..\build\x64
	cmake --build . --config Release --target install
	```
4. Build Tesseract:
	
	Notes: 	
	* Building training tools also requires ICU, as this isn't directory required for .net wrapper around tesseract this has been disabled.
	* May need to disable AVX optimisations if you want the library to run on machines without these available, see Appendix below
	
	```	
	git clone https://github.com/tesseract-ocr/tesseract.git
	cd tesserct
	git checkout -b 4.1.0 4.1.0
	mkdir vs16-x86 & cd vs16-x86
	cmake .. -G "Visual Studio 16 2019" -A Win32 -DBUILD_TRAINING_TOOLS=OFF -DCMAKE_INSTALL_PREFIX=..\..\build\x86
	cmake --build . --config Release --target install
	cd ..
	mkdir vs16-x64 & cd vs16-x64
	cmake .. -G "Visual Studio 16 2019" -A x64  -DBUILD_TRAINING_TOOLS=OFF -DCMAKE_INSTALL_PREFIX=..\..\build\x64
	cmake --build . --config Release --target install
	```

### Building leptonica (master) and tesseract (4.1 branch)
In order to build the current master branch of leptonica, as of 2019-11-27, "SW_BUILD" must be disabled as it's not available on windows. This can be accomplished by adding `-DSW_BUILD=OFF` to the cmake command line above.
The same is valid for building tesseract, the 4.1 branch.
By default the cmake is setup to use the optimization macros, but it is resulting of failing tesseract on some old processors, and therefore the option `-DAUTO_OPTIMIZE=OFF` should be added also when compiling tesseract.

See: [Issue 497 - comment A](https://github.com/charlesw/tesseract/issues/497#issuecomment-559010640)
[Issue 497 - comment B](https://github.com/charlesw/tesseract/issues/497#issuecomment-560134125)

### Disabling AVX optimisations

Tesseract will automatically detect and enable the instruction set extensions supported on the machine 
used to compile it. While the resulting binaries will of course work just fine on your machine, or any 
that support said instruction sets, they will crash on machines that do not. Therefore if your distributing 
your application you will likely need to change the targeted instruction set extensions for the generated 
libtesseract and tesseract projects in visual studio. 

For reference the precompiled dlls distributed with the wrapper where compiled using "Not Set" (SSE2). 

1. Open the generated project e.g. ~\teseract\vs16-x64\tesseract.sln
2. For both libtesseract and tesseract projects:
	1. Open project properties > Configuration Properties > C/C++ > Code Generation
	2. Change the build configuration to "All Configurations"
	3. Change "Enable Enhanced Instruction Set" to either "Not Set" or "SSE2"
3. Build the "INSTALL" CMake target or execute build command from console.

For more information see: https://docs.microsoft.com/en-us/cpp/build/reference/arch-x64?view=vs-2019#to-set-the-archavx-archavx2-or-archavx512-compiler-option-in-visual-studio