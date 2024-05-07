module pblsh.DataAccess

open Microsoft.AspNetCore.Identity.EntityFrameworkCore
open Microsoft.EntityFrameworkCore
open Microsoft.EntityFrameworkCore.Design
open FSharp.Data.Sql
open pblsh.Configuration

type ApplicationDbContext(options: DbContextOptions<ApplicationDbContext>) =
    inherit IdentityDbContext(options)

type ApplicationDbContextFactory() =
    interface IDesignTimeDbContextFactory<ApplicationDbContext> with
        member _.CreateDbContext(args: string[]) =
            let optionsBuilder = DbContextOptionsBuilder<ApplicationDbContext>()

            optionsBuilder.UseSqlite(connectionString)
            |> ignore

            new ApplicationDbContext(optionsBuilder.Options)