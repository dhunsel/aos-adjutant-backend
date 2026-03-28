using AosAdjutant.Api.Common;
using AosAdjutant.Api.Database;
using AosAdjutant.Api.Features.Abilities;
using AosAdjutant.Api.Features.Factions;
using Microsoft.EntityFrameworkCore;

namespace AosAdjutant.Api.Features.Units;

public sealed class UnitService(ApplicationDbContext context)
{
    public async Task<Result<Unit>> CreateUnit(int factionId, CreateUnitDto unitData)
    {
        var factionExists = await context.Factions.AnyAsync(f => f.FactionId == factionId);
        if (!factionExists)
            return Result<Unit>.Failure(FactionErrors.NotFound);

        var isDuplicate = await context.Units.AnyAsync(u => u.Name == unitData.Name && u.FactionId == factionId);
        if (isDuplicate)
            return Result<Unit>.Failure(UnitErrors.AlreadyExists);

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
            return Result<List<Unit>>.Failure(FactionErrors.NotFound);

        var units = await context.Units.AsNoTracking().Where(u => u.FactionId == factionId).ToListAsync();
        return Result<List<Unit>>.Success(units);
    }

    public async Task<Result<Unit>> GetUnit(int unitId)
    {
        var unit = await context.Units.AsNoTracking().FirstOrDefaultAsync(u => u.UnitId == unitId);

        return unit is null ? Result<Unit>.Failure(UnitErrors.NotFound) : Result<Unit>.Success(unit);
    }

    public async Task<Result<Unit>> UpdateUnit(int unitId, ChangeUnitDto unitData)
    {
        var unit = await context.Units.FindAsync(unitId);

        if (unit is null)
            return Result<Unit>.Failure(UnitErrors.NotFound);

        if (unit.Version != unitData.Version)
            return Result<Unit>.Failure(UnitErrors.Concurrency);

        var isDuplicate = await context.Units.AnyAsync(u =>
            u.Name == unitData.Name && u.FactionId == unit.FactionId && u.UnitId != unitId
        );
        if (isDuplicate)
            return Result<Unit>.Failure(UnitErrors.AlreadyExists);

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
            return Result.Failure(UnitErrors.NotFound);

        context.Units.Remove(unit);
        await context.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result<Ability>> CreateUnitAbility(int unitId, CreateAbilityDto abilityData)
    {
        var unit = await context.Units.FindAsync(unitId);

        if (unit is null)
            return Result<Ability>.Failure(UnitErrors.NotFound);

        var newAbilityResult = Ability.Create(
            new AbilityData
            {
                Name = abilityData.Name,
                Reaction = abilityData.Reaction,
                Declaration = abilityData.Declaration,
                Effect = abilityData.Effect,
                Phase = abilityData.Phase,
                Restriction = abilityData.Restriction,
                Turn = abilityData.Turn,
                IsGeneric = false
            }
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
            ? Result<List<Ability>>.Failure(UnitErrors.NotFound)
            : Result<List<Ability>>.Success(unit.Abilities.ToList());
    }
}
