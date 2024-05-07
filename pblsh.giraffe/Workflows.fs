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
        ()


    let saveNewPost author title dots files =
        path {
            let! post = Post.createNew author title dots
            persistPost post files
            return post
        }
