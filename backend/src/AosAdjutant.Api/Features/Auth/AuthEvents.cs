using System.Globalization;
using System.Security.Claims;
using AosAdjutant.Api.Common;
using AosAdjutant.Api.Features.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace AosAdjutant.Api.Features.Auth;

public static class AuthEvents
{
    public static async Task OnTokenValidated(TokenValidatedContext ctx)
    {
        // Save ID token so logout redirect works
        ctx.Properties!.StoreTokens([
            new() { Name = "id_token", Value = ctx.TokenEndpointResponse!.IdToken },
        ]);

        var principal = ctx.Principal!;
        var identity = principal.Identities.First();

        string RequireClaim(string type) =>
            principal.FindFirstValue(type)
            ?? throw new InvalidOperationException(
                $"Required claim: '{type}' missing from Pocket ID token."
            );

        var externalIdentity = new ExternalIdentity(
            Provider: "pocket-id",
            Subject: RequireClaim("sub"),
            Username: RequireClaim("preferred_username"),
            Email: RequireClaim("email")
        );

        var userService = ctx.HttpContext.RequestServices.GetRequiredService<UserService>();

        var user = await userService.ResolveExternalIdentity(externalIdentity);
        identity.AddClaim(
            new Claim(AppClaims.UserId, user.UserId.ToString(CultureInfo.InvariantCulture))
        );
    }

    public static Task OnRemoteFailure(RemoteFailureContext ctx)
    {
        var logger = ctx
            .HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()
            .CreateLogger("Authentication.OpenIdConnect");

        logger.Log_OIDC_failure(ctx.Failure);
        ctx.Response.Redirect("/error");
        ctx.HandleResponse();

        return Task.CompletedTask;
    }
}
