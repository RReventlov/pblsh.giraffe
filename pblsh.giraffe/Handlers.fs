module pblsh.Handlers

open Giraffe
open Microsoft.AspNetCore.Identity
open Microsoft.AspNetCore.Http
open pblsh.Models
open pblsh.Models.Forms
open pblsh.Models.QueryStrings

let getUser (ctx: HttpContext) = { UserName = ctx.User.Identity.Name }

let getUserOption (ctx: HttpContext) =
    if (ctx.User.Identity.IsAuthenticated) then
        Some(getUser ctx)
    else
        None

let setSessionRecord (ctx: HttpContext) key value =
    ctx.Session.SetString(key, System.Text.Json.JsonSerializer.Serialize(value))

let getIndex () : HttpHandler =
    fun next ctx ->
        let view = Views.index (getUserOption ctx)
        htmlView view next ctx

let getLogin () : HttpHandler =
    fun next ctx ->
        let redirectAfterLogin = ctx.TryBindQueryString<RedirectInfo>() |> Result.toOption

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
                    | Error _ -> redirectTo true "/account/me" next ctx
            else
                return! text (ctx.GetRequestUrl()) next ctx
        }

let getSignup () =
    let view = Views.signup ()
    htmlView view

let postSignup (uncheckedSignUpInfo: UncheckedSignUpInfo) : HttpHandler =
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
                    (Views.errorWithRedirect "/account/signup")

            return! (htmlView view) next ctx
        }

let getAccount () : HttpHandler =
    fun next ctx ->
        let userInfo = getUser ctx

        htmlView (Views.me userInfo) next ctx

let getLogout () : HttpHandler =
    fun next ctx ->
        task {
            let loginManager = ctx.GetService<SignInManager<IdentityUser>>()
            let! logout = loginManager.SignOutAsync()

            return! redirectTo true "/index" next ctx
        }

let getNewPost () : HttpHandler =
    fun next ctx ->
        let userInfo = getUser ctx

        htmlView (Views.newPost userInfo) next ctx

