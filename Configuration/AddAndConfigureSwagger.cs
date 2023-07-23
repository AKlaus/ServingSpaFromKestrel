using System.Reflection;
using Microsoft.OpenApi.Models;

// ReSharper disable once CheckNamespace
namespace AK.HostingSpa.Configuration;

internal static partial class ServiceCollectionExtensions
{
	public static IServiceCollection AddAndConfigureSwagger(this IServiceCollection services)
		=> services.AddSwaggerGen(options =>
		{
			options.SwaggerDoc("v1", new OpenApiInfo { Title = "Hosting SPA Test API", Version = "v1" });
			options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
		});

	public static IApplicationBuilder UseAppSwagger(this IApplicationBuilder app)
		=> app.UseSwagger()
			  .UseSwaggerUI(c =>
				{
					c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hosting SPA Test API v1");
				});
}