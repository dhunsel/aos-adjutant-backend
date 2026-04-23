using System.ComponentModel.DataAnnotations;

namespace AosAdjutant.Api.Features.Units;

public sealed record UnitResponseDto(
    int UnitId,
    string Name,
    int Health,
    string Move,
    int Save,
    int Control,
    int? WardSave,
    int FactionId,
    uint Version
);

public sealed record CreateUnitDto
{
    [StringLength(100, MinimumLength = 1)]
    public required string Name { get; init; }
    public required int Health { get; init; }

    [StringLength(100, MinimumLength = 1)]
    public required string Move { get; init; }
    public required int Save { get; init; }
    public required int Control { get; init; }
    public int? WardSave { get; init; }
}

public sealed record ChangeUnitDto
{
    [StringLength(100, MinimumLength = 1)]
    public required string Name { get; init; }
    public required int Health { get; init; }

    [StringLength(100, MinimumLength = 1)]
    public required string Move { get; init; }
    public required int Save { get; init; }
    public required int Control { get; init; }
    public int? WardSave { get; init; }
    public required uint Version { get; init; }
}
