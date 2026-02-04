[![Build](https://github.com/AKlaus/ServingSpaFromKestrel/actions/workflows/build.yml/badge.svg)](https://github.com/AKlaus/ServingSpaFromKestrel/actions/workflows/build.yml)
[![Coverage Status](https://coveralls.io/repos/github/AKlaus/ServingSpaFromKestrel/badge.svg?branch=main)](https://coveralls.io/github/AKlaus/ServingSpaFromKestrel?branch=main)

> Code samples for "[Hosting SPA + .NET API solution](https://alex-klaus.com/hosting-spa-in-dotnet/)" article.

# SPA hosted by .NET API with a caching strategy

Example of WebAPI projects (ASP.NET) servicing a SPA front-end along with the API and Swagger/OpenAPI interface, and managing cache settings of the front-end assets.

| Route                                                      | Handler                                                          |
|------------------------------------------------------------|------------------------------------------------------------------|
| Root (e.g. `/`) or `/index.html`                           | Returns `index.html` with specified caching headers              |
| Any static file, e.g. `styles.css`                         | Returns the requested static file with specified caching headers |
| Any unknown route, e.g. a SPA page or unknown page/resource | Returns `index.html` to let the SPA to show the "404 Not Found"  |
| `/swagger/*`                                               | The Swagger/OpenAPI interface                                    |
| API end-points, e.g. `/api/*`                              | Processed by the API controllers/routes                          | 

## Folder structure
The key elements of the solution:
- `ClassicApi` – a WebAPI project with classic controllers servicing the front-end from `wwwroot`.
- `MinimalApi` – a WebAPI project built with [minimal API](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis) servicing the front-end from `wwwroot`.
- `Configuration` – helper methods used by both projects to configure the middleware for Swagger and SPA. 
- `wwwroot` – a sample SPA front-end with two routes serviced by the back-end project.
- `Tests` – automates tests for valid and invalid calls to the API, Swagger and SPA.
