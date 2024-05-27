module pblsh.Urls

open System
open pblsh.Models
open pblsh.Types

let newPost = "/posts/new"

let dotUrl dot = sprintf "/dots/%s" (String1.value dot)

let userUrl (postInfo: PostInformation) = sprintf "/users/%O" postInfo.AuthorId
let userUrlComment (commentInfo: CommentInformation) = sprintf "/users/%O" commentInfo.AuthorId
let postUrl (postInfo: PostInformation) = sprintf "/posts/%s" (postInfo.Id.ToString())
