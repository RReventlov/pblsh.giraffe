module pblsh.Views

open System.Collections
open Giraffe.ViewEngine.Attributes
open Giraffe.ViewEngine.HtmlElements
open Microsoft.AspNetCore.Identity
open pblsh.Components
open pblsh.Models
open pblsh.Models.QueryStrings
open pblsh.Types

let index (userInfo: UserInfo option) (posts: PostInformation list) =
    [ accountTopRow userInfo
      navigation [ { Text = "Home"; Link = "/index" } ]
      main [] [ div [] (posts |> List.map postCard) ] ]
    |> titledLayoutCss [ "index.css" ] "pblsh"

let login (redirectAfterLogin: RedirectInfo option) =
    let actionUrl =
        match redirectAfterLogin with
        | Some r -> Urls.loginWithRedirect r.ReturnUrl
        | None -> "/account/login"

    [ form
          [ _action actionUrl; _method "post"; _class "main-content" ]
          [ h1 [] [ encodedText "Log in" ]
            label [ _for "username" ] [ encodedText "Username" ]
            input [ _id "username"; _type "text"; _name "username"; _placeholder "Username" ]
            label [ _for "password" ] [ encodedText "Password" ]
            input [ _id "password"; _type "password"; _name "password"; _placeholder "Password" ]
            input [ _type "submit"; _value "Log in"; _class "filled-action" ] ]
      div
          []
          [ span [] [ encodedText "Don't have an account? " ]
            a [ _class "web-link"; _href "/account/signup" ] [ encodedText "Sign up" ] ] ]
    |> dialogCss [ "login.css" ] "pblsh.login"

let signup (errors:string List) =
    [ form
          [ _action "signup"; _method "post"; _class "main-content" ]
          [ h1 [] [ encodedText "Sign up" ]
            label [ _for "email" ] [ encodedText "E-Mail" ]
            input [ _id "email"; _type "text"; _name "email"; _placeholder "E-Mail" ]
            label [ _for "password" ] [ encodedText "Password" ]
            input [ _id "password"; _type "password"; _name "password"; _placeholder "Password" ]
            label [ _for "username" ] [ encodedText "Username" ]
            input [ _id "username"; _type "text"; _name "username"; _placeholder "Username" ]
            input [ _type "submit"; _value "Sign up"; _class "filled-action" ]
            for e in errors do
                p [_class "Error"] [encodedText e]
          ]
      div
          []
          [ span [] [ encodedText "Already signed up? " ]
            a [ _class "web-link"; _href "/account/login" ] [ encodedText "Log in" ] ] ]
    |> dialog "pblsh.signup"

let errorWithRedirect (link: string) =
    [ div
          [ _id "frame" ]
          [ img [ _src "/images/error.jpg" ]
            h1 [] [ encodedText "You summoned an octopus!" ]
            div [ _id "bottom-text" ] [ encodedText "An error occurred. Please try again another time." ] ] ]
    |> dialogCss [ "error.css" ] "pblsh.error"

let signUpComplete () =
    [ div
          [ _class "main-content" ]
          [ h1 [] [ encodedText "Thanks for signing up!" ]
            // div [] [ encodedText "We send you an email to confirm. Please enter the code below:" ]
            ] ]
    |> dialog "pblsh.confirm"

let me (userInfo: UserInfo) (articles:PostInformation list) =
    [ accountTopRow (Some userInfo)
      navigation
          [ { Text = "Home"; Link = "/index" }
            { Text = "Account"
              Link = "/account/me" } ]
      main
          []
          [ h1 [] [ encodedText (userInfo.UserName.ToUpper()) ]
            div [ _class "writtenArticles"] [
                h1 [] [encodedText "My Articles"]
                div [] (articles |> List.map postCard)
            ]
            a [ _class "action"; _href "/account/logout" ] [ encodedText "Log out" ] ] ]
    |> titledLayout "pblsh.account"

let newPost userInfo (errors: string list) =
    [ accountTopRow (Some userInfo)
      navigation
          [ { Text = "Home"; Link = "/index" }
            { Text = "New Post"
              Link = Urls.newPost } ]
      main
          []
          [ div [] (errors |> List.map (fun e -> p [ _class "error" ] [ encodedText e ]))
            form
                [ _action "new"; _method "post"; _enctype "multipart/form-data" ]
                [ label [ _for "title" ] [ encodedText "Title" ]
                  input [ _id "title"; _type "text"; _name "title" ]
                  label [ _for "dots" ] [ encodedText "Dots" ]
                  input
                      [ _id "dots"
                        _type "text"
                        _name "dots"
                        _placeholder "Adding dots helps readers to find your post. Separate dots with a '.'."
                        _list "dotDataList" ]
                  datalist [ _id "dotDataList" ] [ option [ _value "blog" ] [] ]
                  label [ _for "file" ] [ encodedText "Upload file to publish" ]
                  div
                      []
                      [ encodedText "Supplied files should follow the "
                        a
                            [ _href "https://commonmark.org/"; _target "_blank"; _class "web-link" ]
                            [ encodedText "CommonMark" ]
                        encodedText " standard to ensure they are rendered correctly." ]
                  input [ _id "file"; _type "file"; _name "file"; _accept ".md" ]
                  div
                      [ _id "btn-group" ]
                      [ input [ _type "submit"; _value "Publish post"; _class "filled-action excited" ]
                        input [ _type "reset"; _value "Reset"; _class "warned-action" ] ] ] ] ]
    |> titledLayoutCss [ "post.new.css" ] "New post"

let post (userInfo: UserInfo option) (postInfo: PostInformation) content (comments: 'a list) =
    let replyButton =
        match userInfo with
        | None ->
            a
                [ _type "button"
                  _href (Urls.loginWithRedirect (sprintf "/posts/%O" postInfo.Id))
                  _class "comment-new-login filled-action" ]
        | Some _ -> button [ _type "button"; _class "comment-new-open comment-open-reply-box filled-action" ]

    [ accountTopRow userInfo
      navigation
          [ { Text = "Home"; Link = "/index" }
            { Text = String1.value postInfo.Author
              Link = Urls.userUrl postInfo }
            { Text = String1.value postInfo.Title
              Link = Urls.postUrl postInfo } ]
      main
          []
          [ h1 [ _id "title" ] [ encodedText (String1.value postInfo.Title) ]
            div
                [ _id "author" ]
                [ encodedText "Written by "
                  a [ _id "author-url"; _href (Urls.userUrl postInfo) ] [ encodedText (String1.value postInfo.Author) ] ]
            div [ _id "dot-list" ] (postInfo.Dots |> List.map dot)
            div [] [ rawText content ]
            div
                [ _class "comments" ]
                [ div
                      [ _class "comment-information" ]
                      [ h3 [] [ encodedText "Comment Section" ]
                        p [] [ encodedText (sprintf "%d Results found" comments.Length) ]
                        p
                            [ _class "comment-new-container" ]
                            [ replyButton [ encodedText "Reply"; img [ _src "/icons/chat-dots.svg" ] ] ]
                        match userInfo with
                        | None -> div [] []
                        | Some _ -> replyContainer (sprintf "/posts/%O/comments/%O" postInfo.Id System.Guid.Empty) ]

                  div
                      []
                      (comments
                       |> List.map (fun commentInformation -> commentCard commentInformation userInfo)) ] ] ]
    |> titledLayoutCssJs [ "pblsh.css"; "post.css" ] [ "post.js" ] (String1.value postInfo.Title)


let search (userInfo: UserInfo option) (query: string) (results: PostInformation list) =
    [ accountTopRow userInfo
      navigation
          [ { Text = "Home"; Link = "/index" }
            { Text = "Search"; Link = "/search" }
            { Text = (sprintf "\"%s\"" query)
              Link = "#" } ]
      main
          []
          [ div
                [ _class "resultMetaDisplay" ]
                [ h1
                      [ _class "queryDisplay" ]
                      [ span
                            []
                            [ encodedText "Search Results for \""
                              span [ _class "query" ] [ encodedText query ]
                              encodedText "\"" ] ]
                  div [ _class "countDisplay" ] [ encodedText (sprintf "%d Results found" results.Length) ] ]
            div [] (results |> List.map postCard) ] ]
    |> titledLayoutCss [ "index.css"; "search.css" ] "Search"

let userView (curUserInfo: UserInfo option) (reqUserInfo: UserInfo) (articles: PostInformation list) =
    [ accountTopRow curUserInfo
      navigation
          [ { Text = "Home"; Link = "/index" }
            { Text = "Users"; Link = "/users" }
            { Text = reqUserInfo.UserName
              Link = "#" } ]
      main
          []
          [ h1 [] [ encodedText (reqUserInfo.UserName) ]
            div
                [ _class "writtenArticles" ]
                [ h1 [] [ encodedText "Articles written by User" ]
                  div [] (articles |> List.map postCard) ] ] ]
    |> titledLayoutCss [ "index.css" ] reqUserInfo.UserName

let dotView (userInfo: UserInfo option) (dot: string) (results: PostInformation list) =
    [ accountTopRow userInfo
      navigation
          [ { Text = "Home"; Link = "/index" }
            { Text = "Search"; Link = "/search" }
            { Text = (sprintf "\"%s\"" dot)
              Link = "#" } ]
      main
          []
          [ div
                [ _class "resultMetaDisplay" ]
                [ h1
                      [ _class "queryDisplay" ]
                      [ span
                            []
                            [ encodedText "Results for \""
                              span [ _class "query" ] [ encodedText dot ]
                              encodedText "\"" ] ]
                  div [ _class "countDisplay" ] [ encodedText (sprintf "%d Results found" results.Length) ] ]
            div [] (results |> List.map postCard) ] ]
    |> titledLayoutCss [ "index.css"; "search.css" ] dot
