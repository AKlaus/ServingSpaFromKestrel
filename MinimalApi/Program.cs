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

// Map minimal API routes
app	.MapTestRoutes();

await app.RunAsync();