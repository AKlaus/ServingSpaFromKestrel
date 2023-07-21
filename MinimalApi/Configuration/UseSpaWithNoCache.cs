using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Net.Http.Headers;

namespace AK.HostingSpa.MinimalApi.Configuration;

internal static partial class ServiceCollectionExtensions
{
	public static void UseSpaWithNoCache(this IApplicationBuilder app)
	{
		// List front-end files (in addition to 'index.html') that require to be served with no browser cache 
		var filesWithNoCache = Array.Empty<string>();
		
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

		// Does 3 things:
		//	- Redirects all requests to the default page;
		//	- Serves 'index.html'
		//	- Tries to configure static files serving (falls back to UseSpaStaticFiles() and serving them from 'wwwroot')
		app.UseSpa(c =>c.Options.DefaultPageStaticFileOptions = new StaticFileOptions  { OnPrepareResponse = SetNoCaching});
		
		// Note: There's no need in calling UseDefaultFiles() prior to UseStaticFiles() as the docs insist (https://learn.microsoft.com/en-us/aspnet/core/fundamentals/static-files#serve-default-documents),
		// because UseSpa() extension does the same and allows to setup caching policies 
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