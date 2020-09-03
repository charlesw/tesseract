# Compiling Tesseract

A quick start on how to compile Tesseract. When doing this, you can use the 32 or 64 bit Visual Studio binaries to build all platforms.

## Prerequisites

1. Install SW from https://software-network.org/.

## Build Leptonica

Leptonica needs to be build as a standalong library first otherwise you will get a fully shared library.

1. Build for 32 bit

```sh
sw build -platform=x86 -win-md -configuration=release -shared-build -static-deps -v -show-output -ide-copy-to-dir=. org.sw.demo.danbloomberg.leptonica-1.80.0
```

2. Build for 64 bit

```sh
sw build -platform=x64 -win-md -configuration=release -shared-build -static-deps -v -show-output -ide-copy-to-dir=. org.sw.demo.danbloomberg.leptonica-1.80.0
```

## Build Tesseract

1. Checkout Tesseract from Git

```sh
git clone https://github.com/tesseract-ocr/tesseract.git
```

2. Build for 32 bit

```sh
cmake -G "Visual Studio 16 2019" -A Win32 -S . -B build -DCMAKE_VERBOSE_MAKEFILE=ON -DCMAKE_INSTALL_PREFIX=<install dir> -DSW_BUILD=ON -DSW_EXECUTABLE=<path to SW>\sw.exe
cmake --build build --config Release --target install
```

3. Build for 64 bit

```
cmake -G "Visual Studio 16 2019" -A x64 -S . -B build64 -DCMAKE_VERBOSE_MAKEFILE=ON -DCMAKE_INSTALL_PREFIX=<install dir> -DSW_BUILD=ON -DSW_EXECUTABLE=<path to SW>\sw.exe
cmake --build build64 --config Release --target install
```


