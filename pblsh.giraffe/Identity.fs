module pblsh.giraffe.Identity

open System
open Microsoft.AspNetCore.Identity.EntityFrameworkCore
open Microsoft.EntityFrameworkCore
open Microsoft.EntityFrameworkCore.Design
open Microsoft.Extensions.Configuration
type ApplicationDbContext(options: DbContextOptions<ApplicationDbContext>) =
    inherit IdentityDbContext(options)

type ApplicationDbContextFactory() =
    interface IDesignTimeDbContextFactory<ApplicationDbContext> with
        member _.CreateDbContext(args: string[]) =
            let optionsBuilder = DbContextOptionsBuilder<ApplicationDbContext>()
            let config =
                ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.json")
                    .Build()

            optionsBuilder.UseSqlServer(config["connectionString"]) |> ignore

            new ApplicationDbContext(optionsBuilder.Options)
