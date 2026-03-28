using AosAdjutant.Api.Common;
using AosAdjutant.Api.Database;
using AosAdjutant.Api.Features.Units;
using Microsoft.EntityFrameworkCore;

namespace AosAdjutant.Api.Features.AttackProfiles;

public sealed class AttackProfileService(ApplicationDbContext context)
{
    public async Task<Result<AttackProfile>> CreateAttackProfile(int unitId, CreateAttackProfileDto attackProfileData)
    {
        var unitExists = await context.Units.AnyAsync(u => u.UnitId == unitId);
        if (!unitExists)
            return Result<AttackProfile>.Failure(UnitErrors.NotFound);

        var isDuplicate =
            await context.AttackProfiles.AnyAsync(ap => ap.Name == attackProfileData.Name && ap.UnitId == unitId);
        if (isDuplicate)
            return Result<AttackProfile>.Failure(AttackProfileErrors.AlreadyExists);

        var newAttackProfileResult = AttackProfile.Create(
            new AttackProfileData
            {
                Name = attackProfileData.Name,
                IsRanged = attackProfileData.IsRanged,
                Range = attackProfileData.Range,
                Attacks = attackProfileData.Attacks,
                ToHit = attackProfileData.ToHit,
                ToWound = attackProfileData.ToWound,
                Rend = attackProfileData.Rend,
                Damage = attackProfileData.Damage,
                UnitId = unitId
            }
        );

        if (!newAttackProfileResult.IsSuccess) return newAttackProfileResult;

        var newAttackProfile = newAttackProfileResult.GetValue;

        var weaponEffects = await context.WeaponEffects
            .Where(we => attackProfileData.WeaponEffects.Contains(we.Key))
            .ToListAsync();

        if (weaponEffects.Count != attackProfileData.WeaponEffects.Count)
            return Result<AttackProfile>.Failure(
                new AppError(ErrorCode.ValidationError, "One or more weapon effect keys invalid.")
            );

        foreach (var weaponEffect in weaponEffects)
            newAttackProfile.WeaponEffects.Add(weaponEffect);

        // Because of race conditions this might still fail on UK/FK error
        // Ignore for now (won't occur in practice) but revisit in the future
        context.AttackProfiles.Add(newAttackProfile);
        await context.SaveChangesAsync();

        return Result<AttackProfile>.Success(newAttackProfile);
    }

    public async Task<Result<List<AttackProfile>>> GetUnitAttackProfiles(int unitId)
    {
        var unitExists = await context.Units.AnyAsync(u => u.UnitId == unitId);
        if (!unitExists)
            return Result<List<AttackProfile>>.Failure(UnitErrors.NotFound);

        var attackProfiles = await context.AttackProfiles.AsNoTracking().Where(ap => ap.UnitId == unitId).ToListAsync();
        return Result<List<AttackProfile>>.Success(attackProfiles);
    }

    public async Task<Result<AttackProfile>> GetAttackProfile(int attackProfileId)
    {
        var attackProfile = await context.AttackProfiles
            .AsNoTracking()
            .Include(ap => ap.WeaponEffects)
            .FirstOrDefaultAsync(ap => ap.AttackProfileId == attackProfileId);

        return attackProfile is null
            ? Result<AttackProfile>.Failure(AttackProfileErrors.NotFound)
            : Result<AttackProfile>.Success(attackProfile);
    }

    public async Task<Result<AttackProfile>> UpdateAttackProfile(
        int attackProfileId,
        ChangeAttackProfileDto attackProfileData
    )
    {
        var attackProfile = await context.AttackProfiles
            .Include(ap => ap.WeaponEffects)
            .FirstOrDefaultAsync(ap => ap.AttackProfileId == attackProfileId);

        if (attackProfile is null)
            return Result<AttackProfile>.Failure(AttackProfileErrors.NotFound);

        if (attackProfile.Version != attackProfileData.Version)
            return Result<AttackProfile>.Failure(AttackProfileErrors.Concurrency);

        var isDuplicate = await context.AttackProfiles.AnyAsync(ap =>
            ap.Name == attackProfileData.Name && ap.UnitId == attackProfile.UnitId &&
            ap.AttackProfileId != attackProfileId
        );
        if (isDuplicate)
            return Result<AttackProfile>.Failure(AttackProfileErrors.AlreadyExists);

        var changeResult = attackProfile.Change(
            new AttackProfileData
            {
                Name = attackProfileData.Name,
                IsRanged = attackProfileData.IsRanged,
                Range = attackProfileData.Range,
                Attacks = attackProfileData.Attacks,
                ToHit = attackProfileData.ToHit,
                ToWound = attackProfileData.ToWound,
                Rend = attackProfileData.Rend,
                Damage = attackProfileData.Damage,
                UnitId = attackProfile.UnitId
            }
        );

        if (!changeResult.IsSuccess) return Result<AttackProfile>.Failure(changeResult.GetError);

        var weaponEffects = await context.WeaponEffects
            .Where(we => attackProfileData.WeaponEffects.Contains(we.Key))
            .ToListAsync();

        if (weaponEffects.Count != attackProfileData.WeaponEffects.Count)
            return Result<AttackProfile>.Failure(
                new AppError(ErrorCode.ValidationError, "One or more weapon effect keys invalid.")
            );

        foreach (var weaponEffect in weaponEffects)
            attackProfile.WeaponEffects.Add(weaponEffect);

        await context.SaveChangesAsync();

        return Result<AttackProfile>.Success(attackProfile);
    }

    public async Task<Result> DeleteAttackProfile(int attackProfileId)
    {
        var attackProfile = await context.AttackProfiles.FindAsync(attackProfileId);

        if (attackProfile is null)
            return Result.Failure(AttackProfileErrors.NotFound);

        context.AttackProfiles.Remove(attackProfile);
        await context.SaveChangesAsync();

        return Result.Success();
    }
}
