module pblsh.Views

open Giraffe.ViewEngine.Attributes
open Giraffe.ViewEngine.HtmlElements
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
        | Some r -> sprintf "login?ReturnUrl=%s" r.ReturnUrl
        | None -> "login"

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

let signup () =
    [ form
          [ _action "signup"; _method "post"; _class "main-content" ]
          [ h1 [] [ encodedText "Sign up" ]
            label [ _for "email" ] [ encodedText "E-Mail" ]
            input [ _id "email"; _type "text"; _name "email"; _placeholder "E-Mail" ]
            label [ _for "password" ] [ encodedText "Password" ]
            input [ _id "password"; _type "password"; _name "password"; _placeholder "Password" ]
            label [ _for "username" ] [ encodedText "Username" ]
            input [ _id "username"; _type "text"; _name "username"; _placeholder "Username" ]
            input [ _type "submit"; _value "Sign up"; _class "filled-action" ] ]
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

let me userInfo =
    [ accountTopRow (Some userInfo)
      navigation
          [ { Text = "Home"; Link = "/index" }
            { Text = "Account"
              Link = "/account/me" } ]
      main
          []
          [ h1 [] [ encodedText (userInfo.UserName.ToUpper()) ]
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

let post userInfo postInfo content =
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
            div [] [ rawText content ] ] ]
    |> titledLayoutCss [ "pblsh.css"; "post.css" ] (String1.value postInfo.Title)


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
            div [] (results |> List.map postCard)    
            ] ]
    |> titledLayoutCss [ "index.css"; "search.css" ] "Search"
    
let userView (curUserInfo: UserInfo option) (reqUserInfo: UserInfo) =
    [
        accountTopRow curUserInfo
        navigation
          [ { Text = "Home"; Link = "/index" }
            { Text = "Users"; Link = "/users" }
            { Text = reqUserInfo.UserName; Link = "#" }
          ]
        main
          []
          [ h1 [] [ encodedText (reqUserInfo.UserName) ]
          ] 
    ] |> titledLayoutCss ["index.css"] reqUserInfo.UserName
