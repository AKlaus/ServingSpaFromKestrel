using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace AK.HostingSpa.Tests;

/// <summary>
///		Tests for SPA and Swagger calls 
/// </summary>
public abstract class SpaRequestTestsBase<TProgram> : IClassFixture<WebApplicationFactory<TProgram>>
	where TProgram : class
{
	private readonly HttpClient _client;

	protected SpaRequestTestsBase(WebApplicationFactory<TProgram> factory)
	{
		_client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
	}

	[Fact]
	public async Task Valid_Get_Request_For_Spa_ReturnsOk()
	{
		var response = await _client.GetAsync("/");

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
	}

	[Theory]
	[InlineData("POST")]
	[InlineData("PUT")]
	[InlineData("DELETE")]
	[InlineData("PATCH")]
	[InlineData("OPTIONS")]
	public async Task Wrong_Http_Method_For_Spa_Returns_MethodNotAllowed(string method)
	{
		var request = new HttpRequestMessage(new HttpMethod(method), "/");

		var response = await _client.SendAsync(request);

		Assert.Equal(HttpStatusCode.MethodNotAllowed, response.StatusCode);
	}

	[Theory]
	[InlineData("/swagger/v1/swagger.json", "GET")]
	[InlineData("/swagger/index.html", "GET")]
	public async Task Swagger_Returns_OK(string path, string method)
	{
		var request = new HttpRequestMessage(new HttpMethod(method), path);

		var response = await _client.SendAsync(request);

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
	}
}

public class ClassicApiSpaRequestTests(WebApplicationFactory<ClassicApi.Program> factory)
	: SpaRequestTestsBase<ClassicApi.Program>(factory);

public class MinimalApiSpaRequestTests(WebApplicationFactory<MinimalApi.Program> factory)
	: SpaRequestTestsBase<MinimalApi.Program>(factory);
