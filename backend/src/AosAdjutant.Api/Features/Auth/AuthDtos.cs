namespace AosAdjutant.Api.Features.Auth;

public sealed record CurrentUserResponseDto(string Username, bool IsAdmin);
