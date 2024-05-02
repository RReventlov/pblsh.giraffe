module pblsh.Handlers

open System.Threading
open Giraffe
open Giraffe.ViewEngine
open Microsoft.AspNetCore.Http.Features
open Microsoft.AspNetCore.Identity
open Microsoft.AspNetCore.Http
open pblsh.Models
open pblsh.Models.Forms
open pblsh.Models.QueryStrings

let getForm (ctx: HttpContext) =
    task {
        let formFeature = ctx.Features.Get<IFormFeature>()
        return! formFeature.ReadFormAsync CancellationToken.None
    }

let getUser (ctx: HttpContext) = { UserName = ctx.User.Identity.Name }

let getUserOption (ctx: HttpContext) =
    if ctx.User.Identity.IsAuthenticated then
        Some(getUser ctx)
    else
        None

let setSessionRecord (ctx: HttpContext) key value =
    ctx.Session.SetString(key, System.Text.Json.JsonSerializer.Serialize(value))

let getIndex () : HttpHandler =
    fun next ctx ->
        let view = Views.index (getUserOption ctx)
        htmlView view next ctx

let postSearch (content: SearchContent) : HttpHandler =
    fun next ctx ->
        let queryResult =
            match Parser.Query.parse content with
            | Error e -> e
            | Ok o -> "parsing succeeded"
        let results = ["";" "]   
        let view = Views.search (getUserOption ctx) content.Query results   
        htmlView view next ctx
  
let getLogin () : HttpHandler =
    fun next ctx ->
        let redirectAfterLogin = ctx.TryBindQueryString<RedirectInfo>() |> Result.toOption

        let view = Views.login redirectAfterLogin
        htmlView view next ctx

let postLogin (loginInfo: LoginInfo) : HttpHandler =
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
            let! _ = loginManager.SignOutAsync()

            return! redirectTo true "/index" next ctx
        }

let getNewPost () : HttpHandler =
    fun next ctx ->
        let userInfo = getUser ctx

        htmlView (Views.newPost userInfo []) next ctx

let postNewPost (newPostInfo: NewPostInfo) : HttpHandler =
    fun next ctx ->
        task {
            let! form = getForm ctx
            let files = form.Files |> Seq.fold (fun a s -> sprintf "%s\n%s" a s.FileName) ""

            return!
                htmlView
                    (Giraffe.ViewEngine.HtmlElements.p
                        []
                        [ Giraffe.ViewEngine.HtmlElements.encodedText (
                              sprintf "%s\n\nFiles:%s" (newPostInfo.ToString()) files
                          ) ])
                    next
                    ctx
        }
