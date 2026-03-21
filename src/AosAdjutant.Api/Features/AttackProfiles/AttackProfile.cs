using AosAdjutant.Api.Features.WeaponEffects;
using AosAdjutant.Api.Shared;

namespace AosAdjutant.Api.Features.AttackProfiles;

public class AttackProfile
{
    public int AttackProfileId { get; set; }
    public required string Name { get; set; }
    public required bool IsRanged { get; set; }
    public int? Range { get; set; }
    public required string Attacks { get; set; }
    public required int ToHit { get; set; }
    public required int ToWound { get; set; }
    public int? Rend { get; set; }
    public required string Damage { get; set; }
    public required int UnitId { get; set; }
    public uint Version { get; set; }

    public ICollection<WeaponEffect> WeaponEffects { get; } = new List<WeaponEffect>();

    public static Result<AttackProfile> Create(
        string name,
        bool isRanged,
        int? range,
        string attacks,
        int toHit,
        int toWound,
        int? rend,
        string damage,
        int unitId
    )
    {
        var validationResult = ValidateAttackProfile(name, isRanged, range, attacks, toHit, toWound, rend, damage);

        if (!validationResult.IsSuccess) return Result<AttackProfile>.Failure(validationResult.GetError);

        var attackProfile = new AttackProfile
        {
            Name = name,
            IsRanged = isRanged,
            Range = range,
            Attacks = attacks,
            ToHit = toHit,
            ToWound = toWound,
            Rend = rend,
            Damage = damage,
            UnitId = unitId
        };

        return Result<AttackProfile>.Success(attackProfile);
    }

    public Result Change(
        string name,
        bool isRanged,
        int? range,
        string attacks,
        int toHit,
        int toWound,
        int? rend,
        string damage
    )
    {
        var validationResult = ValidateAttackProfile(name, isRanged, range, attacks, toHit, toWound, rend, damage);

        if (!validationResult.IsSuccess) return Result.Failure(validationResult.GetError);

        Name = name;
        IsRanged = isRanged;
        Range = range;
        Attacks = attacks;
        ToHit = toHit;
        ToWound = toWound;
        Rend = rend;
        Damage = damage;

        return Result.Success();
    }

    private static Result ValidateAttackProfile(
        string name,
        bool isRanged,
        int? range,
        string attacks,
        int toHit,
        int toWound,
        int? rend,
        string damage
    )
    {
        if (isRanged && range is null)
            return Result.Failure(
                new AppError(ErrorCode.ValidationError, "A Ranged weapon profile must have a range.")
            );

        if (!isRanged && range is not null)
            return Result.Failure(
                new AppError(ErrorCode.ValidationError, "A melee weapon profile cannot have a range.")
            );

        if (toHit < 2 || toHit > 7 || toWound < 2 || toWound > 7)
            return Result.Failure(
                new AppError(ErrorCode.ValidationError, "To hit and to wound values must be between 2 and 6.")
            );

        return Result.Success();
    }
}
