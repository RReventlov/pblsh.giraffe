module pblsh.Handlers

open Giraffe
open Microsoft.AspNetCore.Identity
open pblsh.Models
open pblsh.Models.Forms
open pblsh.Models.QueryStrings

let getIndex () =
    let view = Views.index ()
    htmlView view

let getLogin () : HttpHandler =
    fun next ctx ->
        let redirectAfterLogin =
            ctx.TryBindQueryString<RedirectInfo>() |> Result.toOption

        let view = Views.login redirectAfterLogin
        htmlView view next ctx

let postLogin (loginInfo: Forms.LoginInfo) : HttpHandler =
    fun next ctx ->
        task {
            let loginManager = ctx.GetService<SignInManager<IdentityUser>>()
            let! login = loginManager.PasswordSignInAsync(loginInfo.UserName, loginInfo.Password, false, false)
            
            if login.Succeeded then
                return!
                    match ctx.TryBindQueryString<RedirectInfo>() with
                    | Ok r -> redirectTo true r.ReturnUrl next ctx
                    | Error e -> redirectTo true "/account/me" next ctx
            else
                return! text (ctx.GetRequestUrl()) next ctx
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
