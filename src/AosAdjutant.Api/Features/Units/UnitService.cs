using AosAdjutant.Api.Database;
using AosAdjutant.Api.Features.Abilities;
using AosAdjutant.Api.Shared;
using Microsoft.EntityFrameworkCore;

namespace AosAdjutant.Api.Features.Units;

public class UnitService(ApplicationDbContext context)
{
    public async Task<Result<Unit>> CreateUnit(int factionId, CreateUnitDto unitData)
    {
        var factionExists = await context.Factions.AnyAsync(f => f.FactionId == factionId);
        if (!factionExists)
            return Result<Unit>.Failure(new AppError(ErrorCode.NotFound, "Faction not found."));

        var isDuplicate = await context.Units.AnyAsync(u => u.Name == unitData.Name && u.FactionId == factionId);
        if (isDuplicate)
            return Result<Unit>.Failure(new AppError(ErrorCode.UniqueKeyError, "Unit already exists."));

        var newUnit = new Unit
        {
            Name = unitData.Name,
            Health = unitData.Health,
            Move = unitData.Move,
            Save = unitData.Save,
            Control = unitData.Control,
            WardSave = unitData.WardSave,
            FactionId = factionId,
        };

        // Because of race conditions this might still fail on UK/FK error
        // Ignore for now (won't occur in practice) but revisit in the future
        context.Units.Add(newUnit);
        await context.SaveChangesAsync();

        return Result<Unit>.Success(newUnit);
    }

    public async Task<Result<List<Unit>>> GetFactionUnits(int factionId)
    {
        var factionExists = await context.Factions.AnyAsync(f => f.FactionId == factionId);
        if (!factionExists)
            return Result<List<Unit>>.Failure(new AppError(ErrorCode.NotFound, "Faction not found."));

        var units = await context.Units.AsNoTracking().Where(u => u.FactionId == factionId).ToListAsync();
        return Result<List<Unit>>.Success(units);
    }

    public async Task<Result<Unit>> GetUnit(int unitId)
    {
        var unit = await context.Units.AsNoTracking().FirstOrDefaultAsync(u => u.UnitId == unitId);

        return unit is null
            ? Result<Unit>.Failure(new AppError(ErrorCode.NotFound, "Unit not found."))
            : Result<Unit>.Success(unit);
    }

    public async Task<Result<Unit>> UpdateUnit(int unitId, ChangeUnitDto unitData)
    {
        var unit = await context.Units.FindAsync(unitId);

        if (unit is null)
            return Result<Unit>.Failure(new AppError(ErrorCode.NotFound, "Unit not found."));

        if (unit.Version != unitData.Version)
            return Result<Unit>.Failure(
                new AppError(ErrorCode.ConcurrencyError, "Unit was already modified in the background.")
            );

        var isDuplicate = await context.Units.AnyAsync(u =>
            u.Name == unitData.Name && u.FactionId == unit.FactionId && u.UnitId != unitId
        );
        if (isDuplicate)
            return Result<Unit>.Failure(new AppError(ErrorCode.UniqueKeyError, "Unit already exists."));

        unit.Name = unitData.Name;
        unit.Health = unitData.Health;
        unit.Move = unitData.Move;
        unit.Save = unitData.Save;
        unit.Control = unitData.Control;
        unit.WardSave = unitData.WardSave;

        await context.SaveChangesAsync();

        return Result<Unit>.Success(unit);
    }

    public async Task<Result> DeleteUnit(int unitId)
    {
        var unit = await context.Units.FindAsync(unitId);

        if (unit is null)
            return Result.Failure(new AppError(ErrorCode.NotFound, "Unit not found."));

        context.Units.Remove(unit);
        await context.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result<Ability>> CreateUnitAbility(int unitId, CreateAbilityDto abilityData)
    {
        var unit = await context.Units.FindAsync(unitId);

        if (unit is null)
            return Result<Ability>.Failure(new AppError(ErrorCode.NotFound, "Unit not found."));

        var newAbilityResult = Ability.Create(
            abilityData.Name,
            abilityData.Reaction,
            abilityData.Declaration,
            abilityData.Effect,
            abilityData.Phase,
            abilityData.Restriction,
            abilityData.Turn,
            false
        );

        if (!newAbilityResult.IsSuccess) return Result<Ability>.Failure(newAbilityResult.GetError);

        var newAbility = newAbilityResult.GetValue;
        unit.Abilities.Add(newAbility);
        await context.SaveChangesAsync();

        return Result<Ability>.Success(newAbility);
    }

    public async Task<Result<List<Ability>>> GetUnitAbilities(int unitId)
    {
        var unit = await context.Units
            .AsNoTracking()
            .Include(u => u.Abilities)
            .FirstOrDefaultAsync(u => u.UnitId == unitId);

        return unit is null
            ? Result<List<Ability>>.Failure(new AppError(ErrorCode.NotFound, "Unit not found."))
            : Result<List<Ability>>.Success(unit.Abilities.ToList());
    }
}
