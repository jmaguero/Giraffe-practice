open Giraffe
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Primitives
open Microsoft.Net.Http.Headers
open System.Text
open System.Threading.Tasks
open Thoth

[<CLIMutable>]
type MessageResponse = {
    Message: string
}

let Message = { Message =  "Hello world" }

let myWebApp =
    choose [
        route "/json"   >=> json Message
        route "/text"   >=> text "Hello World!"
    ]






// This is boilerplate to create an ASP .NET Core web application
let builder = WebApplication.CreateBuilder()
builder.Services.AddGiraffe() |> ignore
let app = builder.Build()
app.UseGiraffe myWebApp
app.Run()
