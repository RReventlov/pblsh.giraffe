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

                entity
                    { posts.Id = post.Id.ToString()
                      posts.Author = post.Author
                      posts.Title = String5.value post.Title
                      posts.PublishedOn = DateOnly.FromDateTime post.PublishedOn }
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

    let dotsFor postId =
        (selectTask HydraReader.Read createDbx {
            for d in dots do
                where (d.UsedBy = postId)
                select d.Dot
        })
            .Result
        |> List.ofSeq

    let createPostInformation ((id, title, author): string * string * string) =
        let dots = dotsFor id
        PostInformation.create id author title dots


    let getTop n =
        task {
            let! results =
                selectTask HydraReader.Read createDbx {
                    for p in posts do
                        join u in AspNetUsers on (p.Author = u.Id)
                        select (p.Id, p.Title, u.UserName)
                        take n
                }

            return
                results
                |> Seq.map createPostInformation
                |> Seq.choose (fun x ->
                    match x with
                    | Happy pi -> Some(pi)
                    | _ -> None)
                |> List.ofSeq
        }
