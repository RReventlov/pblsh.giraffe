module pblsh.Paths

type Path<'s, 'f> =
    | Happy of 's
    | Sad of 'f


let mapResult r =
    match r with
    | Ok o -> Happy o
    | Error e -> Sad e

let lift fn input =
    match input with
    | Happy s -> Happy(fn s)
    | Sad f -> Sad f

let (|>^) input fn = lift fn input

let bind fn input =
    match input with
    | Happy s -> fn s
    | Sad f -> Sad f

let (|>!) input fn = bind input fn

let deadEnd fn input =
    fn input
    input

let passBy fn input =
    fn ()
    input

let mapSad fn input =
    match input with
    | Happy h -> Happy h
    | Sad s -> Sad(fn s)

let combine combineHappy combineSad fn1 fn2 =
    let fn input = 
        match (fn1 input), (fn2 input) with
        | Happy h1, Happy h2 -> Happy(combineHappy h1 h2)
        | Happy _, Sad s2 -> Sad s2
        | Sad s1, Happy _ -> Sad s1
        | Sad s1, Sad s2 -> Sad(combineSad s1 s2)
    
    fn
    
let allOf combineHappy combineSad fnL = List.reduce (combine combineHappy combineSad) fnL