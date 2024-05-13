module pblsh.Helper

open System.Threading.Tasks

let await (x: Task<_>) = x.Result