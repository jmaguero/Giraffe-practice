open System
open Giraffe
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Primitives
open Microsoft.Net.Http.Headers
open System.Text
open System.Threading.Tasks


[<CLIMutable>]
type MessageResponse = {
    Message: string
}
[<CLIMutable>]
type ErrorResponse = {
    Error: string
}

let Message = { Message =  "Hello world"}

let langHandler : HttpHandler =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        let handler =
            match ctx.TryGetQueryStringValue "lang" with
            | Some a when a.Length < 1 -> RequestErrors.badRequest (json {Message = "Lang can't be an empty string"})
            | Some l when l.Length > 2 -> RequestErrors.badRequest (json { Error = $"{l} is {l.Length} length and lang param only accepts ISO 639-1 codes"})
            | Some "en" -> json { Message = "Hello world"}
            | Some "de" -> json { Message = "Hallo welt"}
            | Some "sp" -> json { Message = "Hola mundo"}
            | Some unknownLanguage ->  
                let lang = $"{unknownLanguage} is not a supported language at the moment. Currently supported languages are: en (English), es (Spanish), fr (French), de (German)."
                let msg = { Error = lang }
                RequestErrors.badRequest (json msg)
            | None -> RequestErrors.badRequest (json {Message = "Language can't be empty"})
        handler next ctx

let myWebApp =
    choose [
        route "/" >=> langHandler
        route "/json"   >=> json Message
        route "/text"   >=> text "Hello World!"
    ]


// This is boilerplate to create an ASP .NET Core web application
let builder = WebApplication.CreateBuilder()
builder.Services.AddGiraffe() |> ignore
let app = builder.Build()
app.UseGiraffe myWebApp
app.Run()
