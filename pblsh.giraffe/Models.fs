module pblsh.Models

open Microsoft.AspNetCore.Http
open pblsh.Types


type Message = { Text: string }

type RoutePart = { Text: string; Link: string }

type UserInfo = { UserName: string }

module Forms =
    [<CLIMutable>]
    type UncheckedSignUpInfo =
        { Email: string
          Password: string
          UserName: string }

    [<CLIMutable>]
    type LoginInfo = { UserName: string; Password: string }

    [<CLIMutable>]
    type NewPostInfo =
        { Title: string
          Dots: string }
        
    type ValidatedNewPostInfo = {
        Title: string5
        Dots: string
    }
    
    module NewPostInfo =
        let validate newPostInfo = ()
            

module QueryStrings =

    [<CLIMutable>]
    type RedirectInfo = { ReturnUrl: string }
