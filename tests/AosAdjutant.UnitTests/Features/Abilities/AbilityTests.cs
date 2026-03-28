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
                    Phase = TurnPhase.Hero,
                    Turn = PlayerTurn.YourTurn,
                    IsGeneric = false
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
                    Phase = TurnPhase.Hero,
                    Restriction = (ActivationRestriction?)null,
                    Turn = (PlayerTurn?)PlayerTurn.YourTurn,
                    IsGeneric = false
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
                    Name = "TestAbility", Effect = "TestEffect", Phase = TurnPhase.Passive, IsGeneric = false
                }
            );

            Assert.True(result.IsSuccess);
            Assert.Equal(TurnPhase.Passive, result.GetValue.Phase);
        }

        [Fact]
        public void ReturnsValidationError_WhenNonPassiveMissingDeclaration()
        {
            var result = Ability.Create(
                new AbilityData
                {
                    Name = "TestAbility",
                    Effect = "TestEffect",
                    Phase = TurnPhase.Hero,
                    Turn = PlayerTurn.YourTurn,
                    IsGeneric = false
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
                    Phase = TurnPhase.Passive,
                    IsGeneric = false
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
                    Phase = TurnPhase.Passive,
                    IsGeneric = false
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
                    Phase = TurnPhase.Passive,
                    Restriction = ActivationRestriction.OnceBattle,
                    IsGeneric = false
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
                    Phase = TurnPhase.Passive,
                    Turn = PlayerTurn.YourTurn,
                    IsGeneric = false
                }
            );

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.ValidationError, result.GetError.Code);
        }
    }

    public class ChangeAbility
    {
        private static Ability CreateValidAbility() =>
            Ability.Create(
                    new AbilityData
                    {
                        Name = "TestAbility",
                        Declaration = "TestDeclaration",
                        Effect = "TestEffect",
                        Phase = TurnPhase.Hero,
                        Turn = PlayerTurn.YourTurn,
                        IsGeneric = false
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
                    Phase = TurnPhase.Combat,
                    Turn = PlayerTurn.EnemyTurn,
                    IsGeneric = false
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
                    Phase = TurnPhase.Combat,
                    Restriction = (ActivationRestriction?)null,
                    Turn = (PlayerTurn?)PlayerTurn.EnemyTurn
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
                    Phase = TurnPhase.Hero,
                    Turn = PlayerTurn.EnemyTurn,
                    IsGeneric = false
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
                    Phase = TurnPhase.Hero,
                    Restriction = (ActivationRestriction?)null,
                    Turn = (PlayerTurn?)PlayerTurn.YourTurn
                },
                ability
            );
        }
    }
}
