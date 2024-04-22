module pblsh.Models


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

module QueryStrings =

    [<CLIMutable>]
    type RedirectInfo = { ReturnUrl: string }
