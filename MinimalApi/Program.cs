using AK.HostingSpa.Configuration;
using AK.HostingSpa.MinimalApi.Routes;

var builder = WebApplication.CreateBuilder(args);

var isLocal = builder.Environment.IsDevelopment() || builder.Environment.IsEnvironment("Local");

// Configure Open API / Swagger
builder.Services.AddEndpointsApiExplorer()
				.AddAndConfigureSwagger();

// Configure the HTTP request pipeline
var app = builder.Build();

if (!isLocal)
{
	// CORS if needed for development only 
	// app.UseCors();
}

// Enforce use of HTTPS
app	.UseHsts()
	.UseHttpsRedirection();

if (isLocal)
	app	.UseAppSwagger()
		.UseDeveloperExceptionPage();

app	.UseSpaWithNoCache();
// Add UseAuthentication() & UseAuthorization() here if required

// Map minimal API routes
app	.MapTestRoutes();

await app.RunAsync();