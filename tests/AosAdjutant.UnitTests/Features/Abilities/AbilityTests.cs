using AosAdjutant.Api.Features.Abilities;
using AosAdjutant.Api.Shared;

namespace AosAdjutant.UnitTests.Features.Abilities;

public class AbilityTests
{
    public class Create
    {
        [Fact]
        public void ReturnsAbility_WhenNonPassiveWithDeclaration()
        {
            var result = Ability.Create(
                "TestAbility",
                null,
                "TestDeclaration",
                "TestEffect",
                TurnPhase.Hero,
                null,
                PlayerTurn.YourTurn,
                false
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
            var result = Ability.Create("TestAbility", null, null, "TestEffect", TurnPhase.Passive, null, null, false);

            Assert.True(result.IsSuccess);
            Assert.Equal(TurnPhase.Passive, result.GetValue.Phase);
        }

        [Fact]
        public void ReturnsValidationError_WhenNonPassiveMissingDeclaration()
        {
            var result = Ability.Create(
                "TestAbility",
                null,
                null,
                "TestEffect",
                TurnPhase.Hero,
                null,
                PlayerTurn.YourTurn,
                false
            );

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.ValidationError, result.GetError.Code);
        }

        [Fact]
        public void ReturnsValidationError_WhenPassiveHasReaction()
        {
            var result = Ability.Create(
                "TestAbility",
                "TestReaction",
                null,
                "TestEffect",
                TurnPhase.Passive,
                null,
                null,
                false
            );

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.ValidationError, result.GetError.Code);
        }

        [Fact]
        public void ReturnsValidationError_WhenPassiveHasDeclaration()
        {
            var result = Ability.Create(
                "TestAbility",
                null,
                "TestDeclaration",
                "TestEffect",
                TurnPhase.Passive,
                null,
                null,
                false
            );

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.ValidationError, result.GetError.Code);
        }

        [Fact]
        public void ReturnsValidationError_WhenPassiveHasRestriction()
        {
            var result = Ability.Create(
                "TestAbility",
                null,
                null,
                "TestEffect",
                TurnPhase.Passive,
                ActivationRestriction.OnceBattle,
                null,
                false
            );

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.ValidationError, result.GetError.Code);
        }

        [Fact]
        public void ReturnsValidationError_WhenPassiveHasTurn()
        {
            var result = Ability.Create(
                "TestAbility",
                null,
                null,
                "TestEffect",
                TurnPhase.Passive,
                null,
                PlayerTurn.YourTurn,
                false
            );

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.ValidationError, result.GetError.Code);
        }
    }

    public class ChangeAbility
    {
        private static Ability CreateValidAbility() =>
            Ability.Create(
                    "TestAbility",
                    null,
                    "TestDeclaration",
                    "TestEffect",
                    TurnPhase.Hero,
                    null,
                    PlayerTurn.YourTurn,
                    false
                )
                .GetValue;

        [Fact]
        public void UpdatesAbility_WhenDataIsValid()
        {
            var ability = CreateValidAbility();

            var result = ability.ChangeAbility(
                "UpdatedAbility",
                null,
                "UpdatedDeclaration",
                "UpdatedEffect",
                TurnPhase.Combat,
                null,
                PlayerTurn.EnemyTurn
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
                "UpdatedAbility",
                null,
                null,
                "UpdatedEffect",
                TurnPhase.Hero,
                null,
                PlayerTurn.EnemyTurn
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
