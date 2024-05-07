module pblsh.Models

open System
open pblsh.Types
open pblsh.Paths


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

module QueryStrings =

    [<CLIMutable>]
    type RedirectInfo = { ReturnUrl: string }


type UserInfo = { UserName: string }

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
