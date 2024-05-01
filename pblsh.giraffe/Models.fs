﻿module pblsh.Models

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

open Forms

module QueryStrings =

    [<CLIMutable>]
    type RedirectInfo = { ReturnUrl: string }


type UserInfo = { UserName: string }

type Post =
    { Id: Guid
      Author: Guid
      Title: String5
      Dots: string list
      PublishedOn: DateTime
      UniqueViews: Int0 }

module Post =
    type PostErrors = | TitleTooShort

    let create id author title dots publishedOn uniqueViews =
        mapResult (String5.create title)
        |>^ (fun title5 ->
            { Id = id
              Author = author
              Title = title5
              Dots = dots
              PublishedOn = publishedOn
              UniqueViews = uniqueViews })

    let createNew author title dots =
        create (Guid.NewGuid()) author title dots DateTime.Now (Int0.zero)
