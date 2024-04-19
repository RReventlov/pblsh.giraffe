module pblsh.Handlers

open System.Text.Json
open Giraffe
open Microsoft.AspNetCore.Identity
open pblsh.Models

let getIndex () =
    let view = Views.index ()
    htmlView view

let getLogin () =
    let view = Views.login ()
    htmlView view

let postLogin (loginInfo: Forms.LoginInfo) : HttpHandler =
    fun next ctx ->
        task {
            let loginManager = ctx.GetService<SignInManager<IdentityUser>>()
            let! login = loginManager.PasswordSignInAsync(loginInfo.UserName, loginInfo.Password, false, false)

            return!
                (if login.Succeeded then
                     text "Login succeeded"
                 else
                     text "Login failed")
                    next
                    ctx
        }

let getSignup () =
    let view = Views.signup ()
    htmlView view

let postSignup (uncheckedSignUpInfo: Forms.UncheckedSignUpInfo) : HttpHandler =
    fun next ctx ->
        task {
            let userManager = ctx.GetService<UserManager<IdentityUser>>()

            let user =
                IdentityUser(Email = uncheckedSignUpInfo.Email, UserName = uncheckedSignUpInfo.UserName)

            let! result = userManager.CreateAsync(user, uncheckedSignUpInfo.Password)

            let view =
                if result.Succeeded then
                    Views.signUpComplete ()
                else
                    (Views.errorWithRedirect "account/signup")

            return! (htmlView view) next ctx
        }
