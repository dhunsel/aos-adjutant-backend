using AosAdjutant.Api.Common;
using AosAdjutant.Api.Database;
using AosAdjutant.Api.Features.Abilities;
using AosAdjutant.Api.Features.BattleFormations;
using AosAdjutant.Api.Features.Factions;
using Microsoft.EntityFrameworkCore;

namespace AosAdjutant.UnitTests.Features.BattleFormations;

public class BattleFormationServiceTests
{
    private static ApplicationDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

    public class CreateBattleFormation
    {
        [Fact]
        public async Task ReturnsCreatedBattleFormation_WhenFactionExistsAndNameIsUnique()
        {
            await using var context = CreateContext();
            context.Factions.Add(new Faction { Name = "TestFaction" });
            await context.SaveChangesAsync();
            var factionId = context.Factions.Single().FactionId;
            var service = new BattleFormationService(context);

            var result = await service.CreateBattleFormation(
                factionId,
                new CreateBattleFormationDto { Name = "TestBattleFormation" }
            );

            Assert.True(result.IsSuccess);
            Assert.Equal("TestBattleFormation", result.GetValue.Name);
            Assert.Equal(factionId, result.GetValue.FactionId);
        }

        [Fact]
        public async Task ReturnsNotFound_WhenFactionDoesNotExist()
        {
            await using var context = CreateContext();
            var service = new BattleFormationService(context);

            var result = await service.CreateBattleFormation(
                999,
                new CreateBattleFormationDto { Name = "TestBattleFormation" }
            );

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
            context.BattleFormations.Add(new BattleFormation { Name = "TestBattleFormation", FactionId = factionId });
            await context.SaveChangesAsync();
            var service = new BattleFormationService(context);

            var result = await service.CreateBattleFormation(
                factionId,
                new CreateBattleFormationDto { Name = "TestBattleFormation" }
            );

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.UniqueKeyError, result.GetError.Code);
        }
    }

    public class GetFactionBattleFormations
    {
        [Fact]
        public async Task ReturnsBattleFormations_WhenFactionExists()
        {
            await using var context = CreateContext();
            context.Factions.Add(new Faction { Name = "TestFaction" });
            await context.SaveChangesAsync();
            var factionId = context.Factions.Single().FactionId;
            context.BattleFormations.AddRange(
                new BattleFormation { Name = "TestBattleFormation1", FactionId = factionId },
                new BattleFormation { Name = "TestBattleFormation2", FactionId = factionId }
            );
            await context.SaveChangesAsync();
            var service = new BattleFormationService(context);

            var result = await service.GetFactionBattleFormations(factionId);

            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.GetValue.Count);
        }

        [Fact]
        public async Task ReturnsNotFound_WhenFactionDoesNotExist()
        {
            await using var context = CreateContext();
            var service = new BattleFormationService(context);

            var result = await service.GetFactionBattleFormations(999);

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.NotFound, result.GetError.Code);
        }
    }

    public class GetBattleFormation
    {
        [Fact]
        public async Task ReturnsBattleFormation_WhenFound()
        {
            await using var context = CreateContext();
            context.BattleFormations.Add(new BattleFormation { Name = "TestBattleFormation", FactionId = 1 });
            await context.SaveChangesAsync();
            var battleFormationId = context.BattleFormations.Single().BattleFormationId;
            var service = new BattleFormationService(context);

            var result = await service.GetBattleFormation(battleFormationId);

            Assert.True(result.IsSuccess);
            Assert.Equal("TestBattleFormation", result.GetValue.Name);
        }

        [Fact]
        public async Task ReturnsNotFound_WhenBattleFormationDoesNotExist()
        {
            await using var context = CreateContext();
            var service = new BattleFormationService(context);

            var result = await service.GetBattleFormation(999);

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.NotFound, result.GetError.Code);
        }
    }

    public class UpdateBattleFormation
    {
        [Fact]
        public async Task ReturnsBattleFormation_WhenUpdateSucceeds()
        {
            await using var context = CreateContext();
            context.BattleFormations.Add(new BattleFormation { Name = "OldName", FactionId = 1, Version = 0 });
            await context.SaveChangesAsync();
            var battleFormationId = context.BattleFormations.Single().BattleFormationId;
            var service = new BattleFormationService(context);

            var result = await service.UpdateBattleFormation(
                battleFormationId,
                new ChangeBattleFormationDto { Name = "NewName", Version = 0 }
            );

            Assert.True(result.IsSuccess);
            Assert.Equal("NewName", result.GetValue.Name);
        }

        [Fact]
        public async Task ReturnsNotFound_WhenBattleFormationDoesNotExist()
        {
            await using var context = CreateContext();
            var service = new BattleFormationService(context);

            var result = await service.UpdateBattleFormation(
                999,
                new ChangeBattleFormationDto { Name = "NewName", Version = 0 }
            );

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.NotFound, result.GetError.Code);
        }

        [Fact]
        public async Task ReturnsConcurrencyError_WhenVersionMismatch()
        {
            await using var context = CreateContext();
            context.BattleFormations.Add(
                new BattleFormation { Name = "TestBattleFormation", FactionId = 1, Version = 5 }
            );
            await context.SaveChangesAsync();
            var battleFormationId = context.BattleFormations.Single().BattleFormationId;
            var service = new BattleFormationService(context);

            var result = await service.UpdateBattleFormation(
                battleFormationId,
                new ChangeBattleFormationDto { Name = "NewName", Version = 3 }
            );

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.ConcurrencyError, result.GetError.Code);
        }

        [Fact]
        public async Task ReturnsUniqueKeyError_WhenNameAlreadyBelongsToAnotherBattleFormation()
        {
            await using var context = CreateContext();
            context.BattleFormations.AddRange(
                new BattleFormation { Name = "TestBattleFormation1", FactionId = 1, Version = 0 },
                new BattleFormation { Name = "TestBattleFormation2", FactionId = 1, Version = 0 }
            );
            await context.SaveChangesAsync();
            var battleFormationId = context.BattleFormations.First(bf => bf.Name == "TestBattleFormation1")
                .BattleFormationId;
            var service = new BattleFormationService(context);

            var result = await service.UpdateBattleFormation(
                battleFormationId,
                new ChangeBattleFormationDto { Name = "TestBattleFormation2", Version = 0 }
            );

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.UniqueKeyError, result.GetError.Code);
        }
    }

    public class DeleteBattleFormation
    {
        [Fact]
        public async Task ReturnsSuccess_WhenBattleFormationExists()
        {
            await using var context = CreateContext();
            context.BattleFormations.Add(new BattleFormation { Name = "TestBattleFormation", FactionId = 1 });
            await context.SaveChangesAsync();
            var battleFormationId = context.BattleFormations.Single().BattleFormationId;
            var service = new BattleFormationService(context);

            var result = await service.DeleteBattleFormation(battleFormationId);

            Assert.True(result.IsSuccess);
            Assert.Empty(context.BattleFormations);
        }

        [Fact]
        public async Task ReturnsNotFound_WhenBattleFormationDoesNotExist()
        {
            await using var context = CreateContext();
            var service = new BattleFormationService(context);

            var result = await service.DeleteBattleFormation(999);

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.NotFound, result.GetError.Code);
        }
    }

    public class CreateBattleFormationAbility
    {
        private static CreateAbilityDto ValidAbilityDto() => new()
        {
            Name = "TestAbility",
            Declaration = "TestDeclaration",
            Effect = "TestEffect",
            Phase = TurnPhase.Hero,
            Turn = PlayerTurn.YourTurn
        };

        [Fact]
        public async Task ReturnsAbility_WhenBattleFormationExistsAndDataIsValid()
        {
            await using var context = CreateContext();
            context.BattleFormations.Add(new BattleFormation { Name = "TestBattleFormation", FactionId = 1 });
            await context.SaveChangesAsync();
            var battleFormationId = context.BattleFormations.Single().BattleFormationId;
            var service = new BattleFormationService(context);
            var createAbilityDto = ValidAbilityDto();

            var result = await service.CreateBattleFormationAbility(battleFormationId, createAbilityDto);

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
        public async Task ReturnsNotFound_WhenBattleFormationDoesNotExist()
        {
            await using var context = CreateContext();
            var service = new BattleFormationService(context);

            var result = await service.CreateBattleFormationAbility(999, ValidAbilityDto());

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.NotFound, result.GetError.Code);
        }

        [Fact]
        public async Task ReturnsValidationError_WhenAbilityDataIsInvalid()
        {
            await using var context = CreateContext();
            context.BattleFormations.Add(new BattleFormation { Name = "TestBattleFormation", FactionId = 1 });
            await context.SaveChangesAsync();
            var battleFormationId = context.BattleFormations.Single().BattleFormationId;
            var service = new BattleFormationService(context);

            var invalidDto = new CreateAbilityDto
            {
                Name = "TestAbility",
                Declaration = "TestDeclaration",
                Effect = "TestEffect",
                Phase = TurnPhase.Passive
            };

            var result = await service.CreateBattleFormationAbility(battleFormationId, invalidDto);

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.ValidationError, result.GetError.Code);
        }
    }

    public class GetBattleFormationAbilities
    {
        [Fact]
        public async Task ReturnsAbilities_WhenBattleFormationExists()
        {
            await using var context = CreateContext();
            context.BattleFormations.Add(new BattleFormation { Name = "TestBattleFormation", FactionId = 1 });
            await context.SaveChangesAsync();
            var battleFormationId = context.BattleFormations.Single().BattleFormationId;
            var service = new BattleFormationService(context);

            await service.CreateBattleFormationAbility(
                battleFormationId,
                new CreateAbilityDto
                {
                    Name = "TestAbility",
                    Declaration = "TestDeclaration",
                    Effect = "TestEffect",
                    Phase = TurnPhase.Hero,
                    Turn = PlayerTurn.YourTurn
                }
            );

            var result = await service.GetBattleFormationAbilities(battleFormationId);

            Assert.True(result.IsSuccess);
            Assert.Single(result.GetValue);
        }

        [Fact]
        public async Task ReturnsNotFound_WhenBattleFormationDoesNotExist()
        {
            await using var context = CreateContext();
            var service = new BattleFormationService(context);

            var result = await service.GetBattleFormationAbilities(999);

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.NotFound, result.GetError.Code);
        }
    }
}
