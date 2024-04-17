module pblsh.Components

open Giraffe.ViewEngine.Attributes
open Giraffe.ViewEngine.HtmlElements
open pblsh.Models
        
let titledLayoutCJ (pageTitle: string) (cssFiles: string list) (jsFiles: string list) (content: XmlNode list) =
    html
        []
        [ head
              []
              [ title [] [ encodedText pageTitle ]
                link [ _rel "stylesheet"; _type "text/css"; _href "/pblsh.reboot.css" ]
                link [ _rel "stylesheet"; _type "text/css"; _href "/pblsh.css" ]
                yield!
                    cssFiles
                    |> List.map (fun f -> link [ _rel "stylesheet"; _type "text/css"; _href (sprintf "/%s" f) ])
              ]
          body [] content
          yield!
            jsFiles
            |> List.map (fun f -> script [ _type "text/javascript"; _src (sprintf "/%s" f) ] [])
        ]         
        
let titledLayout (pageTitle: string) =
    titledLayoutCJ pageTitle [] []

let titledLayoutCss (pageTitle: string) (cssFiles: string list) =
    titledLayoutCJ pageTitle cssFiles []
        
let titledLayoutJs (pageTitle: string) (jsFiles: string list) =
    titledLayoutCJ pageTitle [] jsFiles

let layout (content: XmlNode list) = titledLayout "pblsh" content

let partial () =
    header
        []
        [ div [ _id "logo" ] [ a [ _href "/index" ] [ encodedText "pblsh" ] ]
          input [ _id "search"; _type "input"; _placeholder "Search..." ]
          div
              [ _id "header-right" ]
              [ div [ _class "action" ] [ a [ _id "login"; _href "/account/login" ] [ encodedText "Log in" ] ]
                div [ _class "filled-action" ] [ a [ _id "signup"; _href "/account/signup" ] [ encodedText "Sign up" ] ] ] ]

let emptyPartial () =
    header [] [ div [ _id "logo" ] [ a [ _href "/index" ] [ encodedText "pblsh" ] ] ]

let private routeElement (routePart: RoutePart) =
    div
        [ _class "route-part" ]
        [ a [ _href routePart.Link ] [ encodedText routePart.Text ]
          img [ _src "/icons/chevron.svg"; _height "14" ] ]

let navigation (route: RoutePart list) =
    div [ _id "navigation" ] [ yield! route |> List.map routeElement ]

let dot str =
    a [ _class "tag"; _href (sprintf "/dots/%s" str) ] [ encodedText (sprintf ".%s" str) ]
