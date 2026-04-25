using System.ComponentModel.DataAnnotations;

namespace AosAdjutant.Api.Features.Factions;

public sealed record FactionResponseDto(
    int FactionId,
    string Name,
    GrandAlliance GrandAlliance,
    uint Version
);

public sealed record CreateFactionDto
{
    [StringLength(100, MinimumLength = 1)]
    public required string Name { get; init; }
    public required GrandAlliance GrandAlliance { get; init; }
}

public sealed record ChangeFactionDto
{
    [StringLength(100, MinimumLength = 1)]
    public required string Name { get; init; }
    public required GrandAlliance GrandAlliance { get; init; }
    public required uint Version { get; init; }
}

public sealed record FactionQueryFilter
{
    public GrandAlliance? GrandAlliance { get; init; }
}
