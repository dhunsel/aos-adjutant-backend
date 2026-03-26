using AosAdjutant.Api.Features.AttackProfiles;
using AosAdjutant.Api.Shared;

namespace AosAdjutant.UnitTests.Features.AttackProfiles;

public class AttackProfileTests
{
    public class Create
    {
        [Fact]
        public void ReturnsAttackProfile_WhenMeleeWithNoRange()
        {
            var result = AttackProfile.Create("TestProfile", false, null, "2", 3, 3, null, "1", 1);

            Assert.True(result.IsSuccess);
            Assert.Equivalent(
                new
                {
                    Name = "TestProfile",
                    IsRanged = false,
                    Range = (int?)null,
                    Attacks = "2",
                    ToHit = 3,
                    ToWound = 3,
                    Rend = (int?)null,
                    Damage = "1",
                    UnitId = 1
                },
                result.GetValue
            );
        }

        [Fact]
        public void ReturnsAttackProfile_WhenRangedWithRange()
        {
            var result = AttackProfile.Create("TestProfile", true, 18, "2", 3, 3, null, "1", 1);

            Assert.True(result.IsSuccess);
            Assert.Equal(18, result.GetValue.Range);
        }

        [Fact]
        public void ReturnsValidationError_WhenRangedWithoutRange()
        {
            var result = AttackProfile.Create("TestProfile", true, null, "2", 3, 3, null, "1", 1);

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.ValidationError, result.GetError.Code);
        }

        [Fact]
        public void ReturnsValidationError_WhenMeleeWithRange()
        {
            var result = AttackProfile.Create("TestProfile", false, 18, "2", 3, 3, null, "1", 1);

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.ValidationError, result.GetError.Code);
        }

        [Fact]
        public void ReturnsValidationError_WhenToHitBelowMinimum()
        {
            var result = AttackProfile.Create("TestProfile", false, null, "2", 1, 3, null, "1", 1);

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.ValidationError, result.GetError.Code);
        }

        [Fact]
        public void ReturnsValidationError_WhenToHitAboveMaximum()
        {
            var result = AttackProfile.Create("TestProfile", false, null, "2", 8, 3, null, "1", 1);

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.ValidationError, result.GetError.Code);
        }

        [Fact]
        public void ReturnsValidationError_WhenToWoundBelowMinimum()
        {
            var result = AttackProfile.Create("TestProfile", false, null, "2", 3, 1, null, "1", 1);

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.ValidationError, result.GetError.Code);
        }

        [Fact]
        public void ReturnsValidationError_WhenToWoundAboveMaximum()
        {
            var result = AttackProfile.Create("TestProfile", false, null, "2", 3, 8, null, "1", 1);

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.ValidationError, result.GetError.Code);
        }
    }

    public class Change
    {
        private static AttackProfile CreateValidMeleeProfile() =>
            AttackProfile.Create("TestProfile", false, null, "2", 3, 3, null, "1", 1).GetValue;

        [Fact]
        public void UpdatesAttackProfile_WhenDataIsValid()
        {
            var attackProfile = CreateValidMeleeProfile();

            var result = attackProfile.Change("UpdatedProfile", true, 24, "3", 4, 4, 1, "2");

            Assert.True(result.IsSuccess);
            Assert.Equivalent(
                new
                {
                    Name = "UpdatedProfile",
                    IsRanged = true,
                    Range = (int?)24,
                    Attacks = "3",
                    ToHit = 4,
                    ToWound = 4,
                    Rend = (int?)1,
                    Damage = "2"
                },
                attackProfile
            );
        }

        [Fact]
        public void DoesNotMutateAttackProfile_WhenValidationFails()
        {
            var attackProfile = CreateValidMeleeProfile();

            var result = attackProfile.Change("UpdatedProfile", true, null, "3", 4, 4, 1, "2");

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.ValidationError, result.GetError.Code);
            Assert.Equivalent(
                new
                {
                    Name = "TestProfile",
                    IsRanged = false,
                    Range = (int?)null,
                    Attacks = "2",
                    ToHit = 3,
                    ToWound = 3,
                    Rend = (int?)null,
                    Damage = "1"
                },
                attackProfile
            );
        }
    }
}
