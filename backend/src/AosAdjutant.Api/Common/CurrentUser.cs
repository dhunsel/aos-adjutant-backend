using System.Globalization;
using System.Security.Claims;

namespace AosAdjutant.Api.Common;

public sealed class CurrentUser(IHttpContextAccessor accessor) : ICurrentUser
{
    public int? UserId =>
        int.TryParse(
            accessor.HttpContext?.User.FindFirstValue(AppClaims.UserId),
            CultureInfo.InvariantCulture,
            out var id
        )
            ? id
            : null;
}
