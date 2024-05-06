module pblsh.test.Parser

open System
open Giraffe
open Xunit
open pblsh.Parser
open FParsec
open Helper

let articleQuery = [ "", true; " ", true; " Test", true; "Test", true; "123", true ]
let articleQueryData = articleQuery |> tupledData

let dotsQuery = [ ".lol", true; "..", false; ".", false; "", true ]
let dotsQueryData = dotsQuery |> tupledData

let dotsArticleQuery = [ ".lol ", true; ".lol Test", true ]
let dotsArticleQueryData = dotsArticleQuery |> tupledData

let authorQuery =
    [ ":", false; "author:", true; " :", false; "12341:", true; "", false ]

let authorQueryData = authorQuery |> tupledData


let authorDotsQuery = combineData authorQuery dotsQuery

let authorDotsQueryData = authorDotsQuery |> tupledData

[<Theory>]
[<MemberData(nameof articleQueryData)>]
let ``pArticle test`` (query, expected) =
    match run Query.pArticle query with
    | ParserResult.Success _ -> Assert.True expected
    | _ -> Assert.False expected

[<Theory>]
[<MemberData(nameof articleQueryData)>]
[<MemberData(nameof dotsQueryData)>]
[<MemberData(nameof dotsArticleQueryData)>]
let ``pDots .>>. pArticle test`` (query, expected) =
    match run (Query.pDots .>>. Query.pArticle) query with
    | ParserResult.Success _ -> Assert.True expected
    | _ -> Assert.False expected

[<Theory>]
[<MemberData(nameof authorQueryData)>]
[<MemberData(nameof dotsQueryData)>]
[<MemberData(nameof authorDotsQueryData)>]
let ``pAuthor .>>. pDots test`` (query, expected) =
    match run (Query.pAuthor .>>. Query.pDots) query with
    | ParserResult.Success _ -> Assert.True expected
    | _ -> Assert.False expected
    
[<Theory>]
[<MemberData(nameof authorQueryData)>]
let ``pAuthor test`` (query, expected) =
    match run Query.pAuthor query with
    | ParserResult.Success _ -> Assert.True expected
    | _ -> Assert.False expected
