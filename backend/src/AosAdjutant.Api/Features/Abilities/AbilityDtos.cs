using System.ComponentModel.DataAnnotations;
using AosAdjutant.Api.Common;

namespace AosAdjutant.Api.Features.Abilities;

public sealed record AbilityResponseDto(
    int AbilityId,
    string Name,
    string? Reaction,
    string? Declaration,
    string Effect,
    Phase Phase,
    Restriction? Restriction,
    Turn? Turn,
    uint Version
);

public sealed record CreateAbilityDto
{
    [StringLength(100, MinimumLength = 1)]
    public required string Name { get; init; }

    [StringLength(100, MinimumLength = 1)]
    public string? Reaction { get; init; }

    [StringLength(100, MinimumLength = 1)]
    public string? Declaration { get; init; }

    [StringLength(100, MinimumLength = 1)]
    public required string Effect { get; init; }
    public required Phase Phase { get; init; }
    public Restriction? Restriction { get; init; }
    public Turn? Turn { get; init; }
}

public sealed record ChangeAbilityDto
{
    [StringLength(100, MinimumLength = 1)]
    public required string Name { get; init; }

    [StringLength(100, MinimumLength = 1)]
    public string? Reaction { get; init; }

    [StringLength(100, MinimumLength = 1)]
    public string? Declaration { get; init; }

    [StringLength(100, MinimumLength = 1)]
    public required string Effect { get; init; }
    public required Phase Phase { get; init; }
    public Restriction? Restriction { get; init; }
    public Turn? Turn { get; init; }
    public required uint Version { get; init; }
}

public sealed record AbilityQuery : PagedQuery
{
    public Phase? Phase { init; get; }
}
