using AK.HostingSpa.MinimalApi.Configuration;
using AK.HostingSpa.MinimalApi.Routes;

var builder = WebApplication.CreateBuilder(args);

var isLocal = builder.Environment.IsDevelopment() || builder.Environment.IsEnvironment("Local");

builder.Services.AddEndpointsApiExplorer()
				.AddAndConfigureSwagger();

// Configure App
var app = builder.Build();

if (isLocal)
	app	.UseAppSwagger()
		.UseDeveloperExceptionPage();

app	.UseHttpsRedirection()
	.UseSpaWithNoCache();
	// Add UseAuthentication() & UseAuthorization() here if required

// Map minimal API routes
app	.MapTestRoutes();

await app.RunAsync();