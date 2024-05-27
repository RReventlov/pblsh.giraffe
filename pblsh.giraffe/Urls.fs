module pblsh.Urls

open System
open pblsh.Models
open pblsh.Types

let newPost = "/posts/new"

let dotUrl dot = sprintf "/dots/%s" (String1.value dot)

let userUrl (postInfo: PostInformation) = sprintf "/users/%s" (postInfo.AuthorId.ToString())
let userUrlComment (commentInfo: CommentInformation) = sprintf "/users/%s" (commentInfo.AuthorId.ToString())
let postUrl (postInfo: PostInformation) = sprintf "/posts/%s" (postInfo.Id.ToString())