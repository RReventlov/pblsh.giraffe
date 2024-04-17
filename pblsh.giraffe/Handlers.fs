module pblsh.Handlers

open Giraffe
open Microsoft.AspNetCore.Identity
open pblsh.Models

let getIndex () =
    let view = Views.index ()
    htmlView view

let getLogin () =
    let view = Views.login ()
    htmlView view

let postLogin () = text "postHandler"

let getSignup () =
    let view = Views.signup ()
    htmlView view
    
let postSignup (uncheckedSignUpInfo: UncheckedSignUpInfo) : HttpHandler = 
    fun nxt ctx ->
        task {
            let userManager = ctx.GetService<UserManager<IdentityUser>>()
            let user = IdentityUser(Email = uncheckedSignUpInfo.Email, UserName = uncheckedSignUpInfo.UserName)
            let! result = userManager.CreateAsync(user, uncheckedSignUpInfo.Password)
            
            let view = if result.Succeeded then Views.confirmEmail () else (Views.errorWithRedirect "account/signup")
            
            return! (htmlView view) nxt ctx
        }