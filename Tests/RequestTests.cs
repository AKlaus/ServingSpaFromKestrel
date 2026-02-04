using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace AK.HostingSpa.Tests;

/// <summary>
///		Tests for API calls 
/// </summary>
public abstract class ApiRequestTestsBase<TProgram> : IClassFixture<WebApplicationFactory<TProgram>> where TProgram : class
{
	private readonly HttpClient _client;

	protected ApiRequestTestsBase(WebApplicationFactory<TProgram> factory)
	{
		_client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
	}

	[Fact]
	public async Task Valid_Get_Api_Request_ReturnsNoContent()
	{
		var response = await _client.GetAsync("/api/test");

		Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
	}

	[Theory]
	[InlineData("/api/../../../etc/passwd")]
	[InlineData("/api/..")]
	public async Task NonExistent_NonApi_Route_Returns_Spa(string path)
	{
		var response = await _client.GetAsync(path);

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
	}

	[Theory]
	[InlineData("POST")]
	[InlineData("PUT")]
	[InlineData("DELETE")]
	[InlineData("PATCH")]
	[InlineData("OPTIONS")]
	[InlineData("HEAD")]
	public async Task Wrong_Http_Method_For_Api_Endpoint_Returns_MethodNotAllowed(string method)
	{
		var request = new HttpRequestMessage(new HttpMethod(method), "/api/test");

		var response = await _client.SendAsync(request);

		Assert.Equal(HttpStatusCode.MethodNotAllowed, response.StatusCode);
	}
	
	[Theory]
	[InlineData("/api/nonexistent")]
	[InlineData("/api/test%20test")]
	[InlineData("/api/test/日本語")]	// Special characters
	[InlineData("/api/")]				// Boundary paths
	[InlineData("/api//test")]
	public async Task NonExistent_Endpoint_Returns_NotFound(string path)
	{
		var response = await _client.GetAsync(path);

		Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
	}

	[Theory]
	[InlineData("?id=1'")]
	[InlineData("?id=1%20OR%201=1")]
	[InlineData("?x=<script>alert(1)</script>")]
	[InlineData("?cmd=|cat%20/etc/passwd")]
	public async Task Malformed_Query_Strings_To_Api_Method_Ignored_Returns_NoContent(string queryString)
	{
		var response = await _client.GetAsync($"/api/test{queryString}");

		Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
	}

	[Theory]
	[InlineData("Accept", "!!!invalid!!!")]
	[InlineData("Accept-Encoding", "invalid-encoding")]
	[InlineData("Authorization", "Bearer invalid")]
	public async Task Malformed_Headers__Ignored_Returns_NoContent(string headerName, string headerValue)
	{
		var request = new HttpRequestMessage(HttpMethod.Get, "/api/test");
		request.Headers.TryAddWithoutValidation(headerName, headerValue);

		var response = await _client.SendAsync(request);

		Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
	}
}

public class ClassicApiRequestTests(WebApplicationFactory<ClassicApi.Program> factory)
	: ApiRequestTestsBase<ClassicApi.Program>(factory);

public class MinimalApiRequestTests(WebApplicationFactory<MinimalApi.Program> factory)
	: ApiRequestTestsBase<MinimalApi.Program>(factory);
