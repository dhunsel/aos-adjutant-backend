using System.ComponentModel.DataAnnotations;

namespace AosAdjutant.Api.Features.Units;

public record UnitResponse(
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

public record CreateUnitDto(
    [StringLength(100, MinimumLength = 1)] string Name,
    int Health,
    [StringLength(100, MinimumLength = 1)] string Move,
    int Save,
    int Control,
    int? WardSave
);

public record ChangeUnitDto(
    [StringLength(100, MinimumLength = 1)] string Name,
    int Health,
    [StringLength(100, MinimumLength = 1)] string Move,
    int Save,
    int Control,
    int? WardSave,
    uint Version
);
