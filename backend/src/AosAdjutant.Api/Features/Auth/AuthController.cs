using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AosAdjutant.Api.Features.Auth;

[Route("auth")]
[ApiController]
[Tags("Auth")]
public sealed class AuthController() : ControllerBase
{
    [HttpGet("login")]
    [AllowAnonymous]
    public IActionResult Login()
    {
        return Challenge(
            new AuthenticationProperties { RedirectUri = "/" },
            OpenIdConnectDefaults.AuthenticationScheme
        );
    }

    [HttpGet("logout")]
    [AllowAnonymous]
    public IActionResult Logout()
    {
        if (User.Identity?.IsAuthenticated != true)
            return LocalRedirect("/");

        return SignOut(
            new AuthenticationProperties { RedirectUri = "/" },
            CookieAuthenticationDefaults.AuthenticationScheme,
            OpenIdConnectDefaults.AuthenticationScheme
        );
    }

    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType<CurrentUserResponseDto>(StatusCodes.Status200OK)]
    public ActionResult<CurrentUserResponseDto> Me()
    {
        return Ok(new CurrentUserResponseDto(User.Identity!.Name!, User.IsInRole("admins")));
    }
}
