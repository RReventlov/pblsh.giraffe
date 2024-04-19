module pblsh.App

open System
open System.IO
open Microsoft.AspNetCore.Authentication.Cookies
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Cors.Infrastructure
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Identity
open Microsoft.EntityFrameworkCore
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection
open Giraffe
open pblsh.Models.Forms
open pblsh.Models.QueryStrings
open pblsh.giraffe.Identity

let authScheme = CookieAuthenticationDefaults.AuthenticationScheme

let logoutHandler () =
    signOut authScheme >=> redirectTo false "/index"

let parsingError (err: string) = RequestErrors.BAD_REQUEST err

let webApp =
    choose
        [ route "/index" >=> Handlers.getIndex ()
          routeCi "/account/login"
          >=> choose
                  [ GET >=> Handlers.getLogin ()
                    POST >=> tryBindForm<LoginInfo> parsingError None Handlers.postLogin ]
          routeCi "/account/signup"
          >=> choose
                  [ GET >=> Handlers.getSignup ()
                    POST >=> tryBindForm<UncheckedSignUpInfo> parsingError None Handlers.postSignup ]
          requiresAuthentication (challenge authScheme)
          >=> choose
                  [ subRoute
                        "/account"
                        (choose
                            [ routeCi "/logout" >=> text "logout is coming soon"
                              routeCi "/me" >=> text "this is your account" ])
                    route "/am-i-authenticated" >=> text "you are authenticated" ]
          setStatusCode 404 >=> text "🐈 Not Found 🐈‍⬛" ]

let errorHandler (ex: Exception) (logger: ILogger) =
    logger.LogError(ex, "An unhandled exception has occurred while executing the request.")
    clearResponse >=> setStatusCode 500 >=> text ex.Message

let configureCors (builder: CorsPolicyBuilder) =
    builder
        .WithOrigins("http://localhost:5000", "https://localhost:5001")
        .AllowAnyMethod()
        .AllowAnyHeader()
    |> ignore

let configureApp (app: IApplicationBuilder) =
    let env = app.ApplicationServices.GetService<IWebHostEnvironment>()

    app.UseAuthentication() |> ignore

    (match env.IsDevelopment() with
     | true -> app.UseDeveloperExceptionPage()
     | false -> app.UseGiraffeErrorHandler(errorHandler).UseHttpsRedirection())
        .UseCors(configureCors)
        .UseStaticFiles()
        .UseGiraffe(webApp)

let configureServices (services: IServiceCollection) =
    services.AddCors() |> ignore
    services.AddGiraffe() |> ignore
    services.AddAuthentication().AddCookie(authScheme) |> ignore

    services
        .AddDistributedMemoryCache()
        .AddRouting(fun o ->
            o.LowercaseUrls <- true
            o.LowercaseQueryStrings <- true)
    |> ignore

    services
        .AddDbContext<ApplicationDbContext>(fun o ->
            o.UseSqlServer("data source=Orion\MSSQLSERVER03;initial catalog=pblsh.identity;trusted_connection=true")
            |> ignore)
        .AddDefaultIdentity<IdentityUser>()
        .AddEntityFrameworkStores<ApplicationDbContext>()
    |> ignore


    services.AddSession(fun o ->
        o.Cookie.HttpOnly <- true
        o.Cookie.IsEssential <- true)
    |> ignore

let configureLogging (builder: ILoggingBuilder) =
    builder.AddConsole().AddDebug() |> ignore

[<EntryPoint>]
let main args =
    let contentRoot = Directory.GetCurrentDirectory()
    let webRoot = Path.Combine(contentRoot, "WebRoot")

    Host
        .CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(fun webHostBuilder ->
            webHostBuilder
                .UseContentRoot(contentRoot)
                .UseWebRoot(webRoot)
                .Configure(Action<IApplicationBuilder> configureApp)
                .ConfigureServices(configureServices)
                .ConfigureLogging(configureLogging)
            |> ignore)
        .Build()
        .Run()

    0
