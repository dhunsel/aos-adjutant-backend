using AosAdjutant.Api.Common;
using AosAdjutant.Api.Database;
using AosAdjutant.Api.Features.Abilities;
using Microsoft.EntityFrameworkCore;

namespace AosAdjutant.UnitTests.Features.Abilities;

public class AbilityServiceTests
{
    private static ApplicationDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

    public class CreateGenericAbility
    {
        [Fact]
        public async Task ReturnsAbility_WhenDataIsValid()
        {
            await using var context = CreateContext();
            var service = new AbilityService(context);
            var createAbilityDto = new CreateAbilityDto
            {
                Name = "TestAbility",
                Declaration = "TestDeclaration",
                Effect = "TestEffect",
                Phase = TurnPhase.Hero,
                Turn = PlayerTurn.YourTurn
            };

            var result = await service.CreateGenericAbility(createAbilityDto);

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
                    IsGeneric = true
                },
                result.GetValue
            );
        }

        [Fact]
        public async Task ReturnsValidationError_WhenAbilityDataIsInvalid()
        {
            await using var context = CreateContext();
            var service = new AbilityService(context);

            var result = await service.CreateGenericAbility(
                new CreateAbilityDto
                {
                    Name = "TestAbility",
                    Declaration = "TestDeclaration",
                    Effect = "TestEffect",
                    Phase = TurnPhase.Passive
                }
            );

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.ValidationError, result.GetError.Code);
        }
    }

    public class GetAbility
    {
        [Fact]
        public async Task ReturnsAbility_WhenFound()
        {
            await using var context = CreateContext();
            var ability = new Ability
            {
                Name = "TestAbility", Effect = "TestEffect", Phase = TurnPhase.Hero, Declaration = "TestDeclaration"
            };
            context.Abilities.Add(ability);
            await context.SaveChangesAsync();
            var abilityId = context.Abilities.Single().AbilityId;
            var service = new AbilityService(context);

            var result = await service.GetAbility(abilityId);

            Assert.True(result.IsSuccess);
            Assert.Equivalent(ability, result.GetValue);
        }

        [Fact]
        public async Task ReturnsNotFound_WhenAbilityDoesNotExist()
        {
            await using var context = CreateContext();
            var service = new AbilityService(context);

            var result = await service.GetAbility(999);

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.NotFound, result.GetError.Code);
        }
    }

    public class UpdateAbility
    {
        [Fact]
        public async Task ReturnsAbility_WhenUpdateSucceeds()
        {
            await using var context = CreateContext();
            context.Abilities.Add(
                new Ability
                {
                    Name = "TestAbility",
                    Effect = "TestEffect",
                    Phase = TurnPhase.Hero,
                    Declaration = "TestDeclaration",
                    Version = 0
                }
            );
            await context.SaveChangesAsync();
            var abilityId = context.Abilities.Single().AbilityId;
            var service = new AbilityService(context);
            var changeAbilityDto = new ChangeAbilityDto
            {
                Name = "UpdatedAbility",
                Declaration = "UpdatedDeclaration",
                Effect = "UpdatedEffect",
                Phase = TurnPhase.Combat,
                Turn = PlayerTurn.EnemyTurn,
                Version = 0
            };

            var result = await service.UpdateAbility(abilityId, changeAbilityDto);

            Assert.True(result.IsSuccess);
            Assert.Equivalent(
                new
                {
                    changeAbilityDto.Name,
                    changeAbilityDto.Reaction,
                    changeAbilityDto.Declaration,
                    changeAbilityDto.Effect,
                    changeAbilityDto.Phase,
                    changeAbilityDto.Restriction,
                    changeAbilityDto.Turn
                },
                result.GetValue
            );
        }

        [Fact]
        public async Task ReturnsNotFound_WhenAbilityDoesNotExist()
        {
            await using var context = CreateContext();
            var service = new AbilityService(context);

            var result = await service.UpdateAbility(
                999,
                new ChangeAbilityDto
                {
                    Name = "UpdatedAbility",
                    Declaration = "UpdatedDeclaration",
                    Effect = "UpdatedEffect",
                    Phase = TurnPhase.Combat,
                    Turn = PlayerTurn.EnemyTurn,
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
            context.Abilities.Add(
                new Ability
                {
                    Name = "TestAbility",
                    Effect = "TestEffect",
                    Phase = TurnPhase.Hero,
                    Declaration = "TestDeclaration",
                    Version = 5
                }
            );
            await context.SaveChangesAsync();
            var abilityId = context.Abilities.Single().AbilityId;
            var service = new AbilityService(context);

            var result = await service.UpdateAbility(
                abilityId,
                new ChangeAbilityDto
                {
                    Name = "UpdatedAbility",
                    Declaration = "UpdatedDeclaration",
                    Effect = "UpdatedEffect",
                    Phase = TurnPhase.Combat,
                    Turn = PlayerTurn.EnemyTurn,
                    Version = 3
                }
            );

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.ConcurrencyError, result.GetError.Code);
        }

        [Fact]
        public async Task ReturnsValidationError_WhenAbilityDataIsInvalid()
        {
            await using var context = CreateContext();
            context.Abilities.Add(
                new Ability
                {
                    Name = "TestAbility",
                    Effect = "TestEffect",
                    Phase = TurnPhase.Hero,
                    Declaration = "TestDeclaration",
                    Version = 0
                }
            );
            await context.SaveChangesAsync();
            var abilityId = context.Abilities.Single().AbilityId;
            var service = new AbilityService(context);

            var result = await service.UpdateAbility(
                abilityId,
                new ChangeAbilityDto
                {
                    Name = "TestAbility",
                    Effect = "TestEffect",
                    Phase = TurnPhase.Passive,
                    Turn = PlayerTurn.YourTurn,
                    Version = 0
                }
            );

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.ValidationError, result.GetError.Code);
        }
    }

    public class DeleteAbility
    {
        [Fact]
        public async Task ReturnsSuccess_WhenAbilityExists()
        {
            await using var context = CreateContext();
            context.Abilities.Add(
                new Ability
                {
                    Name = "TestAbility",
                    Effect = "TestEffect",
                    Phase = TurnPhase.Hero,
                    Declaration = "TestDeclaration"
                }
            );
            await context.SaveChangesAsync();
            var abilityId = context.Abilities.Single().AbilityId;
            var service = new AbilityService(context);

            var result = await service.DeleteAbility(abilityId);

            Assert.True(result.IsSuccess);
            Assert.Empty(context.Abilities);
        }

        [Fact]
        public async Task ReturnsNotFound_WhenAbilityDoesNotExist()
        {
            await using var context = CreateContext();
            var service = new AbilityService(context);

            var result = await service.DeleteAbility(999);

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.NotFound, result.GetError.Code);
        }
    }
}
