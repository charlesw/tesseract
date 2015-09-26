module Program

open System
open System.Diagnostics
open Tesseract

type Logger =
  inherit IDisposable
  abstract Log : string -> unit
  abstract Begin : string -> Logger

type FormattedConsoleLogger(level : int, name : string) =
  let delta = 2

  let log level msg =
    printfn "%s%s" (new String(' ', level)) msg

  interface Logger with
    member x.Log msg =
      log level msg

    member x.Begin name =
      log level (sprintf "######## START '%s' ########" name)
      upcast new FormattedConsoleLogger(level + delta, name)

  interface IDisposable with
    member x.Dispose() =
      log (level - delta) (sprintf "######### END '%s' ########" name)

  new(name) = new FormattedConsoleLogger(0, name)

type Content =
  | Block
  | Para
  | TextLine
  | Symbol
  | Word

  member x.iteratorLevel =
    match x with
    | Block -> PageIteratorLevel.Block
    | Para -> PageIteratorLevel.Para
    | TextLine -> PageIteratorLevel.TextLine
    | Symbol -> PageIteratorLevel.Symbol
    | Word -> PageIteratorLevel.Word

let parentOf = function
  | Block -> Block
  | Para -> Block
  | TextLine -> Para
  | Symbol -> TextLine
  | Word -> TextLine

let xOf x (iter : PageIterator) =
  List.fold (fun s -> function
    | Block -> if x iter Block then Block :: s else s
    | Para -> if x iter Para then Para :: s else s
    | TextLine -> if x iter TextLine then TextLine :: s else s
    | Symbol -> if x iter Symbol then Symbol :: s else s
    | Word -> if x iter Word then Word :: s else s)
    []
    [ Block; Para; TextLine; Symbol; Word ]

let beginningOf (iter : PageIterator) =
  xOf (fun iter element -> iter.IsAtBeginningOf element.iteratorLevel) iter

let finalOf (iter : PageIterator) =
  xOf (fun iter element -> iter.IsAtFinalOf((parentOf element).iteratorLevel, element.iteratorLevel)) iter

let (|BeginningOf|_|) lookFor (iter : PageIterator) =
  let l = beginningOf iter
  l
  |> List.tryFind ((=) lookFor)
  |> Option.map (fun _ -> l)

let (|FinalOf|_|) (lookFor : Content) (iter : PageIterator) =
  let l = finalOf iter
  l
  |> List.tryFind ((=) lookFor)
  |> Option.map (fun _ -> l)

module ResultPrinter =
  let print (logger : Logger) (iter : ResultIterator) =
    let beginning xs = logger.Log (sprintf "beginningOf %A" xs)
    let final xs = logger.Log (sprintf "finalOf %A" xs)

    match iter with
    | BeginningOf Block others -> beginning others
    | BeginningOf Para others -> beginning others
    | BeginningOf TextLine others -> beginning others
    | BeginningOf Symbol others -> beginning others
    | BeginningOf Word others -> beginning others
    | otherwise -> logger.Log "beginningOf Nothing!"

    match iter with
    | FinalOf Block others -> final others
    | FinalOf Para others -> final others
    | FinalOf TextLine others -> final others
    | FinalOf Symbol others -> final others
    | FinalOf Word others -> final others
    | otherwise -> logger.Log "finalOf Nothing!"

    //logger.Log (sprintf "Block text: \"%s\"" (iter.GetText(PageIteratorLevel.Block)))
    //logger.Log (sprintf "Para text: \"%s\"" (iter.GetText(PageIteratorLevel.Para)))
    //logger.Log (sprintf "TextLine text: \"%s\"" (iter.GetText(PageIteratorLevel.TextLine)))
    logger.Log (sprintf "Word text: \"%s\"" (iter.GetText(PageIteratorLevel.Word)))
    //logger.Log (sprintf "Symbol text: \"%s\"" (iter.GetText(PageIteratorLevel.Symbol)))

let print (logger : Logger) (iter : ResultIterator) =

  let rec wordIterator (logger : Logger) i =

    logger.Log (sprintf "word iteration # %i – PolyBlockType: %A" i iter.BlockType)
    ResultPrinter.print logger iter

    if iter.Next(PageIteratorLevel.TextLine, PageIteratorLevel.Word) then
      wordIterator logger (i + 1)
    else
      logger.Log "Next(TextLine, Word) => false"
      // this prints the same as the above (even after next)
      //ResultPrinter.print wi iter
      
  let rec textLineIterator (logger : Logger) i =
    logger.Log (sprintf "TextLine: %s" (iter.GetText(PageIteratorLevel.TextLine)))

    let wi = logger.Begin "Word Iteration"
    wordIterator wi i
    wi.Dispose()

    if iter.Next(PageIteratorLevel.Para, PageIteratorLevel.TextLine) then
      textLineIterator logger (i + 1)
    else
      logger.Log "Next(Para, TextLine) => false"

  let rec blockIterator (logger : Logger) i =
    //ResultPrinter.print bi iter
    let pi = logger.Begin (sprintf "Paragraph Iteration @ %i – PolyBlockType: %A" 0 iter.BlockType)
    textLineIterator pi i
    pi.Dispose()

    if iter.Next(PageIteratorLevel.Block) then
      blockIterator logger (i + 1)
    else
      logger.Log "Next(Block) => false"
      ResultPrinter.print logger iter

  use bi = logger.Begin (sprintf "Block Iteration – PolyBlockType: %A" iter.BlockType)
  blockIterator logger 0

[<EntryPoint>]
let main argv =
  if argv.Length < 2 then
    Console.Error.WriteLine("Usage: playground <lang> <pix>")
    2
  else
    let lang, file = argv.[0], argv.[1]

    let logger = new FormattedConsoleLogger("Main") :> Logger
    use engine = new TesseractEngine(@"/usr/local/share/tessdata", lang, EngineMode.Default)
    use img = Pix.LoadFromFile(file)
    use pi = logger.Begin("Process image")

    try
      use page = engine.Process(img)
      //let text = page.GetText()
      //logger.Log (sprintf "Text: %s" text)
      pi.Log (sprintf "Mean confidence: %f" (page.GetMeanConfidence()))
      use iter = page.GetIterator()
      iter.Begin()
      print pi iter
      0

    with e ->
      Trace.TraceError(e.ToString())
      Console.Error.WriteLine("Unexpected Error: " + e.Message)
      Console.Error.WriteLine("Details: ")
      Console.Error.WriteLine(e)
      Console.Error.Write("Press any key to continue . . . ")
      Console.ReadKey(true) |> ignore
      1