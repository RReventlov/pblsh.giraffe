module pblsh.Components

open Giraffe.ViewEngine.Attributes
open Giraffe.ViewEngine.HtmlElements
open pblsh.Models

let titledLayoutCssJs (cssFiles: string list) (jsFiles: string list) (pageTitle: string) (content: XmlNode list) =
    html
        []
        [ head
              []
              [ title [] [ encodedText pageTitle ]
                link [ _rel "stylesheet"; _type "text/css"; _href "/css/pblsh.reboot.css" ]
                link [ _rel "stylesheet"; _type "text/css"; _href "/css/pblsh.animations.css" ]
                link [ _rel "stylesheet"; _type "text/css"; _href "/css/pblsh.css" ]
                yield!
                    cssFiles
                    |> List.map (fun f -> link [ _rel "stylesheet"; _type "text/css"; _href (sprintf "/css/%s" f) ]) ]
          body [] content
          yield!
              jsFiles
              |> List.map (fun f -> script [ _type "text/javascript"; _src (sprintf "/js/%s" f) ] []) ]

let titledLayout = titledLayoutCssJs [] []

let titledLayoutCss (cssFiles: string list) = titledLayoutCssJs cssFiles []

let titledLayoutJs (jsFiles: string list) = titledLayoutCssJs [] jsFiles

let layout (content: XmlNode list) = titledLayout "pblsh" content

let topRow (middle: XmlNode list) (right: XmlNode list) =
    header
        []
        [ div [ _id "logo" ] [ a [ _href "/index" ] [ encodedText "pblsh" ] ]
          div [ _id "header-middle" ] middle
          div [ _id "header-right" ] right ]

let accountTopRow (userInfo: UserInfo option) =
    topRow
        [ input [ _id "search"; _type "input"; _placeholder "Search..." ] ]
        [ match userInfo with
          | Some u ->
              a [ _id "newpost"; _class "action"; _href Urls.newPost ] [ encodedText "New post" ]
              a [ _id "account"; _class "filled-action"; _href "/account/me" ] [ encodedText u.UserName ]      
          | None ->
              a [ _id "login"; _class "action"; _href "/account/login" ] [ encodedText "Log in" ]
              a [ _id "signup"; _class "filled-action"; _href "/account/signup" ] [ encodedText "Sign up" ] ]

let emptyTopRow () = topRow [] []

type RoutePart = { Text: string; Link: string }

let private routeElement (routePart: RoutePart) =
    div
        [ _class "route-part" ]
        [ a [ _href routePart.Link ] [ encodedText routePart.Text ]
          img [ _src "/icons/chevron.svg"; _height "14" ] ]

let navigation (route: RoutePart list) =
    div [ _id "navigation" ] (route |> List.map routeElement)

let dot str =
    a [ _class "tag"; _href (sprintf "/dots/%s" str) ] [ encodedText (sprintf ".%s" str) ]

let dialogCssJs (cssFiles: string list) (jsFiles: string list) (pageTitle: string) (content: XmlNode list) =
    [ emptyTopRow (); main [ _class "shiny" ] [ div [ _id "center" ] content ] ]
    |> titledLayoutCssJs ("dialog.css" :: cssFiles) ("mousetracker.js" :: jsFiles) pageTitle

let dialogCss cssFiles = dialogCssJs cssFiles []

let dialogJs jsFiles = dialogCssJs [] jsFiles

let dialog = dialogCssJs [] []
