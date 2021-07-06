# Examples
* [Index](./ReadMe.md)
* Most of these are taken from the [tesseract-samples](https://github.com/charlesw/tesseract-samples) project

## Notes
* you need trained data in `tessdata` by language
  * You can get them at https://github.com/tesseract-ocr/tessdata or https://github.com/tesseract-ocr/tessdata_fast

## Basic Text from Image from filepath
* from [Tesseract.ConsoleDemo/Program.cs](https://github.com/charlesw/tesseract-samples/blob/master/src/Tesseract.ConsoleDemo/Program.cs)
```cs
using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default))
    {
        using (var img = Pix.LoadFromFile(testImagePath))
        {
            using (var page = engine.Process(img))
            {
                var text = page.GetText();
                Console.WriteLine("Mean confidence: {0}", page.GetMeanConfidence());

                Console.WriteLine("Text (GetText): \r\n{0}", text);
                Console.WriteLine("Text (iterator):");
                }
        }
    }
```

## Basic Text from Image bytes
```cs
FileStream fs = new FileStream(filename, FileMode.Open, file_access);
var ms = new MemoryStream();
fs.CopyTo(ms);
fs.Close();
bytes[] fileBytes = ms.ToArray();
ms.Close();
using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default))
     {
        using (var img = Pix.LoadFromMemory(fileBytes))
              {
              using (var page = engine.Process(img))
                    {
                        var txt = page.GetText();
                    }
              }
      }
```

## Image to txt searchable pdf using paths
```cs
using (IResultRenderer renderer = Tesseract.PdfResultRenderer.CreatePdfRenderer(@"test.pdf", @"./tessdata", false))
    {
        // PDF Title
        using (renderer.BeginDocument("Serachablepdftest"))
        {
            string configurationFilePath = @"C:\tessdata";
            using (TesseractEngine engine = new TesseractEngine(configurationFilePath, "eng", EngineMode.TesseractAndLstm))
            {
                using (var img = Pix.LoadFromFile(@"C:\file-page1.jpg"))
                {
                    using (var page = engine.Process(img, "Serachablepdftest"))
                    {
                        renderer.AddPage(page);
                    }
                }
            }
        }
    }
```

## Image to pdf returning file bytes
```cs
    var tmpPdfLocation = "./tessdata/pdf";
    var sep = Path.PathSeparator;
    var tmpFile = tmpPdfLocation + sep + Path.GetTempFileName();
    bytes[] fileBytes = null;
    using (IResultRenderer renderer = Tesseract.PdfResultRenderer.CreatePdfRenderer(tmpFile, @"./tessdata", false))
    {
        // PDF Title
        using (renderer.BeginDocument("Serachablepdftest"))
        {
            // string configurationFilePath = @"C:\tessdata";
            using (TesseractEngine engine2 = new TesseractEngine(configurationFilePath, "eng", EngineMode.TesseractAndLstm))
            {
                using (var img = Pix.LoadFromFile(@"C:\file-page1.jpg"))
                {
                    using (var page = engine.Process(img, "Searchablepdftest"))
                    {
                        renderer.AddPage(page);
                    }
                }
            }
           
        }

    }
    // on dispose file should be created
    var stream = new FileStream(tmpFile, FileMode.Open, FileAccess.Read);
    MemoryStream ms = new MemoryStream();
    stream.CopyTo(ms);
    fileBytes = ms.ToArray();
    stream.Dispose();
    ms.Close();
    // delete tmp file
    File.Delete(tmpFile);
```
