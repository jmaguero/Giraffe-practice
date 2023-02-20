open System
open Giraffe
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Configuration


[<CLIMutable>]
type MessageResponse = {
    Message: string
}
[<CLIMutable>]
type ErrorResponse = {
    Error: string
}

let Message = { Message =  "Hello world"}

let uriLangHandler (lang : string): HttpHandler =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        // Read appsettings.json
        let configuration = ctx.GetService<IConfiguration>()
        // get lang section, see .json file
        let getSec = configuration.GetSection("Lang")
        let handler =
            match lang with
            | "en" -> json { Message = getSec.GetValue("en:msg")} // It is possible to add a type between <>: getSec.GetValue<string>("en:msg")}
            | "de"-> json { Message = getSec.GetValue<string>("de:msg")}
            | "es" -> json { Message = getSec.GetValue<string>("es:msg")}
            | x when x.Length > 2 || x.Length < 2 -> RequestErrors.badRequest (json {Message = "Language must be a ISO 639-1 value"})
            | xx ->
                let lang = $"{xx} is not a supported language at the moment. Currently supported languages are: en (English), es (Spanish), fr (French), de (German)."
                let msg = { Error = lang }
                RequestErrors.badRequest (json msg)
        handler next ctx

let myWebApp =
    choose [
        routef "/api/greet/%s" uriLangHandler

        // If none of the routes matched then return a 404
        RequestErrors.NOT_FOUND "Not Found"
    ]


// This is boilerplate to create an ASP .NET Core web application
let builder = WebApplication.CreateBuilder()
builder.Services.AddGiraffe() |> ignore
let app = builder.Build()
app.UseGiraffe myWebApp
app.Run()

