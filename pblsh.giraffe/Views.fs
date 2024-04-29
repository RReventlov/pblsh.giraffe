module pblsh.Views

open Giraffe.ViewEngine.Attributes
open Giraffe.ViewEngine.HtmlElements
open pblsh.Components
open pblsh.Models
open pblsh.Models.QueryStrings


let randomPostCard _ =
    div
        [ _class "postcard" ]
        [ h2 [] [ a [ _href "posts/post-id" ] [ encodedText "How to build a medium Medium-Clone" ] ]
          div
              []
              [ encodedText
                    "Follow us on our journey to build a cheap medium clone with a bit of reddit and a dot of patreon" ]
          div
              []
              [ span [] [ encodedText "Written by " ]
                a [ _href "#"; _class "postcard-author" ] [ encodedText "pblsh.dev" ] ]
          div [ _class "postcard-tags" ] [ yield! [ "blog"; "programming" ] |> List.map dot ] ]

let index (userInfo: UserInfo option) =
    let elements = List.ofSeq { 1..10 }

    [ accountTopRow userInfo
      navigation [ { Text = "Home"; Link = "/index" } ]
      main [] [ div [] (elements |> List.map randomPostCard) ] ]
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
            div [] [ encodedText "We send you an email to confirm. Please enter the code below:" ] ] ]
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

let newPost userInfo =
    [ accountTopRow (Some userInfo)
      navigation [ { Text = "Home"; Link = "/index" }; { Text = "Post"; Link = "/post/new" } ]
      main
          []
          [ form
                [ _id "newPost"; _action "post" ]
                [ label [ _for "titleInput" ] [ encodedText "Title" ]
                  input [ _id "titleInput"; _type "textbox" ]
                  label [ _for "dotList" ] [ encodedText "Dots" ]
                  input
                      [ _id "dotInput"
                        _type "textbox"
                        _placeholder "Add dots to categorize your post."
                        _list "dotDataList" ]
                  datalist [ _id "dotDataList" ] [ option [ _value "blog" ] [] ]
                  div
                      [ _id "dotList" ]
                      [ div [ _class "dot" ] [ encodedText "blog"; button [] [ img [ _src "/icons/x.svg" ] ] ] ]
                  label [ _for "docInput" ] [ encodedText "Upload file to publish" ]
                  div
                      []
                      [ encodedText "Supplied files should follow the "
                        a [ _href "https://commonmark.org/"; _class "web-link" ] [ encodedText "CommonMark" ]
                        encodedText " standard to ensure they are rendered correctly." ]
                  input [ _id "docInput"; _type "file"; _accept ".md" ]
                  div
                      [ _id "btn-group" ]
                      [ input [ _type "submit"; _value "Publish post"; _class "filled-action excited" ]
                        input [ _type "reset"; _value "Reset"; _class "warned-action" ] ] ] ] ]
    |> titledLayoutCss [ "post.new.css" ] "New post"
