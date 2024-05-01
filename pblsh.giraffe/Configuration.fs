module pblsh.Configuration

open System.IO
open Microsoft.Extensions.Configuration

let contentRoot = Directory.GetCurrentDirectory()
let webRoot = Path.Combine(contentRoot, "WebRoot")
    
let configuration =
    ConfigurationBuilder()
        .SetBasePath(contentRoot)
        .AddJsonFile("appsettings.json", false)
        .AddJsonFile("appsettings.development.json", true)
        .Build()
