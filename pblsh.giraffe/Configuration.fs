module pblsh.Configuration

type AppSettings = { ConnectionString: string }

let appSettings =
    let text = System.IO.File.ReadAllText "appsettings.dev.json"
    System.Text.Json.JsonSerializer.Deserialize<AppSettings> text
