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
            let connectionString = configuration["connectionString"]

            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
            |> ignore

            new ApplicationDbContext(optionsBuilder.Options)


[<Literal>]
let dbVendor = Common.DatabaseProviderTypes.MYSQL

[<Literal>]
let useOptions = Common.NullableColumnType.OPTION

type sql =
    SqlDataProvider<dbVendor, "server=localhost;port=3307;database=pblsh;user=root;password=root;", UseOptionTypes=useOptions>

let ctx = sql.GetDataContext ()