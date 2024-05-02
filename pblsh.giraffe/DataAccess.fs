module pblsh.DataAccess

open System
open System.ComponentModel.DataAnnotations
open System.ComponentModel.DataAnnotations.Schema
open Microsoft.AspNetCore.Identity
open Microsoft.AspNetCore.Identity.EntityFrameworkCore
open Microsoft.EntityFrameworkCore
open Microsoft.EntityFrameworkCore.Design
open EntityFrameworkCore.FSharp.Extensions
open pblsh.Configuration

module Dtos =

    [<CLIMutable>]
    type PostDto =
        { [<Key>]
          Id: Guid
          Author: IdentityUser
          Title: string
          Dots: List<string>
          PublishedOn: DateTime
          UniqueViews: int }

type ApplicationDbContext(options: DbContextOptions<ApplicationDbContext>) =
    inherit IdentityDbContext(options)

    [<DefaultValue>]
    val mutable posts: DbSet<Dtos.PostDto>

    member this.Posts
        with get () = this.posts
        and set v = this.posts <- v

    override _.OnModelCreating builder =
        builder.RegisterOptionTypes()
        
        base.OnModelCreating builder

    override _.OnConfiguring(options: DbContextOptionsBuilder) =
        options.UseSqlServer(configuration["connectionString"]) |> ignore

type ApplicationDbContextFactory() =
    interface IDesignTimeDbContextFactory<ApplicationDbContext> with
        member _.CreateDbContext(args: string[]) =
            let optionsBuilder = DbContextOptionsBuilder<ApplicationDbContext>()
            optionsBuilder.UseSqlServer(configuration["connectionString"]) |> ignore
            new ApplicationDbContext(optionsBuilder.Options)
