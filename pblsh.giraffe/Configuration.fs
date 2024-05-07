module pblsh.Configuration

open System.IO
open Microsoft.Extensions.Configuration

let contentRoot = Directory.GetCurrentDirectory()
let webRoot = Path.Combine(contentRoot, "WebRoot")

/// Contains configuration on paths and Db-Connections
let configuration =
    ConfigurationBuilder()
        .SetBasePath(contentRoot)
        .AddJsonFile("appsettings.json", false)
        .AddJsonFile("appsettings.development.json", true)
        .Build()

let postRoot = configuration["postRoot"]

let connectionString = configuration["connectionString"]