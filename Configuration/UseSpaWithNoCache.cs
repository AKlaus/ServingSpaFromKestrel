using System.Net;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Net.Http.Headers;

// ReSharper disable once CheckNamespace
namespace AK.HostingSpa.Configuration;

internal static partial class ServiceCollectionExtensions
{
	/// <summary>
	///		Adds the middleware pipeline for serving static files (SPA scripts, HTML and assets)
	/// </summary>
	/// <remarks>
	///		Here's the same pipeline for the Classic and Minimal API 
	/// </remarks>
	public static IApplicationBuilder UseSpaWithNoCache(this IApplicationBuilder app)
	{
		// List front-end files (in addition to 'index.html') that require to be served with no browser cache 
		string[] filesWithNoCache = [];
		
		// Serves static files under the "wwwroot" folder. Maps to the root route ("/")
		// Serves other than `index.html` files under the web root (by default `wwwroot`) folder.
		// Without this method Kestrel would return 'index.html' on all the requests for static content 
		app.UseStaticFiles(
			// Set cache expiration for all static files except 'index.html' and ones listed in `extraFilesWithNoCache`
			new StaticFileOptions
			{
				OnPrepareResponse = ctx =>
				{
					if (filesWithNoCache.Contains(ctx.File.Name, StringComparer.OrdinalIgnoreCase))
						SetNoCaching(ctx);
					else
					{
						var headers = ctx.Context.Response.GetTypedHeaders();
						headers.CacheControl = new CacheControlHeaderValue { MaxAge = TimeSpan.FromDays(12 * 30) };
					}
				}
			});
		
		// Handle unmatched API requests
		app.Use(async (context, next) =>
			{
				// Return 404 for unmatched API requests instead of falling through to SPA middleware
				// This handles cases where route constraints reject invalid parameters (e.g., non-numeric values for :int constraints)
				if (context.GetEndpoint() == null && context.Request.Path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase))
				{
					var problemDetails = new ProblemDetails
						{
							Status = StatusCodes.Status404NotFound,
							Title = "The requested API endpoint not found.",
							Instance = context.Request.Path
						};             
					context.Response.StatusCode = (int)problemDetails.Status;
					context.Response.ContentType = MediaTypeNames.Application.ProblemJson;
					await context.Response.WriteAsJsonAsync(problemDetails);
				}
				// Prevent HTTP 500 response on non-GET requests (e.g. POST, OPTIONS) to non-API end-points
				// See https://github.com/dotnet/aspnetcore/issues/5223#issuecomment-1135817359
				else if (context.GetEndpoint() == null && !HttpMethods.IsGet(context.Request.Method) && !HttpMethods.IsHead(context.Request.Method))
				{
					context.Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
					await context.Response.WriteAsync($"Not allowed HTTP method: {context.Request.Method}");
				}
				else
					await next();
			}
		);

		// Does 3 things:
		//	- Redirects all requests to the default page;
		//	- Serves 'index.html'
		//	- Tries to configure static files serving (falls back to UseSpaStaticFiles() and serving them from 'wwwroot')
		app.UseSpa(c => c.Options.DefaultPageStaticFileOptions = new StaticFileOptions { OnPrepareResponse = SetNoCaching });
		
		// Note: There's no need in calling UseDefaultFiles() prior to UseStaticFiles() as the docs insist (https://learn.microsoft.com/en-us/aspnet/core/fundamentals/static-files#serve-default-documents),
		// because UseSpa() extension does the same and allows to setup caching policies
		
		return app;
	}

	/// <summary>
	/// Disable caching (https://stackoverflow.com/q/49547/968003)
	/// The correct minimum set includes:
	///     Cache-Control: no-cache, no-store, must-revalidate, max-age=0
	/// </summary>
	private static void SetNoCaching(StaticFileResponseContext ctx)
	{
		var response = ctx.Context.Response;
		response.GetTypedHeaders().CacheControl =
			new
				CacheControlHeaderValue // Supersedes 'Pragma' header (https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Pragma)
				{
					NoCache = true,
					NoStore = true,
					MustRevalidate = true,
					MaxAge = TimeSpan.Zero // Overtakes 'Expires: 0' (https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Expires)
				};
	}
}