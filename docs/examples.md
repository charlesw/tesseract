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

## Image to txt searchable pdf
* (pending)
