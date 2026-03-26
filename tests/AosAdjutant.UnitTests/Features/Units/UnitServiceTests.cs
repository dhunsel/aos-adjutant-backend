using AosAdjutant.Api.Database;
using AosAdjutant.Api.Features.Abilities;
using AosAdjutant.Api.Features.Factions;
using AosAdjutant.Api.Features.Units;
using AosAdjutant.Api.Shared;
using Microsoft.EntityFrameworkCore;

namespace AosAdjutant.UnitTests.Features.Units;

public class UnitServiceTests
{
    private static ApplicationDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

    public class CreateUnit
    {
        [Fact]
        public async Task ReturnsCreatedUnit_WhenFactionExistsAndNameIsUnique()
        {
            await using var context = CreateContext();
            context.Factions.Add(new Faction { Name = "TestFaction" });
            await context.SaveChangesAsync();
            var factionId = context.Factions.Single().FactionId;
            var service = new UnitService(context);
            var createUnitDto = new CreateUnitDto("TestUnit", 10, "5", 4, 2, null);

            var result = await service.CreateUnit(factionId, createUnitDto);

            Assert.True(result.IsSuccess);
            Assert.Equivalent(
                new
                {
                    createUnitDto.Name,
                    createUnitDto.Health,
                    createUnitDto.Move,
                    createUnitDto.Save,
                    createUnitDto.Control,
                    createUnitDto.WardSave,
                    FactionId = factionId
                },
                result.GetValue
            );
        }

        [Fact]
        public async Task ReturnsNotFound_WhenFactionDoesNotExist()
        {
            await using var context = CreateContext();
            var service = new UnitService(context);

            var result = await service.CreateUnit(999, new CreateUnitDto("TestUnit", 10, "5", 4, 2, null));

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.NotFound, result.GetError.Code);
        }

        [Fact]
        public async Task ReturnsUniqueKeyError_WhenNameAlreadyExistsInFaction()
        {
            await using var context = CreateContext();
            context.Factions.Add(new Faction { Name = "TestFaction" });
            await context.SaveChangesAsync();
            var factionId = context.Factions.Single().FactionId;
            var unit = new Unit
            {
                Name = "TestUnit",
                Health = 10,
                Move = "5",
                Save = 4,
                Control = 2,
                FactionId = factionId
            };
            context.Units.Add(unit);
            await context.SaveChangesAsync();
            var service = new UnitService(context);

            var result = await service.CreateUnit(factionId, new CreateUnitDto("TestUnit", 10, "5", 4, 2, null));

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.UniqueKeyError, result.GetError.Code);
        }
    }

    public class GetFactionUnits
    {
        [Fact]
        public async Task ReturnsUnits_WhenFactionExists()
        {
            await using var context = CreateContext();
            context.Factions.Add(new Faction { Name = "TestFaction" });
            await context.SaveChangesAsync();
            var factionId = context.Factions.Single().FactionId;
            context.Units.AddRange(
                new Unit
                {
                    Name = "TestUnit1",
                    Health = 10,
                    Move = "5",
                    Save = 4,
                    Control = 2,
                    FactionId = factionId
                },
                new Unit
                {
                    Name = "TestUnit2",
                    Health = 10,
                    Move = "5",
                    Save = 4,
                    Control = 2,
                    FactionId = factionId
                }
            );
            await context.SaveChangesAsync();
            var service = new UnitService(context);

            var result = await service.GetFactionUnits(factionId);

            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.GetValue.Count);
        }

        [Fact]
        public async Task ReturnsNotFound_WhenFactionDoesNotExist()
        {
            await using var context = CreateContext();
            var service = new UnitService(context);

            var result = await service.GetFactionUnits(999);

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.NotFound, result.GetError.Code);
        }
    }

    public class GetUnit
    {
        [Fact]
        public async Task ReturnsUnit_WhenFound()
        {
            await using var context = CreateContext();
            var unit = new Unit
            {
                Name = "TestUnit",
                Health = 10,
                Move = "5",
                Save = 4,
                Control = 2,
                FactionId = 1
            };
            context.Units.Add(unit);
            await context.SaveChangesAsync();
            var unitId = context.Units.Single().UnitId;
            var service = new UnitService(context);

            var result = await service.GetUnit(unitId);

            Assert.True(result.IsSuccess);
            Assert.Equivalent(unit, result.GetValue);
        }

        [Fact]
        public async Task ReturnsNotFound_WhenUnitDoesNotExist()
        {
            await using var context = CreateContext();
            var service = new UnitService(context);

            var result = await service.GetUnit(999);

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.NotFound, result.GetError.Code);
        }
    }

    public class UpdateUnit
    {
        [Fact]
        public async Task ReturnsUnit_WhenUpdateSucceeds()
        {
            await using var context = CreateContext();
            var unit = new Unit
            {
                Name = "TestUnit",
                Health = 10,
                Move = "5",
                Save = 4,
                Control = 2,
                FactionId = 1
            };
            context.Units.Add(unit);
            await context.SaveChangesAsync();
            var unitId = context.Units.Single().UnitId;
            var service = new UnitService(context);
            var changeUnitDto = new ChangeUnitDto("UpdatedUnit", 20, "6", 3, 1, 5, 0);

            var result = await service.UpdateUnit(unitId, changeUnitDto);

            Assert.True(result.IsSuccess);
            Assert.Equivalent(
                new
                {
                    changeUnitDto.Name,
                    changeUnitDto.Health,
                    changeUnitDto.Move,
                    changeUnitDto.Save,
                    changeUnitDto.Control,
                    changeUnitDto.WardSave,
                },
                result.GetValue
            );
        }

        [Fact]
        public async Task ReturnsNotFound_WhenUnitDoesNotExist()
        {
            await using var context = CreateContext();
            var service = new UnitService(context);

            var result = await service.UpdateUnit(999, new ChangeUnitDto("UpdatedUnit", 20, "6", 3, 1, null, 0));

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.NotFound, result.GetError.Code);
        }

        [Fact]
        public async Task ReturnsConcurrencyError_WhenVersionMismatch()
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
                    FactionId = 1,
                    Version = 5
                }
            );
            await context.SaveChangesAsync();
            var unitId = context.Units.Single().UnitId;
            var service = new UnitService(context);

            var result = await service.UpdateUnit(unitId, new ChangeUnitDto("UpdatedUnit", 20, "6", 3, 1, null, 3));

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.ConcurrencyError, result.GetError.Code);
        }

        [Fact]
        public async Task ReturnsUniqueKeyError_WhenNameAlreadyBelongsToAnotherUnit()
        {
            await using var context = CreateContext();
            context.Units.AddRange(
                new Unit
                {
                    Name = "TestUnit1",
                    Health = 10,
                    Move = "5",
                    Save = 4,
                    Control = 2,
                    FactionId = 1,
                    Version = 0
                },
                new Unit
                {
                    Name = "TestUnit2",
                    Health = 10,
                    Move = "5",
                    Save = 4,
                    Control = 2,
                    FactionId = 1,
                    Version = 0
                }
            );
            await context.SaveChangesAsync();
            var unitId = context.Units.First(u => u.Name == "TestUnit1").UnitId;
            var service = new UnitService(context);

            var result = await service.UpdateUnit(unitId, new ChangeUnitDto("TestUnit2", 10, "5", 4, 2, null, 0));

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.UniqueKeyError, result.GetError.Code);
        }
    }

    public class DeleteUnit
    {
        [Fact]
        public async Task ReturnsSuccess_WhenUnitExists()
        {
            await using var context = CreateContext();
            var unit = new Unit
            {
                Name = "TestUnit",
                Health = 10,
                Move = "5",
                Save = 4,
                Control = 2,
                FactionId = 1
            };
            context.Units.Add(unit);
            await context.SaveChangesAsync();
            var unitId = context.Units.Single().UnitId;
            var service = new UnitService(context);

            var result = await service.DeleteUnit(unitId);

            Assert.True(result.IsSuccess);
            Assert.Empty(context.Units);
        }

        [Fact]
        public async Task ReturnsNotFound_WhenUnitDoesNotExist()
        {
            await using var context = CreateContext();
            var service = new UnitService(context);

            var result = await service.DeleteUnit(999);

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.NotFound, result.GetError.Code);
        }
    }

    public class CreateUnitAbility
    {
        private static CreateAbilityDto ValidAbilityDto() => new(
            "TestAbility",
            null,
            "TestDeclaration",
            "TestEffect",
            TurnPhase.Hero,
            null,
            PlayerTurn.YourTurn
        );

        [Fact]
        public async Task ReturnsAbility_WhenUnitExistsAndDataIsValid()
        {
            await using var context = CreateContext();
            var unit = new Unit
            {
                Name = "TestUnit",
                Health = 10,
                Move = "5",
                Save = 4,
                Control = 2,
                FactionId = 1
            };
            context.Units.Add(unit);
            await context.SaveChangesAsync();
            var unitId = context.Units.Single().UnitId;
            var service = new UnitService(context);
            var createAbilityDto = ValidAbilityDto();

            var result = await service.CreateUnitAbility(unitId, createAbilityDto);

            Assert.True(result.IsSuccess);
            Assert.Equivalent(
                new
                {
                    createAbilityDto.Name,
                    createAbilityDto.Reaction,
                    createAbilityDto.Declaration,
                    createAbilityDto.Effect,
                    createAbilityDto.Phase,
                    createAbilityDto.Restriction,
                    createAbilityDto.Turn,
                    IsGeneric = false
                },
                result.GetValue
            );
        }

        [Fact]
        public async Task ReturnsNotFound_WhenUnitDoesNotExist()
        {
            await using var context = CreateContext();
            var service = new UnitService(context);

            var result = await service.CreateUnitAbility(999, ValidAbilityDto());

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.NotFound, result.GetError.Code);
        }

        [Fact]
        public async Task ReturnsValidationError_WhenAbilityDataIsInvalid()
        {
            await using var context = CreateContext();
            var unit = new Unit
            {
                Name = "TestUnit",
                Health = 10,
                Move = "5",
                Save = 4,
                Control = 2,
                FactionId = 1
            };
            context.Units.Add(unit);
            await context.SaveChangesAsync();
            var unitId = context.Units.Single().UnitId;
            var service = new UnitService(context);

            var invalidDto = new CreateAbilityDto(
                "TestAbility",
                null,
                "TestDeclaration",
                "TestEffect",
                TurnPhase.Passive,
                null,
                null
            );

            var result = await service.CreateUnitAbility(unitId, invalidDto);

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.ValidationError, result.GetError.Code);
        }
    }

    public class GetUnitAbilities
    {
        [Fact]
        public async Task ReturnsAbilities_WhenUnitExists()
        {
            await using var context = CreateContext();
            var unit = new Unit
            {
                Name = "TestUnit",
                Health = 10,
                Move = "5",
                Save = 4,
                Control = 2,
                FactionId = 1
            };
            context.Units.Add(unit);
            await context.SaveChangesAsync();
            var unitId = context.Units.Single().UnitId;
            var service = new UnitService(context);

            await service.CreateUnitAbility(
                unitId,
                new CreateAbilityDto(
                    "TestAbility",
                    null,
                    "TestDeclaration",
                    "TestEffect",
                    TurnPhase.Hero,
                    null,
                    PlayerTurn.YourTurn
                )
            );

            var result = await service.GetUnitAbilities(unitId);

            Assert.True(result.IsSuccess);
            Assert.Single(result.GetValue);
        }

        [Fact]
        public async Task ReturnsNotFound_WhenUnitDoesNotExist()
        {
            await using var context = CreateContext();
            var service = new UnitService(context);

            var result = await service.GetUnitAbilities(999);

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.NotFound, result.GetError.Code);
        }
    }
}
