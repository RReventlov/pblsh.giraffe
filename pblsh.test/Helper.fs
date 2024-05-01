module pblsh.test.Helper

let tupledData (l: ('a * 'b) list) =
    l |> List.map (fun (x, y) -> [| x :> obj; y :> obj |])
    
let combineData (l1: (string * bool) list) (l2: (string * bool) list) =
    seq{
        for (query1,expected1) in l1 do
            for (query2,expected2) in l2 do
                ((sprintf "%s%s" query1 query2 ), expected1 && expected2)
    } |> List.ofSeq
