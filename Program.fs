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


let langHandler =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        // Read appsettings.json
        let configuration = ctx.GetService<IConfiguration>()
        // get lang section, see .json file
        let getSec = configuration.GetSection("Lang")
        // match query, prob there's a more neat way to implement this
        let handler =
            printfn "%A" ctx.GetEndpoint
            match ctx.TryGetQueryStringValue "lang" with
            | Some a when a.Length < 1 -> RequestErrors.badRequest (json {Message = "Lang can't be an empty string"})
            | Some l when l.Length > 2 -> RequestErrors.badRequest (json { Error = $"{l} is {l.Length} length and lang param only accepts ISO 639-1 codes"})
            | Some "en" -> text $"{ctx.GetEndpoint}" // It is possible to add a type between <>: getSec.GetValue<string>("en:msg")}
            | Some "de" -> json { Message = getSec.GetValue<string>("de:msg")}
            | Some "es" -> json { Message = getSec.GetValue<string>("es:msg")}
            | Some unknownLanguage ->  
                let lang = $"{unknownLanguage} is not a supported language at the moment. Currently supported languages are: en (English), es (Spanish), fr (French), de (German)."
                let msg = { Error = lang }
                RequestErrors.badRequest (json msg)
            | None -> RequestErrors.badRequest (json {Message = "Language can't be empty"})        
        handler next ctx


let myWebApp =
    choose [
        route "/api/greet" >=> langHandler
        RequestErrors.NOT_FOUND "Not Found" 
    ]


// This is boilerplate to create an ASP .NET Core web application
let builder = WebApplication.CreateBuilder()
builder.Services.AddGiraffe() |> ignore
let app = builder.Build()
app.UseGiraffe myWebApp
app.Run()

