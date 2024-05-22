﻿module pblsh.Components

open System
open Giraffe.ViewEngine.Attributes
open Giraffe.ViewEngine.HtmlElements
open pblsh.Models
open pblsh.Types

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
        [ form
              [ _action "/search"; _method "post" ]
              [ input [ _id "search"; _type "input"; _name "Query"; _placeholder "Search..." ]
                input [ _type "submit"; _style "display:none" ] ]

          ]
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

let dot dot =
    a
        [ _class "dot"; _href (sprintf "/dots/%s" (String1.value dot)) ]
        [ encodedText (sprintf ".%s" (String1.value dot)) ]

let postCard postInfo =
    div
        [ _class "postcard" ]
        [ h2
              []
              [ a
                    [ _href (Urls.postUrl postInfo) ]
                    [ encodedText (String1.value postInfo.Title) ] ]
          div
              []
              [ span [] [ encodedText "Written by " ]
                a [ _href (Urls.userUrl postInfo); _class "postcard-author" ] [ encodedText (String1.value postInfo.Author) ] ]
          div [ _class "postcard-tags" ] [ yield! postInfo.Dots |> List.map dot ] ]
        
let rec commentCard commentInfo =
    div
        [ _class "commentcard"; ]
        [ h2
            []
            [
                a [ _href (Urls.userUrlComment commentInfo) ] [ encodedText  commentInfo.Author ]
                ]
          div [ _class "content" ] [
              rawText  commentInfo.Content
          ]
          span [ _class "interactions" ] [
              a [ _href "#" ] [ encodedText "Like" ]
              a [ _href "#" ] [ encodedText "Reply" ]
          ]
          if(commentInfo.Children.Length > 0) then
              div [ _class "replies" ] [
              div [] (commentInfo.Children |> List.map commentCard)
           ]
    ]

let newCommentDialog (id:Guid) =
    form [ _action (sprintf "/posts/%O/comments" id); _method "post" ] [
        textarea [_name "content" ;_rows "5"; _cols "20"] []
        br []
        span [_class "Buttons"] [
            input [ _type "submit"; _value "Submit"; _class "filled-action"]
            input [ _type "reset"; _value "Reset"; _class "warned-action" ]
        ]
    ]

let commentCardPlaceholder commentInfo = commentCard {Author = "Lars"; Id = Guid.Empty; AuthorId  = Guid.Empty; Content = "Hallo das ist ein Testkommentar"; Depth = 0; Children = [{Author = "Lars"; Id = Guid.Empty; AuthorId  = Guid.Empty; Content = "Hallo das ist eine Testantwort"; Depth = 0; Children = [] }] }
let dialogCssJs (cssFiles: string list) (jsFiles: string list) (pageTitle: string) (content: XmlNode list) =
    [ emptyTopRow (); main [ _class "shiny" ] [ div [ _id "center" ] content ] ]
    |> titledLayoutCssJs ("dialog.css" :: cssFiles) ("mousetracker.js" :: jsFiles) pageTitle

let dialogCss cssFiles = dialogCssJs cssFiles []

let dialogJs jsFiles = dialogCssJs [] jsFiles

let dialog = dialogCssJs [] []
