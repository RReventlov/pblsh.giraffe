module pblsh.giraffe.Identity

open System
open Microsoft.AspNetCore.Identity
open Microsoft.AspNetCore.Identity.EntityFrameworkCore
open Microsoft.EntityFrameworkCore
open Microsoft.EntityFrameworkCore.Design

type ApplicationDbContext(options: DbContextOptions<ApplicationDbContext>) =
    inherit IdentityDbContext(options)

type ApplicationDbContextFactory() =
    interface IDesignTimeDbContextFactory<ApplicationDbContext> with
        member _.CreateDbContext(args: string[]) =
            let optionsBuilder = DbContextOptionsBuilder<ApplicationDbContext>()

            optionsBuilder.UseSqlServer(
                "data source=Orion\MSSQLSERVER03;initial catalog=pblsh.identity;trusted_connection=true"
            )
            |> ignore

            new ApplicationDbContext(optionsBuilder.Options)
