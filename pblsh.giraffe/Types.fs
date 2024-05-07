module pblsh.Types

let hasMinLength length str =
    not (System.String.IsNullOrEmpty str || str.Length < length)

type String5 = private String5 of string

module String5 =

    let create str =
        if hasMinLength 5 str then
            Ok(String5 str)
        else
            Error "string must contain 5 or more characters"

    let value (String5 str) = str

type String1 = private String1 of string

module String1 =
    
    let create str =
        if hasMinLength 1 str then
            Ok(String1 str)
        else
            Error "string must contain 1 or more characters"

    let value (String1 str) = str

type Int0 = private Int0 of int

module Int0 =
    let create i =
        if i < 0 then
            Error "value must be 0 or positive"
        else
            Ok(Int0 i)

    let value (Int0 i) = i

    let zero = Int0 0
