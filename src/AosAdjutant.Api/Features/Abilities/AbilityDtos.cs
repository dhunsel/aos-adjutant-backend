using System.ComponentModel.DataAnnotations;

namespace AosAdjutant.Api.Features.Abilities;

public sealed record AbilityResponseDto(
    int AbilityId,
    string Name,
    string? Reaction,
    string? Declaration,
    string Effect,
    TurnPhase Phase,
    ActivationRestriction? Restriction,
    PlayerTurn? Turn,
    uint Version
);

public sealed record CreateAbilityDto
{
    [StringLength(100, MinimumLength = 1)] public required string Name { get; init; }
    [StringLength(100, MinimumLength = 1)] public string? Reaction { get; init; }
    [StringLength(100, MinimumLength = 1)] public string? Declaration { get; init; }
    [StringLength(100, MinimumLength = 1)] public required string Effect { get; init; }
    public required TurnPhase Phase { get; init; }
    public ActivationRestriction? Restriction { get; init; }
    public PlayerTurn? Turn { get; init; }
}

public sealed record ChangeAbilityDto
{
    [StringLength(100, MinimumLength = 1)] public required string Name { get; init; }
    [StringLength(100, MinimumLength = 1)] public string? Reaction { get; init; }
    [StringLength(100, MinimumLength = 1)] public string? Declaration { get; init; }
    [StringLength(100, MinimumLength = 1)] public required string Effect { get; init; }
    public required TurnPhase Phase { get; init; }
    public ActivationRestriction? Restriction { get; init; }
    public PlayerTurn? Turn { get; init; }
    public required uint Version { get; init; }
}
