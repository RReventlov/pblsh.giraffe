module pblsh.Views

open Giraffe.ViewEngine.Attributes
open Giraffe.ViewEngine.HtmlElements
open pblsh.Components


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

    [ partial ()
      navigation [ { Text = "Home"; Link = "/index" } ]
      main [] [ div [] [ yield! elements |> List.map randomPostCard ] ] ]
    |> titledLayout "pblsh" [ "index.css" ]

let login () =
    [ emptyPartial ()
      main
          []
          [ div
                [ _id "form-holder" ]
                [ h1 [] [ encodedText "Log in" ]
                  form
                      [ _action "users/login"; _method "post" ]
                      [ label [ _for "email" ] [ encodedText "E-Mail" ]
                        input [ _id "email"; _type "text"; _name "email"; _placeholder "E-Mail" ]
                        label [ _for "password" ] [ encodedText "Password" ]
                        input [ _id "password"; _type "password"; _name "password"; _placeholder "Password" ]
                        input [ _type "submit"; _value "Log in"; _class "filled-action" ] ] ] ] ]
    |> titledLayout "pblsh.login" [ "login.css" ]
