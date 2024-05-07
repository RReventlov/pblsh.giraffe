module pblsh.Workflows

open System
open System.IO
open Microsoft.AspNetCore.Http
open SqlHydra.Query
open pblsh.Hydra
open pblsh.Hydra.main
open pblsh.Paths
open pblsh.Configuration
open pblsh.Models
open pblsh.DataAccess
open pblsh.Types

module Posts =

    let postDir post = sprintf "%s/%O" postRoot post.Id

    let persistPost (post: Post) (files: IFormFileCollection) =
        let insertedRows =
            insertTask createDbx {
                into posts
                entity {
                    posts.Id = post.Id.ToString()
                    posts.Author = post.Author
                    posts.Title = String5.value post.Title
                    posts.PublishedOn = DateOnly.FromDateTime post.PublishedOn 
                }
            }
        insertedRows.Result |> ignore
        let contentRoot = sprintf "%s/%s" postRoot (post.Id.ToString())
        Directory.CreateDirectory contentRoot |> ignore
        for file in files do
            let fs = File.Create((sprintf "%s/%s" contentRoot file.FileName))
            file.CopyTo fs
            fs.Dispose()
            

    let saveNewPost author title dots files =
        path {
            let! post = Post.createNew author title dots
            persistPost post files
            return post
        }
