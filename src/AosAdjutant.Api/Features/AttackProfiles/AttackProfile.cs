#pragma warning disable MA0048
using AosAdjutant.Api.Common;
using AosAdjutant.Api.Features.WeaponEffects;

namespace AosAdjutant.Api.Features.AttackProfiles;

public sealed record AttackProfileData
{
    public required string Name { get; init; }
    public required bool IsRanged { get; init; }
    public int? Range { get; init; }
    public required string Attacks { get; init; }
    public required int ToHit { get; init; }
    public required int ToWound { get; init; }
    public int? Rend { get; init; }
    public required string Damage { get; init; }
    public required int UnitId { get; init; }
}

public sealed class AttackProfile
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


    public static Result<AttackProfile> Create(AttackProfileData data)
    {
        var validationResult = ValidateAttackProfile(data);

        if (!validationResult.IsSuccess) return Result<AttackProfile>.Failure(validationResult.GetError);

        var attackProfile = new AttackProfile
        {
            Name = data.Name,
            IsRanged = data.IsRanged,
            Range = data.Range,
            Attacks = data.Attacks,
            ToHit = data.ToHit,
            ToWound = data.ToWound,
            Rend = data.Rend,
            Damage = data.Damage,
            UnitId = data.UnitId,
        };

        return Result<AttackProfile>.Success(attackProfile);
    }

    public Result Change(AttackProfileData data)
    {
        var validationResult = ValidateAttackProfile(data);

        if (!validationResult.IsSuccess) return Result.Failure(validationResult.GetError);

        Name = data.Name;
        IsRanged = data.IsRanged;
        Range = data.Range;
        Attacks = data.Attacks;
        ToHit = data.ToHit;
        ToWound = data.ToWound;
        Rend = data.Rend;
        Damage = data.Damage;

        return Result.Success();
    }

    private static Result ValidateAttackProfile(AttackProfileData data)
    {
        if (data is { IsRanged: true, Range: null })
            return Result.Failure(
                new AppError(ErrorCode.ValidationError, "A Ranged weapon profile must have a range.")
            );

        if (data is { IsRanged: false, Range: not null })
            return Result.Failure(
                new AppError(ErrorCode.ValidationError, "A melee weapon profile cannot have a range.")
            );

        if (data.ToHit < 2 || data.ToHit > 7 || data.ToWound < 2 || data.ToWound > 7)
            return Result.Failure(
                new AppError(ErrorCode.ValidationError, "To hit and to wound values must be between 2 and 6.")
            );

        return Result.Success();
    }
}

public static class AttackProfileErrors
{
    public static readonly AppError NotFound = new(ErrorCode.NotFound, "Attack profile not found.");
    public static readonly AppError AlreadyExists = new(ErrorCode.UniqueKeyError, "Attack profile already exists.");

    public static readonly AppError Concurrency = new(
        ErrorCode.ConcurrencyError,
        "Attack profile was already modified in the background."
    );
}
