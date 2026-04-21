using System.ComponentModel.DataAnnotations;

namespace AosAdjutant.Api.Features.BattleFormations;

public sealed record BattleFormationResponseDto(
    int BattleFormationId,
    string Name,
    int FactionId,
    uint Version
);

public sealed record CreateBattleFormationDto
{
    [StringLength(100, MinimumLength = 1)]
    public required string Name { get; init; }
}

public sealed record ChangeBattleFormationDto
{
    [StringLength(100, MinimumLength = 1)]
    public required string Name { get; init; }
    public required uint Version { get; init; }
}
