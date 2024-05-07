module pblsh.Workflows

open System.IO
open Microsoft.AspNetCore.Http
open pblsh.Types
open pblsh.Paths
open pblsh.Configuration
open pblsh.Models
open pblsh.DataAccess

module Posts =

    let postDir post = sprintf "%s/%O" postRoot post.Id

    let persistPost post (files: IFormFileCollection) =
        let ctx = sql.GetDataContext()
        let posts = ctx.Pblsh.Posts
        let row = posts.Create()
        row.Id <- post.Id.ToString()
        row.Author <- post.Author
        row.Title <- (String5.value post.Title)
        row.PublishedOn <- post.PublishedOn
        row.UniqueViews <- (Int0.value post.UniqueViews)
        ctx.SubmitUpdates()
        
        let target = Directory.CreateDirectory(postDir post)

        for file in files do
            let path = sprintf "%s/%s" target.FullName "my-file.txt"
            let fs = File.Open(path, FileMode.OpenOrCreate)
            file.CopyTo(fs)
            fs.Dispose()


    let saveNewPost author title dots files =
        path {
            let! post = Post.createNew author title dots
            persistPost post files
            return post
        }
