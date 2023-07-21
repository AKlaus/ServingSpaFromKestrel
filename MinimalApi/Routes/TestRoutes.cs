namespace AK.HostingSpa.MinimalApi.Routes;

internal static partial class RoutesExtension
{
	public static IEndpointRouteBuilder MapTestRoutes(this IEndpointRouteBuilder app)
	{
		app	.MapGet("test", Results.NoContent)
			.WithSummary("Test end-point");

		return app;
	}
}