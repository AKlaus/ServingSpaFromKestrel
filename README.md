# SPA hosted by .NET Web API

> Here's a [blog post](https://alex-klaus.com/hosting-spa-in-dotnet/) explaining the magic sauce.

Example of WebAPI projects (ASP.NET) servicing a SPA front-end along with the API and Swagger/OpenAPI interface, and managing cache settings of the front-end assets.

| Route                              | Handler                       |
|------------------------------------|-------------------------------|
| API end-points                     | API controllers/routes        | 
| `/swagger/*`                       | The Swagger/OpenAPI interface |
| Root, e.g. `/`                     | Returns `index.html`          |
| Any static file, e.g. `styles.css` | Returns the requested static file |
| Any other (unknown) route          | Returns `index.html`          |

## Folder structure
The key elements of the solution:
- `wwwroot` – a sample SPA front-end serviced by the projects.
- `ClassicApi` – a WebAPI project with classic controllers servicing the front-end from `wwwroot`.
- `MinimalApi` – a WebAPI project built with [minimal API](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis) servicing the front-end from `wwwroot`.
- `Configuration` – helper methods used by both projects to configure the middleware for Swagger and SPA. 
