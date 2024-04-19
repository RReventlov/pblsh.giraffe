module pblsh.Views

open Giraffe.ViewEngine.Attributes
open Giraffe.ViewEngine.HtmlElements
open pblsh.Components
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

let index () =
    let elements = List.ofSeq { 1..10 }

    [ filledTopRow ()
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
            a [ _class "action-link"; _href "/account/signup" ] [ encodedText "Sign up" ] ] ]
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
            a [ _class "action-link"; _href "/account/login" ] [ encodedText "Log in" ] ] ]
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
