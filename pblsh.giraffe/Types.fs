module pblsh.Types

type String5 = private String5 of string

module String5 =

    let create str =
        if System.String.IsNullOrEmpty str || str.Length < 5 then
            Error "string must contain 5 or more characters"
        else
            Ok(String5 str)

    let value (String5 str) = str

type Int0 = private Int0 of int

module Int0 =
    let create i =
        if i < 0 then
            Error "value must be 0 or positive"
        else
            Ok(Int0 i)

    let value (Int0 i) = i
    
    let zero = Int0 0
