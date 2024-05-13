module pblsh.Paths

open pblsh.Types

type Path<'h, 's> =
    | Happy of 'h
    | Sad of 's

let mapR r =
    match r with
    | Ok o -> Happy o
    | Error e -> Sad e

let filterHappy path =
    match path with
    | Happy h -> true
    | Sad _ -> false

let bind fn input =
    match input with
    | Happy s -> fn s
    | Sad f -> Sad f

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

let returnHappy = Happy

let mergePaths p1 p2 =
    match p1, p2 with
    | Happy h1, Happy h2 -> Happy(h1, h2)
    | Sad s1, Sad s2 -> Sad [ s1; s2 ]
    | Sad s1, Happy _ -> Sad [ s1 ]
    | Happy _, Sad s2 -> Sad [ s2 ]

let mergeWithPathList pl p1 =
    match p1, pl with
    | Happy h1, Happy h2 -> Happy(h1 :: h2)
    | Sad s1, Sad s2 -> Sad(s1 :: s2)
    | Sad s1, Happy _ -> Sad [ s1 ]
    | Happy _, Sad s2 -> Sad s2

let collect paths =
    paths |> List.fold mergeWithPathList (Happy [])

type PathBuilder() =
    member _.Bind(x, f) = bind f x
    member _.Return x = Happy x
    member _.ReturnFrom x = x
    member _.MergeSources(p1, p2) = mergePaths p1 p2

let path = PathBuilder()

let (<|?) path fn = mapSad fn path

type Errors =
    | Error1 of string
    | Error2 of int

let ``test path computation expression`` () =
    let r =
        path {
            let! str1 = (String5.create >> mapR) "tests" <|? Error1
            and! str2 = (String1.create >> mapR) "gaaer" <|? Error1
            return {| First = str1; Second = str2 |}
        }

    printfn "%A" r
    ()
