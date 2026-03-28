using AosAdjutant.Api.Common;
using AosAdjutant.Api.Database;
using AosAdjutant.Api.Features.AttackProfiles;
using AosAdjutant.Api.Features.Units;
using Microsoft.EntityFrameworkCore;

namespace AosAdjutant.UnitTests.Features.AttackProfiles;

public class AttackProfileServiceTests
{
    private static ApplicationDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

    public class CreateAttackProfile
    {
        [Fact]
        public async Task ReturnsAttackProfile_WhenUnitExistsAndDataIsValid()
        {
            await using var context = CreateContext();
            context.Units.Add(
                new Unit
                {
                    Name = "TestUnit",
                    Health = 10,
                    Move = "5",
                    Save = 4,
                    Control = 2,
                    FactionId = 1
                }
            );
            await context.SaveChangesAsync();
            var unitId = context.Units.Single().UnitId;
            var service = new AttackProfileService(context);
            var createDto = new CreateAttackProfileDto
            {
                Name = "TestProfile",
                IsRanged = false,
                Attacks = "2",
                ToHit = 3,
                ToWound = 3,
                Damage = "1",
                WeaponEffects = []
            };

            var result = await service.CreateAttackProfile(unitId, createDto);

            Assert.True(result.IsSuccess);
            Assert.Equivalent(
                new
                {
                    createDto.Name,
                    createDto.IsRanged,
                    createDto.Range,
                    createDto.Attacks,
                    createDto.ToHit,
                    createDto.ToWound,
                    createDto.Rend,
                    createDto.Damage,
                    UnitId = unitId
                },
                result.GetValue
            );
        }

        [Fact]
        public async Task ReturnsNotFound_WhenUnitDoesNotExist()
        {
            await using var context = CreateContext();
            var service = new AttackProfileService(context);

            var result = await service.CreateAttackProfile(
                999,
                new CreateAttackProfileDto
                {
                    Name = "TestProfile",
                    IsRanged = false,
                    Attacks = "2",
                    ToHit = 3,
                    ToWound = 3,
                    Damage = "1",
                    WeaponEffects = []
                }
            );

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.NotFound, result.GetError.Code);
        }

        [Fact]
        public async Task ReturnsUniqueKeyError_WhenNameAlreadyExistsInUnit()
        {
            await using var context = CreateContext();
            context.Units.Add(
                new Unit
                {
                    Name = "TestUnit",
                    Health = 10,
                    Move = "5",
                    Save = 4,
                    Control = 2,
                    FactionId = 1
                }
            );
            await context.SaveChangesAsync();
            var unitId = context.Units.Single().UnitId;
            context.AttackProfiles.Add(
                new AttackProfile
                {
                    Name = "TestProfile",
                    IsRanged = false,
                    Attacks = "2",
                    ToHit = 3,
                    ToWound = 3,
                    Damage = "1",
                    UnitId = unitId
                }
            );
            await context.SaveChangesAsync();
            var service = new AttackProfileService(context);

            var result = await service.CreateAttackProfile(
                unitId,
                new CreateAttackProfileDto
                {
                    Name = "TestProfile",
                    IsRanged = false,
                    Attacks = "2",
                    ToHit = 3,
                    ToWound = 3,
                    Damage = "1",
                    WeaponEffects = []
                }
            );

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.UniqueKeyError, result.GetError.Code);
        }

        [Fact]
        public async Task ReturnsValidationError_WhenAttackProfileDataIsInvalid()
        {
            await using var context = CreateContext();
            context.Units.Add(
                new Unit
                {
                    Name = "TestUnit",
                    Health = 10,
                    Move = "5",
                    Save = 4,
                    Control = 2,
                    FactionId = 1
                }
            );
            await context.SaveChangesAsync();
            var unitId = context.Units.Single().UnitId;
            var service = new AttackProfileService(context);

            var result = await service.CreateAttackProfile(
                unitId,
                new CreateAttackProfileDto
                {
                    Name = "TestProfile",
                    IsRanged = true,
                    Attacks = "2",
                    ToHit = 3,
                    ToWound = 3,
                    Damage = "1",
                    WeaponEffects = []
                }
            );

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.ValidationError, result.GetError.Code);
        }

        [Fact]
        public async Task ReturnsValidationError_WhenWeaponEffectKeyIsInvalid()
        {
            await using var context = CreateContext();
            context.Units.Add(
                new Unit
                {
                    Name = "TestUnit",
                    Health = 10,
                    Move = "5",
                    Save = 4,
                    Control = 2,
                    FactionId = 1
                }
            );
            await context.SaveChangesAsync();
            var unitId = context.Units.Single().UnitId;
            var service = new AttackProfileService(context);

            var result = await service.CreateAttackProfile(
                unitId,
                new CreateAttackProfileDto
                {
                    Name = "TestProfile",
                    IsRanged = false,
                    Attacks = "2",
                    ToHit = 3,
                    ToWound = 3,
                    Damage = "1",
                    WeaponEffects = ["invalid_key"]
                }
            );

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.ValidationError, result.GetError.Code);
        }
    }

    public class GetUnitAttackProfiles
    {
        [Fact]
        public async Task ReturnsAttackProfiles_WhenUnitExists()
        {
            await using var context = CreateContext();
            context.Units.Add(
                new Unit
                {
                    Name = "TestUnit",
                    Health = 10,
                    Move = "5",
                    Save = 4,
                    Control = 2,
                    FactionId = 1
                }
            );
            await context.SaveChangesAsync();
            var unitId = context.Units.Single().UnitId;
            context.AttackProfiles.AddRange(
                new AttackProfile
                {
                    Name = "TestProfile1",
                    IsRanged = false,
                    Attacks = "2",
                    ToHit = 3,
                    ToWound = 3,
                    Damage = "1",
                    UnitId = unitId
                },
                new AttackProfile
                {
                    Name = "TestProfile2",
                    IsRanged = false,
                    Attacks = "2",
                    ToHit = 3,
                    ToWound = 3,
                    Damage = "1",
                    UnitId = unitId
                }
            );
            await context.SaveChangesAsync();
            var service = new AttackProfileService(context);

            var result = await service.GetUnitAttackProfiles(unitId);

            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.GetValue.Count);
        }

        [Fact]
        public async Task ReturnsNotFound_WhenUnitDoesNotExist()
        {
            await using var context = CreateContext();
            var service = new AttackProfileService(context);

            var result = await service.GetUnitAttackProfiles(999);

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.NotFound, result.GetError.Code);
        }
    }

    public class GetAttackProfile
    {
        [Fact]
        public async Task ReturnsAttackProfile_WhenFound()
        {
            await using var context = CreateContext();
            var attackProfile = new AttackProfile
            {
                Name = "TestProfile",
                IsRanged = false,
                Attacks = "2",
                ToHit = 3,
                ToWound = 3,
                Damage = "1",
                UnitId = 1
            };
            context.AttackProfiles.Add(attackProfile);
            await context.SaveChangesAsync();
            var attackProfileId = context.AttackProfiles.Single().AttackProfileId;
            var service = new AttackProfileService(context);

            var result = await service.GetAttackProfile(attackProfileId);

            Assert.True(result.IsSuccess);
            Assert.Equivalent(attackProfile, result.GetValue);
        }

        [Fact]
        public async Task ReturnsNotFound_WhenAttackProfileDoesNotExist()
        {
            await using var context = CreateContext();
            var service = new AttackProfileService(context);

            var result = await service.GetAttackProfile(999);

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.NotFound, result.GetError.Code);
        }
    }

    public class UpdateAttackProfile
    {
        [Fact]
        public async Task ReturnsAttackProfile_WhenUpdateSucceeds()
        {
            await using var context = CreateContext();
            context.AttackProfiles.Add(
                new AttackProfile
                {
                    Name = "TestProfile",
                    IsRanged = false,
                    Attacks = "2",
                    ToHit = 3,
                    ToWound = 3,
                    Damage = "1",
                    UnitId = 1,
                    Version = 0
                }
            );
            await context.SaveChangesAsync();
            var attackProfileId = context.AttackProfiles.Single().AttackProfileId;
            var service = new AttackProfileService(context);
            var changeDto = new ChangeAttackProfileDto
            {
                Name = "UpdatedProfile",
                IsRanged = false,
                Attacks = "3",
                ToHit = 4,
                ToWound = 4,
                Damage = "2",
                WeaponEffects = [],
                Version = 0
            };

            var result = await service.UpdateAttackProfile(attackProfileId, changeDto);

            Assert.True(result.IsSuccess);
            Assert.Equivalent(
                new
                {
                    changeDto.Name,
                    changeDto.IsRanged,
                    changeDto.Range,
                    changeDto.Attacks,
                    changeDto.ToHit,
                    changeDto.ToWound,
                    changeDto.Rend,
                    changeDto.Damage
                },
                result.GetValue
            );
        }

        [Fact]
        public async Task ReturnsNotFound_WhenAttackProfileDoesNotExist()
        {
            await using var context = CreateContext();
            var service = new AttackProfileService(context);

            var result = await service.UpdateAttackProfile(
                999,
                new ChangeAttackProfileDto
                {
                    Name = "UpdatedProfile",
                    IsRanged = false,
                    Attacks = "3",
                    ToHit = 4,
                    ToWound = 4,
                    Damage = "2",
                    WeaponEffects = [],
                    Version = 0
                }
            );

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.NotFound, result.GetError.Code);
        }

        [Fact]
        public async Task ReturnsConcurrencyError_WhenVersionMismatch()
        {
            await using var context = CreateContext();
            context.AttackProfiles.Add(
                new AttackProfile
                {
                    Name = "TestProfile",
                    IsRanged = false,
                    Attacks = "2",
                    ToHit = 3,
                    ToWound = 3,
                    Damage = "1",
                    UnitId = 1,
                    Version = 5
                }
            );
            await context.SaveChangesAsync();
            var attackProfileId = context.AttackProfiles.Single().AttackProfileId;
            var service = new AttackProfileService(context);

            var result = await service.UpdateAttackProfile(
                attackProfileId,
                new ChangeAttackProfileDto
                {
                    Name = "UpdatedProfile",
                    IsRanged = false,
                    Attacks = "3",
                    ToHit = 4,
                    ToWound = 4,
                    Damage = "2",
                    WeaponEffects = [],
                    Version = 3
                }
            );

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.ConcurrencyError, result.GetError.Code);
        }

        [Fact]
        public async Task ReturnsUniqueKeyError_WhenNameAlreadyBelongsToAnotherAttackProfile()
        {
            await using var context = CreateContext();
            context.AttackProfiles.AddRange(
                new AttackProfile
                {
                    Name = "TestProfile1",
                    IsRanged = false,
                    Attacks = "2",
                    ToHit = 3,
                    ToWound = 3,
                    Damage = "1",
                    UnitId = 1,
                    Version = 0
                },
                new AttackProfile
                {
                    Name = "TestProfile2",
                    IsRanged = false,
                    Attacks = "2",
                    ToHit = 3,
                    ToWound = 3,
                    Damage = "1",
                    UnitId = 1,
                    Version = 0
                }
            );
            await context.SaveChangesAsync();
            var attackProfileId = context.AttackProfiles.First(ap => ap.Name == "TestProfile1").AttackProfileId;
            var service = new AttackProfileService(context);

            var result = await service.UpdateAttackProfile(
                attackProfileId,
                new ChangeAttackProfileDto
                {
                    Name = "TestProfile2",
                    IsRanged = false,
                    Attacks = "2",
                    ToHit = 3,
                    ToWound = 3,
                    Damage = "1",
                    WeaponEffects = [],
                    Version = 0
                }
            );

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.UniqueKeyError, result.GetError.Code);
        }

        [Fact]
        public async Task ReturnsValidationError_WhenAttackProfileDataIsInvalid()
        {
            await using var context = CreateContext();
            context.AttackProfiles.Add(
                new AttackProfile
                {
                    Name = "TestProfile",
                    IsRanged = false,
                    Attacks = "2",
                    ToHit = 3,
                    ToWound = 3,
                    Damage = "1",
                    UnitId = 1,
                    Version = 0
                }
            );
            await context.SaveChangesAsync();
            var attackProfileId = context.AttackProfiles.Single().AttackProfileId;
            var service = new AttackProfileService(context);

            var result = await service.UpdateAttackProfile(
                attackProfileId,
                new ChangeAttackProfileDto
                {
                    Name = "TestProfile",
                    IsRanged = true,
                    Attacks = "2",
                    ToHit = 3,
                    ToWound = 3,
                    Damage = "1",
                    WeaponEffects = [],
                    Version = 0
                }
            );

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.ValidationError, result.GetError.Code);
        }

        [Fact]
        public async Task ReturnsValidationError_WhenWeaponEffectKeyIsInvalid()
        {
            await using var context = CreateContext();
            context.AttackProfiles.Add(
                new AttackProfile
                {
                    Name = "TestProfile",
                    IsRanged = false,
                    Attacks = "2",
                    ToHit = 3,
                    ToWound = 3,
                    Damage = "1",
                    UnitId = 1,
                    Version = 0
                }
            );
            await context.SaveChangesAsync();
            var attackProfileId = context.AttackProfiles.Single().AttackProfileId;
            var service = new AttackProfileService(context);

            var result = await service.UpdateAttackProfile(
                attackProfileId,
                new ChangeAttackProfileDto
                {
                    Name = "TestProfile",
                    IsRanged = false,
                    Attacks = "2",
                    ToHit = 3,
                    ToWound = 3,
                    Damage = "1",
                    WeaponEffects = ["invalid_key"],
                    Version = 0
                }
            );

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.ValidationError, result.GetError.Code);
        }
    }

    public class DeleteAttackProfile
    {
        [Fact]
        public async Task ReturnsSuccess_WhenAttackProfileExists()
        {
            await using var context = CreateContext();
            context.AttackProfiles.Add(
                new AttackProfile
                {
                    Name = "TestProfile",
                    IsRanged = false,
                    Attacks = "2",
                    ToHit = 3,
                    ToWound = 3,
                    Damage = "1",
                    UnitId = 1
                }
            );
            await context.SaveChangesAsync();
            var attackProfileId = context.AttackProfiles.Single().AttackProfileId;
            var service = new AttackProfileService(context);

            var result = await service.DeleteAttackProfile(attackProfileId);

            Assert.True(result.IsSuccess);
            Assert.Empty(context.AttackProfiles);
        }

        [Fact]
        public async Task ReturnsNotFound_WhenAttackProfileDoesNotExist()
        {
            await using var context = CreateContext();
            var service = new AttackProfileService(context);

            var result = await service.DeleteAttackProfile(999);

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.NotFound, result.GetError.Code);
        }
    }
}
