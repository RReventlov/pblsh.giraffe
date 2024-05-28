module pblsh.Components

open System
open Giraffe.ViewEngine.Attributes
open Giraffe.ViewEngine.HtmlElements
open pblsh.Models
open pblsh.Types
open pblsh.Workflows
open pblsh.Helper
open pblsh.Paths


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
        [ h2 [] [ a [ _href (Urls.postUrl postInfo) ] [ encodedText (String1.value postInfo.Title) ] ]
          div
              []
              [ span [] [ encodedText "Written by " ]
                a
                    [ _href (Urls.userUrl postInfo); _class "postcard-author" ]
                    [ encodedText (String1.value postInfo.Author) ] ]
          div [ _class "postcard-tags" ] [ yield! postInfo.Dots |> List.map dot ] ]

let replyContainer (actionUrl: string) =
    form
        [ _class "comment-reply-container"; _action actionUrl; _method "post" ]
        [ textarea [ _name "Content" ] []
          div
              [ _class "comment-reply-actions" ]
              [ button
                    [ _class "comment-reply-send transparent-action" ]
                    [ encodedText "Send "; img [ _src "/icons/send.svg" ] ]
                button
                    [ _class "comment-reply-cancel transparent-action"; _type "button" ]
                    [ encodedText "Cancel"; img [ _src "/icons/slash-circle.svg" ] ] ] ]

let replyToComment (commentInfo: CommentInformation) (userInfo: UserInfo option) =
    match userInfo with
    | None -> div [] []
    | Some _ -> replyContainer (sprintf "/posts/%O/comments/%O" commentInfo.PostId commentInfo.Id)

let rec commentCard (commentInfo: CommentInformation) (userInfo: UserInfo option) =
    let replyButton =
        match userInfo with
        | None ->
            a
                [ _type "button"
                  _href (Urls.loginWithRedirect (sprintf "/posts/%O%%23%O" commentInfo.PostId commentInfo.Id))
                  _class "comment-reply-login transparent-action-link" ]
        | Some _ ->
            button
                [ _type "button"
                  _class "comment-reply-open comment-open-reply-box transparent-action" ]

    div
        [ _class "comment-card"; _id (commentInfo.Id.ToString()) ]
        [ p
              [ _class "comment-header" ]
              [ a [ _href (Urls.userUrlComment commentInfo) ] [ encodedText (String1.value commentInfo.Author) ]
                replyButton [ encodedText "Reply"; img [ _src "/icons/chat-dots.svg" ] ] ]
          p [ _class "comment-content" ] [ rawText (String1.value commentInfo.Content) ]
          replyToComment commentInfo userInfo
          if (commentInfo.Replies |> List.length) <> 0 then
              div
                  [ _class "comment-replies-container" ]
                  [ button [ _class "toggle-replies" ] []
                    div
                        [ _class "comment-replies" ]
                        ((Posts.getReplies commentInfo.Replies)
                         |> List.map (fun c -> commentCard c userInfo))
                    div
                        [ _style "display: none" ]
                        [ encodedText (sprintf "%d replies" (commentInfo.Replies |> List.length)) ] ]
          else
              div [] [] ]

let newCommentDialog (id: Guid) =
    form
        [ _action (sprintf "/posts/%O/comments" id); _method "post" ]
        [ textarea [ _name "Content"; _rows "5"; _cols "20" ] []
          input [ _type "hidden"; _name "Parent"; _value (Guid.Empty.ToString()) ]
          br []
          span
              [ _class "Buttons" ]
              [ input [ _type "submit"; _value "Submit"; _class "filled-action" ]
                input [ _type "reset"; _value "Reset"; _class "warned-action" ] ] ]

let dialogCssJs (cssFiles: string list) (jsFiles: string list) (pageTitle: string) (content: XmlNode list) =
    [ emptyTopRow (); main [ _class "shiny" ] [ div [ _id "center" ] content ] ]
    |> titledLayoutCssJs ("dialog.css" :: cssFiles) ("mousetracker.js" :: jsFiles) pageTitle

let dialogCss cssFiles = dialogCssJs cssFiles []

let dialogJs jsFiles = dialogCssJs [] jsFiles

let dialog = dialogCssJs [] []
