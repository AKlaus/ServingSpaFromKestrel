using AK.HostingSpa.Configuration;

namespace AK.HostingSpa.ClassicApi;

public class Startup
{
	// ReSharper disable once NotAccessedField.Local
	private readonly IConfiguration _configuration;

	public Startup(IConfiguration configuration)
	{
		_configuration = configuration;
	}

	/// <summary>
	///		This method gets called by the runtime to add services to the container.
	/// </summary>
	public void ConfigureServices(IServiceCollection services)
	{
		services.AddControllers();
		services.AddAndConfigureSwagger();
	}

	/// <summary>
	///		This method gets called by the runtime to configure the HTTP request pipeline.
	/// </summary>
	public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
	{
		if (env.IsDevelopment())
		{
			// CORS if needed for development only 
			// app.UseCors();
		}
		// Enforce use of HTTPS
		app.UseHsts();
		app.UseHttpsRedirection();

		app	.UseAppSwagger()
			.UseDeveloperExceptionPage();

		// Matches request to an endpoint
		app.UseRouting();

		app.UseSpaWithNoCache();
		// Add UseAuthentication() & UseAuthorization() here if required
		
		// Executes the matched endpoint
		app.UseEndpoints(endpoints => 
				endpoints.MapControllers() // Maps attributes on the controllers, like, [Route], [HttpGet], etc.
			);
	}
}