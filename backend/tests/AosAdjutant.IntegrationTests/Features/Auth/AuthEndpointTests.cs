using System.Net;
using System.Net.Http.Json;
using AosAdjutant.Api.Features.Auth;
using AosAdjutant.Api.Features.Factions;
using AosAdjutant.IntegrationTests.Fixture;
using Microsoft.AspNetCore.Mvc.Testing;

namespace AosAdjutant.IntegrationTests.Features.Auth;

public class AuthEndpointTests(ApiFactory factory) : EndpointTestsBase(factory)
{
    // LocalRedirect on logout returns a 302; the base Client auto-follows it.
    private readonly HttpClient _noRedirectClient = factory.CreateClient(
        new WebApplicationFactoryClientOptions { AllowAutoRedirect = false }
    );

    private static HttpRequestMessage Request(
        HttpMethod method,
        string url,
        string? authMode = null
    )
    {
        var request = new HttpRequestMessage(method, url);
        if (authMode is not null)
            request.Headers.Add(TestAuthHandler.AuthHeader, authMode);
        return request;
    }

    [Fact]
    public async Task Me_Returns200_WithUsernameAndIsAdmin()
    {
        var response = await Client.SendAsync(Request(HttpMethod.Get, "/api/auth/me"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<CurrentUserResponseDto>(JsonOptions);
        Assert.NotNull(body);
        Assert.Equal("integration-test", body.Username);
        Assert.True(body.IsAdmin);
    }

    [Fact]
    public async Task Me_Returns200_WithIsAdminFalse_WhenNonAdmin()
    {
        var response = await Client.SendAsync(
            Request(HttpMethod.Get, "/api/auth/me", authMode: "user")
        );

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<CurrentUserResponseDto>(JsonOptions);
        Assert.NotNull(body);
        Assert.Equal("integration-test", body.Username);
        Assert.False(body.IsAdmin);
    }

    [Fact]
    public async Task Me_Returns401_WhenAnonymous()
    {
        var response = await Client.SendAsync(
            Request(HttpMethod.Get, "/api/auth/me", authMode: "anonymous")
        );

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ProtectedEndpoint_Returns401_WhenAnonymous()
    {
        var response = await Client.SendAsync(
            Request(HttpMethod.Get, "/api/factions", authMode: "anonymous")
        );

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task RequireAdminEndpoint_Returns403_WhenNonAdmin()
    {
        var request = Request(HttpMethod.Post, "/api/factions", authMode: "user");
        request.Content = JsonContent.Create(
            new CreateFactionDto
            {
                Name = "Stormcast Eternals",
                GrandAlliance = GrandAlliance.Order,
            },
            options: JsonOptions
        );

        var response = await Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Logout_RedirectsToRoot_WhenAnonymous()
    {
        var response = await _noRedirectClient.SendAsync(
            Request(HttpMethod.Get, "/api/auth/logout", authMode: "anonymous")
        );

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        Assert.EndsWith("/", response.Headers.Location.OriginalString);
    }
}
