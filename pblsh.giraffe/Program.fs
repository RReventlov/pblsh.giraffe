module pblsh.App

open System
open System.IO
open Microsoft.AspNetCore.Authentication.Cookies
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Cors.Infrastructure
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Identity
open Microsoft.EntityFrameworkCore
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection
open Giraffe
open pblsh.Models
open pblsh.Models.Forms
open pblsh.giraffe.Identity

let authScheme = CookieAuthenticationDefaults.AuthenticationScheme

let logoutHandler () =
    signOut authScheme >=> redirectTo false "/index"

let parsingError (err: string) = RequestErrors.BAD_REQUEST err

let webApp =
    choose
        [ route "/" >=> redirectTo true "/index"
          routeCi "/search" >=> text "hi" //>=> POST >=> tryBindForm<SearchContent> parsingError None Handlers.postSearch
          route "/index" >=> Handlers.getIndex ()
          routeCix "/account/login(.*)"
          >=> choose
                  [ GET >=> Handlers.getLogin ()
                    POST >=> tryBindForm<LoginInfo> parsingError None Handlers.postLogin ]
          routeCi "/account/signup"
          >=> choose
                  [ GET >=> Handlers.getSignup ()
                    POST >=> tryBindForm<UncheckedSignUpInfo> parsingError None Handlers.postSignup ]
          setStatusCode 404 >=> text "🐈 Not Found 🐈‍⬛"
          // Placing handlers that do not require authentication below this line will result in them requiring
          // authentication as well.
          // This is because requiresAuthentication will count failing the authentication as a *hit* and then redirect
          // the user to the login page.
          requiresAuthentication (challenge authScheme)
          >=> choose
                  [ subRoute
                        "/account"
                        (choose
                            [ routeCi "/logout" >=> Handlers.getLogout ()
                              routeCi "/me" >=> Handlers.getAccount () ])
                    GET >=> route "/post/new" >=> Handlers.getNewPost ()
                    POST >=> route "/post/new" >=> bindForm<NewPostInfo> None Handlers.postNewPost
                    route "/am-i-authenticated" >=> text "you are authenticated" ] ]

let errorHandler (ex: Exception) (logger: ILogger) =
    logger.LogError(ex, "An unhandled exception has occurred while executing the request.")
    clearResponse >=> setStatusCode 500 >=> text ex.Message

let configureAppConfiguration (builder: IConfigurationBuilder) =
    builder.AddJsonFile("appsettings.json", true) |> ignore

let configureCors (builder: CorsPolicyBuilder) =
    builder
        .WithOrigins("http://localhost:5000", "https://localhost:5001")
        .AllowAnyMethod()
        .AllowAnyHeader()
    |> ignore

let configureApp (app: IApplicationBuilder) =
    let env = app.ApplicationServices.GetService<IWebHostEnvironment>()

    app.UseAuthentication() |> ignore
    app.UseSession() |> ignore

    (match env.IsDevelopment() with
     | true -> app.UseDeveloperExceptionPage()
     | false -> app.UseGiraffeErrorHandler(errorHandler).UseHttpsRedirection())
        .UseCors(configureCors)
        .UseStaticFiles()
        .UseGiraffe(webApp)

let configureServices (config: IConfiguration) (services: IServiceCollection) =
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
        .AddDbContext<ApplicationDbContext>(fun o -> o.UseSqlServer(config["connectionString"]) |> ignore)
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

    let config =
        ConfigurationBuilder()
            .SetBasePath(contentRoot)
            .AddJsonFile("appsettings.json", false)
            .Build()

    Host
        .CreateDefaultBuilder(args)
        .ConfigureAppConfiguration(configureAppConfiguration)
        .ConfigureWebHostDefaults(fun webHostBuilder ->
            webHostBuilder
                .UseConfiguration(config)
                .UseContentRoot(contentRoot)
                .UseWebRoot(webRoot)
                .Configure(Action<IApplicationBuilder> configureApp)
                .ConfigureServices(configureServices config)
                .ConfigureLogging(configureLogging)
            |> ignore)
        .Build()
        .Run()

    0
