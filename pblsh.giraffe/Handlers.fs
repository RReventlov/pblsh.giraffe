module pblsh.Handlers

open Giraffe
open pblsh.Models

let getIndex () =
    let view = Views.index ()
    htmlView view

let getLogin () =
    let view = Views.login ()
    htmlView view

let postLogin () = text "postHandler"

let getSignup () =
    let view = Views.signup ()
    htmlView view
    
let postSignup (uncheckedSignUpInfo: UncheckedSignUpInfo) = 
    text uncheckedSignUpInfo.Email