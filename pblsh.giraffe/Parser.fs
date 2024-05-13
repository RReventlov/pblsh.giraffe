module pblsh.Parser

open FParsec.CharParsers
open Models.Forms
open FParsec.Primitives


module Query =


    type QueryInfo =
        { Author: string option
          Dots: string list
          Article: string }

    let resultAsQueryInfo author dots article =
        { Author = author
          Dots = dots
          Article = article }


    let smallLetters = [ 'a' .. 'z' ]
    let letters = [ 'A' .. 'Z' ] @ smallLetters
    let toString x = x |> Array.ofList |> System.String

    let pAuthor =
        (many1 (anyOf letters) |>> toString .>> pchar ':')
        |> opt

    let pDots = many (pchar '.' >>. many1 (anyOf smallLetters) |>> toString)

    let pArticle = spaces >>. restOfLine true

    let queryParser = pAuthor .>>. pDots .>>. pArticle

    let parse (content: SearchContent) =
        let result = run queryParser content.Query

        match result with
        | ParserResult.Success(((author, dots), article), _, _) -> Result.Ok(resultAsQueryInfo author dots article)
        | _ -> Result.Error "parsing failed"

let test () =
    let query = " :"

    match run (many1 (anyOf Query.letters) |>> Query.toString .>> pchar ':' |> many) query with
    | ParserResult.Success(x, y, z) -> printfn "success \n%A \n%A \n%A" x y z
    | ParserResult.Failure(x, y, z) -> printfn "failure \n%A \n%A \n%A" x y z

    ()