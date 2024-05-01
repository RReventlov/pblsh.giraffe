module pblsh.Workflows

open pblsh.Paths
open pblsh.Models

let persistPost post files = ()

module Posts = 
    let saveNewPost author title dots files  =
        Post.createNew author title dots
        |> deadEnd (persistPost files)
    
    