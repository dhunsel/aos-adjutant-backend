using AosAdjutant.Api.Common;
using AosAdjutant.Api.Database;
using AosAdjutant.Api.Features.Abilities;
using AosAdjutant.Api.Features.Factions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace AosAdjutant.UnitTests.Features.Factions;

public class FactionServiceTests
{
    private static ApplicationDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

    public class CreateFaction
    {
        [Fact]
        public async Task ReturnsCreatedFaction_WhenNameIsUnique()
        {
            await using var context = CreateContext();
            var service = new FactionService(context, NullLogger<FactionService>.Instance);
            const string Name = "TestFaction";

            var result = await service.CreateFaction(new CreateFactionDto { Name = Name });

            Assert.True(result.IsSuccess);
            Assert.Equal(Name, result.GetValue.Name);
        }

        [Fact]
        public async Task ReturnsUniqueKeyError_WhenNameAlreadyExists()
        {
            await using var context = CreateContext();
            const string Name = "TestFaction";
            context.Factions.Add(new Faction { Name = Name });
            await context.SaveChangesAsync();
            var service = new FactionService(context, NullLogger<FactionService>.Instance);

            var result = await service.CreateFaction(new CreateFactionDto { Name = Name });

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.UniqueKeyError, result.GetError.Code);
        }
    }

    public class GetFactions
    {
        [Fact]
        public async Task ReturnsAllFactions()
        {
            await using var context = CreateContext();
            context.Factions.AddRange(new Faction { Name = "TestFaction1" }, new Faction { Name = "TestFaction2" });
            await context.SaveChangesAsync();
            var service = new FactionService(context, NullLogger<FactionService>.Instance);

            var result = await service.GetFactions();

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task ReturnsEmptyList_WhenNoFactionsExist()
        {
            await using var context = CreateContext();
            var service = new FactionService(context, NullLogger<FactionService>.Instance);

            var result = await service.GetFactions();

            Assert.Empty(result);
        }
    }

    public class GetFaction
    {
        [Fact]
        public async Task ReturnsFaction_WhenFound()
        {
            await using var context = CreateContext();
            const string Name = "TestFaction";
            context.Factions.Add(new Faction { Name = Name });
            await context.SaveChangesAsync();
            var factionId = context.Factions.Single().FactionId;
            var service = new FactionService(context, NullLogger<FactionService>.Instance);

            var result = await service.GetFaction(factionId);

            Assert.True(result.IsSuccess);
            Assert.Equal(Name, result.GetValue.Name);
        }

        [Fact]
        public async Task ReturnsNotFound_WhenFactionDoesNotExist()
        {
            await using var context = CreateContext();
            var service = new FactionService(context, NullLogger<FactionService>.Instance);

            var result = await service.GetFaction(999);

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.NotFound, result.GetError.Code);
        }
    }

    public class ChangeFaction
    {
        [Fact]
        public async Task ReturnsFaction_WhenUpdateSucceeds()
        {
            await using var context = CreateContext();
            context.Factions.Add(new Faction { Name = "TestFaction", Version = 0 });
            await context.SaveChangesAsync();
            var factionId = context.Factions.Single().FactionId;
            var service = new FactionService(context, NullLogger<FactionService>.Instance);
            const string NewName = "TestFactionUpdated";

            var result = await service.ChangeFaction(factionId, new ChangeFactionDto { Name = NewName, Version = 0 });

            Assert.True(result.IsSuccess);
            Assert.Equal(NewName, result.GetValue.Name);
        }

        [Fact]
        public async Task ReturnsNotFound_WhenFactionDoesNotExist()
        {
            await using var context = CreateContext();
            var service = new FactionService(context, NullLogger<FactionService>.Instance);

            var result = await service.ChangeFaction(
                999,
                new ChangeFactionDto { Name = "TestFactionUpdated", Version = 0 }
            );

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.NotFound, result.GetError.Code);
        }

        [Fact]
        public async Task ReturnsConcurrencyError_WhenVersionMismatch()
        {
            await using var context = CreateContext();
            context.Factions.Add(new Faction { Name = "TestFaction", Version = 5 });
            await context.SaveChangesAsync();
            var factionId = context.Factions.Single().FactionId;
            var service = new FactionService(context, NullLogger<FactionService>.Instance);

            var result = await service.ChangeFaction(
                factionId,
                new ChangeFactionDto { Name = "TestFactionUpdated", Version = 3 }
            );

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.ConcurrencyError, result.GetError.Code);
        }

        [Fact]
        public async Task ReturnsUniqueKeyError_WhenNameBelongsToAnotherFaction()
        {
            await using var context = CreateContext();
            context.Factions.AddRange(
                new Faction { Name = "TestFaction1", Version = 0 },
                new Faction { Name = "TestFaction2", Version = 0 }
            );
            await context.SaveChangesAsync();
            var factionId = context.Factions.First(f => f.Name == "TestFaction1").FactionId;
            var service = new FactionService(context, NullLogger<FactionService>.Instance);

            var result = await service.ChangeFaction(
                factionId,
                new ChangeFactionDto { Name = "TestFaction2", Version = 0 }
            );

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.UniqueKeyError, result.GetError.Code);
        }
    }

    public class DeleteFaction
    {
        [Fact]
        public async Task ReturnsSuccess_WhenFactionExists()
        {
            await using var context = CreateContext();
            context.Factions.Add(new Faction { Name = "TestFaction" });
            await context.SaveChangesAsync();
            var factionId = context.Factions.Single().FactionId;
            var service = new FactionService(context, NullLogger<FactionService>.Instance);

            var result = await service.DeleteFaction(factionId);

            Assert.True(result.IsSuccess);
            Assert.Empty(context.Factions);
        }

        [Fact]
        public async Task ReturnsNotFound_WhenFactionDoesNotExist()
        {
            await using var context = CreateContext();
            var service = new FactionService(context, NullLogger<FactionService>.Instance);

            var result = await service.DeleteFaction(999);

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.NotFound, result.GetError.Code);
        }
    }

    public class CreateFactionAbility
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
        public async Task ReturnsAbility_WhenFactionExistsAndDataIsValid()
        {
            await using var context = CreateContext();
            context.Factions.Add(new Faction { Name = "TestFaction" });
            await context.SaveChangesAsync();
            var factionId = context.Factions.Single().FactionId;
            var service = new FactionService(context, NullLogger<FactionService>.Instance);

            var result = await service.CreateFactionAbility(factionId, ValidAbilityDto());

            Assert.True(result.IsSuccess);
            Assert.Equivalent(
                new
                {
                    Name = "TestAbility",
                    Reaction = (string?)null,
                    Declaration = "TestDeclaration",
                    Effect = "TestEffect",
                    Phase = TurnPhase.Hero,
                    Restriction = (ActivationRestriction?)null,
                    Turn = (PlayerTurn?)PlayerTurn.YourTurn,
                    IsGeneric = false
                },
                result.GetValue
            );
        }

        [Fact]
        public async Task ReturnsNotFound_WhenFactionDoesNotExist()
        {
            await using var context = CreateContext();
            var service = new FactionService(context, NullLogger<FactionService>.Instance);

            var result = await service.CreateFactionAbility(999, ValidAbilityDto());

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.NotFound, result.GetError.Code);
        }

        [Fact]
        public async Task ReturnsValidationError_WhenAbilityDataIsInvalid()
        {
            await using var context = CreateContext();
            context.Factions.Add(new Faction { Name = "TestFaction" });
            await context.SaveChangesAsync();
            var factionId = context.Factions.Single().FactionId;
            var service = new FactionService(context, NullLogger<FactionService>.Instance);

            // Passive ability with a declaration is invalid
            var invalidDto = new CreateAbilityDto
            {
                Name = "TestAbility",
                Declaration = "TestDeclaration",
                Effect = "TestEffect",
                Phase = TurnPhase.Passive
            };

            var result = await service.CreateFactionAbility(factionId, invalidDto);

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.ValidationError, result.GetError.Code);
        }
    }

    public class GetFactionAbilities
    {
        [Fact]
        public async Task ReturnsAbilities_WhenFactionExists()
        {
            await using var context = CreateContext();
            context.Factions.Add(new Faction { Name = "TestFaction" });
            await context.SaveChangesAsync();
            var factionId = context.Factions.Single().FactionId;
            var service = new FactionService(context, NullLogger<FactionService>.Instance);

            await service.CreateFactionAbility(
                factionId,
                new CreateAbilityDto
                {
                    Name = "TestAbility",
                    Declaration = "TestDeclaration",
                    Effect = "TestEffect",
                    Phase = TurnPhase.Hero,
                    Turn = PlayerTurn.YourTurn
                }
            );

            var result = await service.GetFactionAbilities(factionId);

            Assert.True(result.IsSuccess);
            Assert.Single(result.GetValue);
        }

        [Fact]
        public async Task ReturnsNotFound_WhenFactionDoesNotExist()
        {
            await using var context = CreateContext();
            var service = new FactionService(context, NullLogger<FactionService>.Instance);

            var result = await service.GetFactionAbilities(999);

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.NotFound, result.GetError.Code);
        }
    }
}
