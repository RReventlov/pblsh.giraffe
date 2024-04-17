module pblsh.Models


type Message = { Text: string }

type RoutePart = { Text: string; Link: string }

[<CLIMutable>]
type UncheckedSignUpInfo = {
    Email: string
    Password: string
    UserName: string
}