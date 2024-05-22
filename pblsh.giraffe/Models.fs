module pblsh.Models

open System
open pblsh.Types
open pblsh.Paths

let parseToGuid (str: string) =
    try
        let value = Guid.Parse str
        Happy(value)
    with
    | :? System.FormatException as e -> Sad e.Message
    | _ -> Sad(sprintf "unknown error parsing %s to guid" str)

module Forms =
    [<CLIMutable>]
    type UncheckedSignUpInfo =
        { Email: string
          Password: string
          UserName: string }

    [<CLIMutable>]
    type LoginInfo = { UserName: string; Password: string }

    [<CLIMutable>]
    type NewPostInfo = { Title: string; Dots: string }
    
    [<CLIMutable>]
    type NewComment = {
        Content:string;
        Parent:Guid
    }
    
    [<CLIMutable>]
    type SearchContent = { Query: string }
    
    [<CLIMutable>]
    type QueryInfo =
        { Author: string option
          Dots: string list
          Article: string }

module QueryStrings =

    [<CLIMutable>]
    type RedirectInfo = { ReturnUrl: string }


type UserInfo =
    {
        UserName: string
        Id: Guid
    }

type Dot = String1

type Post =
    { Id: Guid
      Author: string
      Title: String5
      Dots: Dot list
      PublishedOn: DateTime
      UniqueViews: Int0 }

module Post =
    type PostErrors =
        | TitleTooShort
        | DotTooShort of string list

    let create id author title dots publishedOn uniqueViews =
        path {
            let! title5 = (String5.create >> mapR) title <|? (fun _ -> TitleTooShort)
            and! dDots = dots |> List.map (String1.create >> mapR) |> collect <|? DotTooShort

            return
                { Id = id
                  Author = author
                  Title = title5
                  Dots = dDots
                  PublishedOn = publishedOn
                  UniqueViews = uniqueViews }
        }

    let createNew author title dots =
        create (Guid.NewGuid()) author title dots DateTime.Now Int0.zero

type PostInformation =
    { Id: Guid
      Author: String1
      AuthorId: Guid
      Title: String1
      Dots: Dot list }
    
type CommentInformation =
    {
        Id: Guid
        Author: String1
        AuthorId: Guid
        Content: String1
        Parent: Guid
        PostId: Guid
        Replies: CommentInformation list
    }    
    
module CommentInformation =
    
    type CommentInformationError =
        | AuthorTooShort of string
        | UnknownIdFormat of string
        | ContentTooShort of string
        | RepliesTooShort of string list
        
    
    let create uncheckedId uncheckedAuthor uncheckedAuthorId uncheckedContent uncheckedParentId uncheckedPostId (*uncheckedReplies*) =
        path {
            let! id = parseToGuid uncheckedId <|? UnknownIdFormat
            let! author = String1.create uncheckedAuthor |> mapR <|? AuthorTooShort
            let! authorId = parseToGuid uncheckedAuthorId <|? UnknownIdFormat
            let! content = String1.create uncheckedContent |> mapR <|? ContentTooShort
            let! parentId = parseToGuid uncheckedParentId <|? UnknownIdFormat
            let! postId = parseToGuid uncheckedPostId <|? UnknownIdFormat
            //let! replies = uncheckedReplies |> List.map (CommentInformation.create >> mapR) |> collect <|? RepliesTooShort
            
            return
                {
                    Id = id
                    Author = author 
                    AuthorId = authorId
                    Content = content
                    Parent = parentId
                    PostId = postId
                    Replies = []
                }
        }

module PostInformation =

    type PostInformationError =
        | UnknownIdFormat of string
        | AuthorNameTooShort of string
        | TitleTooShort of string
        | DotTooShort of string list

    let create uncheckedId uncheckedAuthor uncheckedAuthorId uncheckedTitle dotNames =
        path {
            let! id = parseToGuid uncheckedId <|? UnknownIdFormat
            let! author = String1.create uncheckedAuthor |> mapR <|? AuthorNameTooShort
            let! authorId = parseToGuid uncheckedAuthorId <|? UnknownIdFormat
            let! title = String1.create uncheckedTitle |> mapR <|? TitleTooShort
            let! dots = dotNames |> List.map (String1.create >> mapR) |> collect <|? DotTooShort

            return
                { Id = id
                  Author = author
                  AuthorId = authorId
                  Title = title
                  Dots = dots }
        }
