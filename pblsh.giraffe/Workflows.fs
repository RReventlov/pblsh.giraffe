module pblsh.Workflows

open System
open System.IO
open System.Threading.Tasks
open Microsoft.AspNetCore.Http
open Microsoft.FSharp.Control
open SqlHydra.Query
open pblsh.Hydra
open pblsh.Hydra.main
open pblsh.Paths
open pblsh.Configuration
open pblsh.Models
open pblsh.DataAccess
open pblsh.Types
open pblsh.Models.Forms
open pblsh.Helper

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
            
    let persistDots (newDots: string list) (postId: string) =
        let dotsMaybe = 
            newDots 
            |> List.map (fun i -> 
                {
                    dots.Dot = i
                    dots.UsedBy = postId 
                }
            )
            |> AtLeastOne.tryCreate

        match dotsMaybe with
        | Some newDots ->
            insertTask createDbx {
                into dots
                entities newDots
                } :> Task
        | None ->
            async {
            printfn "Skipping insert because entities seq was empty."
            } |> Async.StartAsTask :> Task
            
            
    let saveNewPost author title dots files =
        path {
            let! post = Post.createNew author title dots
            persistPost post files
            persistDots dots (post.Id.ToString()) |> ignore
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

    let createPostInformation ((id, author, authorId, title): string * string * string * string) =
        PostInformation.create id author authorId title (dotsFor id)

    let getTop n =
        task {
            let! results =
                selectTask HydraReader.Read createDbx {
                    for p in posts do
                        join u in AspNetUsers on (p.Author = u.Id)
                        select (p.Id, u.UserName, u.Id, p.Title)
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

    type GetPostError =
        | PostNotFound
        | PostCouldNotBeCreated
    
    let getPost id =
        let idString = id.ToString()
        task {
            let! opt =
                selectTask HydraReader.Read createDbx {
                    for p in posts do
                        join u in AspNetUsers on (p.Author = u.Id)
                        where (p.Id = idString)
                        select (p.Id, u.UserName, u.Id, p.Title)
                        tryHead
                }
            
            return
                match opt with
                | Some (id, author, authorId, title) ->
                    match PostInformation.create id author authorId title (dotsFor idString) with
                    | Happy pi -> Happy pi
                    | Sad _ -> Sad(PostCouldNotBeCreated)
                | None -> Sad(PostNotFound)
        }
        
    let getPostsByAuthor (id:Guid) =
        let idStr = id.ToString()
        task {
            let! selectedArticles =
                selectTask HydraReader.Read createDbx {
                    for p in posts do
                        where (p.Author = idStr)
                        select p.Id
                        
                }
            return selectedArticles
               |> Seq.map (getPost >> await)
               |> Seq.filter filterHappy
               |> Seq.map(fun path ->
                   match path with //Sad Case wont happen
                   | Happy s -> s)
               |> List.ofSeq
        }
    
    let getPostsByDot (dot:string) =
        task {
            let! selectedArticles =
                selectTask HydraReader.Read createDbx {
                    for d in dots do
                        join p in posts on (d.UsedBy = p.Id)
                        where (d.Dot = dot)
                        select p.Id
                }
            return selectedArticles
               |> Seq.map (getPost >> await)
               |> Seq.filter filterHappy
               |> Seq.map(fun path ->
                   match path with //Sad Case wont happen
                   | Happy s -> s)
               |> List.ofSeq
        }
        
    let getContent id =
        let dir = postDir id
        
        if DirectoryInfo(dir).Exists then
            match DirectoryInfo(dir).GetFiles() |> Array.tryHead with
            | Some file -> Happy(File.ReadAllText(file.FullName))
            | None -> Sad "couldn't read file"
        else
            Sad "couldn't find file"
    
module Users =
    let getUser (pId: Guid) =
        let idStr = pId.ToString ()
        task {
            let! userSelectTask =
                selectTask HydraReader.Read createDbx {
                    for u in AspNetUsers do
                        where (u.Id = idStr)
                        select (u.UserName,u.Id)
                        tryHead
                }
            return
                match userSelectTask with
                |Some (userName,id) -> {UserName = userName ; Id = Guid.Parse(id) }
                |None -> {UserName = ""; Id = Guid.Empty}
        }
module Search =
    let searchPosts (queryInfo: QueryInfo) =
        task {
            let authorValue = "%"+(if queryInfo.Author.IsSome then queryInfo.Author.Value else "").ToUpper()+"%"
            let articleValue = "%"+queryInfo.Article+"%"
            let! fittingPosts = selectTask HydraReader.Read createDbx {
                    for p in posts do
                        join u in AspNetUsers on (p.Author = u.Id)
                        where (p.Title =% articleValue && u.NormalizedUserName =% authorValue)
                        select p.Id
                   }
            return fittingPosts
                   |> Seq.map (Posts.getPost >> await)
                   |> Seq.filter filterHappy
                   |> Seq.map(fun path ->
                       match path with //Sad Case wont happen
                       | Happy s -> s)
                   |> List.ofSeq
        }
        
               
        