module pblsh.App

open System
open System.IO
open Microsoft.AspNetCore.Authentication.Cookies
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Cors.Infrastructure
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http.HttpResults
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection
open Giraffe

let authScheme = CookieAuthenticationDefaults.AuthenticationScheme

let indexHandler () =
    let view = Views.index ()
    htmlView view

let loginHandler () =
    let view = Views.login ()
    htmlView view

let logoutHandler () =
    signOut authScheme >=> redirectTo false "/index"

let webApp =
    choose
        [ route "/index" >=> indexHandler ()
          subRouteCi
              "/account"
              (choose
                  [ routeCi "/login" >=> loginHandler ()
                    requiresAuthentication (challenge authScheme)
                    >=> choose [ routeCi "/logout" >=> text "Logout is coming soon" ] ])
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

    (match env.IsDevelopment() with
     | true -> app.UseDeveloperExceptionPage()
     | false -> app.UseGiraffeErrorHandler(errorHandler).UseHttpsRedirection())
        .UseCors(configureCors)
        .UseStaticFiles()
        .UseGiraffe(webApp)

let configureServices (services: IServiceCollection) =
    services.AddCors() |> ignore
    services.AddGiraffe() |> ignore
    services.AddRouting(fun o ->
        o.LowercaseUrls <- true
        o.LowercaseQueryStrings <- true) |> ignore
    services.AddAuthentication().AddCookie(authScheme) |> ignore

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
