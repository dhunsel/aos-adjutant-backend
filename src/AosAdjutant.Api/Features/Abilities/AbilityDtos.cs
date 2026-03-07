namespace AosAdjutant.Api.Features.Abilities;

public record AbilityResponseDto(
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

public record CreateAbilityDto(
    string Name,
    string? Reaction,
    string? Declaration,
    string Effect,
    TurnPhase Phase,
    ActivationRestriction? Restriction,
    PlayerTurn? Turn
);

public record ChangeAbilityDto(
    string Name,
    string? Reaction,
    string? Declaration,
    string Effect,
    TurnPhase Phase,
    ActivationRestriction? Restriction,
    PlayerTurn? Turn,
    uint Version
);
