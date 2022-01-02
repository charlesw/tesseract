To get .net wrapper for Tesseract working on macOS tesseract and leptonica libraries need to installed and linked in project.

# Install libraries via Brew
```sh
brew install leptonica tesseract
```

# Add symlinks to output folder
Add the following Target to the csproj.

```xml
<Target Name="link_deps" AfterTargets="AfterBuild">
  <Exec Command="ln -sf /usr/local/lib/liblept.dylib $(OutDir)x64/libleptonica-1.80.0.dylib"/>
  <Exec Command="ln -sf /usr/local/lib/libtesseract.dylib $(OutDir)x64/libtesseract41.dylib"/>
</Target>
```