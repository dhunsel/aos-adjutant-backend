using AosAdjutant.Api.Common;
using AosAdjutant.Api.Features.Abilities;

namespace AosAdjutant.UnitTests.Features.Abilities;

public class AbilityTests
{
    public class Create
    {
        [Fact]
        public void ReturnsAbility_WhenNonPassiveWithDeclaration()
        {
            var result = Ability.Create(
                new AbilityData
                {
                    Name = "TestAbility",
                    Declaration = "TestDeclaration",
                    Effect = "TestEffect",
                    Phase = Phase.Hero,
                    Turn = Turn.YourTurn,
                    IsGeneric = false,
                }
            );

            Assert.True(result.IsSuccess);
            Assert.Equivalent(
                new
                {
                    Name = "TestAbility",
                    Reaction = (string?)null,
                    Declaration = "TestDeclaration",
                    Effect = "TestEffect",
                    Phase = Phase.Hero,
                    Restriction = (Restriction?)null,
                    Turn = (Turn?)Turn.YourTurn,
                    IsGeneric = false,
                },
                result.GetValue
            );
        }

        [Fact]
        public void ReturnsAbility_WhenPassiveWithNoExtraFields()
        {
            var result = Ability.Create(
                new AbilityData
                {
                    Name = "TestAbility",
                    Effect = "TestEffect",
                    Phase = Phase.Passive,
                    IsGeneric = false,
                }
            );

            Assert.True(result.IsSuccess);
            Assert.Equal(Phase.Passive, result.GetValue.Phase);
        }

        [Fact]
        public void ReturnsValidationError_WhenNonPassiveMissingDeclaration()
        {
            var result = Ability.Create(
                new AbilityData
                {
                    Name = "TestAbility",
                    Effect = "TestEffect",
                    Phase = Phase.Hero,
                    Turn = Turn.YourTurn,
                    IsGeneric = false,
                }
            );

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.ValidationError, result.GetError.Code);
        }

        [Fact]
        public void ReturnsValidationError_WhenPassiveHasReaction()
        {
            var result = Ability.Create(
                new AbilityData
                {
                    Name = "TestAbility",
                    Reaction = "TestReaction",
                    Effect = "TestEffect",
                    Phase = Phase.Passive,
                    IsGeneric = false,
                }
            );

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.ValidationError, result.GetError.Code);
        }

        [Fact]
        public void ReturnsValidationError_WhenPassiveHasDeclaration()
        {
            var result = Ability.Create(
                new AbilityData
                {
                    Name = "TestAbility",
                    Declaration = "TestDeclaration",
                    Effect = "TestEffect",
                    Phase = Phase.Passive,
                    IsGeneric = false,
                }
            );

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.ValidationError, result.GetError.Code);
        }

        [Fact]
        public void ReturnsValidationError_WhenPassiveHasRestriction()
        {
            var result = Ability.Create(
                new AbilityData
                {
                    Name = "TestAbility",
                    Effect = "TestEffect",
                    Phase = Phase.Passive,
                    Restriction = Restriction.OnceBattle,
                    IsGeneric = false,
                }
            );

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.ValidationError, result.GetError.Code);
        }

        [Fact]
        public void ReturnsValidationError_WhenPassiveHasTurn()
        {
            var result = Ability.Create(
                new AbilityData
                {
                    Name = "TestAbility",
                    Effect = "TestEffect",
                    Phase = Phase.Passive,
                    Turn = Turn.YourTurn,
                    IsGeneric = false,
                }
            );

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.ValidationError, result.GetError.Code);
        }
    }

    public class ChangeAbility
    {
        private static Ability CreateValidAbility() =>
            Ability
                .Create(
                    new AbilityData
                    {
                        Name = "TestAbility",
                        Declaration = "TestDeclaration",
                        Effect = "TestEffect",
                        Phase = Phase.Hero,
                        Turn = Turn.YourTurn,
                        IsGeneric = false,
                    }
                )
                .GetValue;

        [Fact]
        public void UpdatesAbility_WhenDataIsValid()
        {
            var ability = CreateValidAbility();

            var result = ability.ChangeAbility(
                new AbilityData
                {
                    Name = "UpdatedAbility",
                    Declaration = "UpdatedDeclaration",
                    Effect = "UpdatedEffect",
                    Phase = Phase.Combat,
                    Turn = Turn.EnemyTurn,
                    IsGeneric = false,
                }
            );

            Assert.True(result.IsSuccess);
            Assert.Equivalent(
                new
                {
                    Name = "UpdatedAbility",
                    Reaction = (string?)null,
                    Declaration = "UpdatedDeclaration",
                    Effect = "UpdatedEffect",
                    Phase = Phase.Combat,
                    Restriction = (Restriction?)null,
                    Turn = (Turn?)Turn.EnemyTurn,
                },
                ability
            );
        }

        [Fact]
        public void DoesNotMutateAbility_WhenValidationFails()
        {
            var ability = CreateValidAbility();

            var result = ability.ChangeAbility(
                new AbilityData
                {
                    Name = "UpdatedAbility",
                    Effect = "UpdatedEffect",
                    Phase = Phase.Hero,
                    Turn = Turn.EnemyTurn,
                    IsGeneric = false,
                }
            );

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.ValidationError, result.GetError.Code);
            Assert.Equivalent(
                new
                {
                    Name = "TestAbility",
                    Reaction = (string?)null,
                    Declaration = "TestDeclaration",
                    Effect = "TestEffect",
                    Phase = Phase.Hero,
                    Restriction = (Restriction?)null,
                    Turn = (Turn?)Turn.YourTurn,
                },
                ability
            );
        }
    }
}
