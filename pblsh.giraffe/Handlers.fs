module pblsh.Handlers

open System
open System.Threading
open Giraffe
open Microsoft.AspNetCore.Http.Features
open Microsoft.AspNetCore.Identity
open Microsoft.AspNetCore.Http
open pblsh.Hydra.main
open pblsh.Models.Post
open pblsh.Helper
open pblsh.Paths
open pblsh.Models
open pblsh.Models.Forms
open pblsh.Models.QueryStrings
open pblsh.Workflows

let getForm (ctx: HttpContext) =
    task {
        let formFeature = ctx.Features.Get<IFormFeature>()
        return! formFeature.ReadFormAsync CancellationToken.None
    }

let getUser (ctx: HttpContext) = { UserName = ctx.User.Identity.Name; Id = Guid.Empty }

let getUserOption (ctx: HttpContext) =
    if ctx.User.Identity.IsAuthenticated then
        Some(getUser ctx)
    else
        None

let setSessionRecord (ctx: HttpContext) key value =
    ctx.Session.SetString(key, System.Text.Json.JsonSerializer.Serialize(value))

let getIndex () : HttpHandler =
    fun next ctx ->
        let topPosts = Posts.getTop 10 |> await
        let view = Views.index (getUserOption ctx) topPosts
        htmlView view next ctx
     
let postSearch (query: SearchContent) : HttpHandler =
    fun next ctx ->
        let parserResult = Parser.Query.parse(query)
        let results = match parserResult with
                        | Ok p -> Search.searchPosts(p) |> await
                        | Error _ -> []
                                     
        let view = Views.search (getUserOption ctx) query.Query results
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
                return! text "An error occurred during login" next ctx
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
        (*let userID = Users.getUserIDbyName(userInfo.UserName).Result
        Console.WriteLine(userInfo.Id)
        let articles = Posts.getPostsByAuthorId(userID).Result*)
        htmlView (Views.me userInfo (*articles*)) next ctx

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

            let userManager = ctx.GetService<UserManager<IdentityUser>>()
            let authorId = userManager.GetUserId(ctx.User)
            let dots = newPostInfo.Dots.Split(".") |> List.ofArray
            let user = getUser ctx

            return!
                match Posts.saveNewPost authorId newPostInfo.Title dots form.Files with
                | Happy h -> redirectTo true (sprintf "/post/%s" (h.Id.ToString())) next ctx
                | Sad s ->
                    let errors =
                        s
                        |> List.collect (fun x ->
                            match x with
                            | DotTooShort msg -> msg
                            | TitleTooShort -> [ "The title is too short" ])

                    htmlView (Views.newPost user errors) next ctx
        }
        
let getUserById (id: Guid) : HttpHandler =
    fun next ctx ->
        let user = getUserOption ctx
        let userInfo = Users.getUser(id) |> await
        //let userInfo = user.Value
        let idStr = id.ToString()
        let articles = Posts.getPostsByAuthor id |> await
        let view = Views.userView user userInfo articles
        htmlView view next ctx
        
let postComment (postId:Guid) : HttpHandler =
    fun next ctx -> task {
        let! comment = ctx.TryBindFormAsync<NewComment>()
        let userManager = ctx.GetService<UserManager<IdentityUser>>()
        let authorId = userManager.GetUserId(ctx.User)
        
        match mapR comment with
        | Happy comment ->
            
            let newId = Posts.postComment comment postId authorId
            let redirectId = if comment.Parent.Equals Guid.Empty then newId else comment.Parent
            return! redirectTo true (sprintf "/posts/%O#%O" postId redirectId) next ctx
                
        | Sad _ ->
            return! htmlView (Views.errorWithRedirect "") next ctx
    }
        
let getPost (id: Guid) : HttpHandler =
    fun next ctx ->
        let post = Posts.getPost id |> await
        let user = getUserOption ctx
        let comments = Posts.getComments id |> await
        match post with
        | Happy postInfo ->
            let content = Posts.getContent postInfo
            
            match content with
            | Happy content -> htmlView (Views.post user postInfo content comments) next ctx
            | Sad _ -> htmlView (Views.errorWithRedirect "") next ctx
        | Sad _ -> htmlView (Views.errorWithRedirect "") next ctx
        
let getDot (dot:string) : HttpHandler =
    fun next ctx ->
        let user = getUserOption ctx
        let articles = Posts.getPostsByDot dot |> await
        let view = Views.dotView user dot articles
        htmlView view next ctx
