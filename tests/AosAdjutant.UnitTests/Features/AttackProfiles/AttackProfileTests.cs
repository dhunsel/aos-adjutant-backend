using AosAdjutant.Api.Common;
using AosAdjutant.Api.Features.AttackProfiles;

namespace AosAdjutant.UnitTests.Features.AttackProfiles;

public class AttackProfileTests
{
    public class Create
    {
        [Fact]
        public void ReturnsAttackProfile_WhenMeleeWithNoRange()
        {
            var result = AttackProfile.Create(
                new AttackProfileData
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
            var result = AttackProfile.Create(
                new AttackProfileData
                {
                    Name = "TestProfile",
                    IsRanged = true,
                    Range = 18,
                    Attacks = "2",
                    ToHit = 3,
                    ToWound = 3,
                    Damage = "1",
                    UnitId = 1
                }
            );

            Assert.True(result.IsSuccess);
            Assert.Equal(18, result.GetValue.Range);
        }

        [Fact]
        public void ReturnsValidationError_WhenRangedWithoutRange()
        {
            var result = AttackProfile.Create(
                new AttackProfileData
                {
                    Name = "TestProfile",
                    IsRanged = true,
                    Attacks = "2",
                    ToHit = 3,
                    ToWound = 3,
                    Damage = "1",
                    UnitId = 1
                }
            );

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.ValidationError, result.GetError.Code);
        }

        [Fact]
        public void ReturnsValidationError_WhenMeleeWithRange()
        {
            var result = AttackProfile.Create(
                new AttackProfileData
                {
                    Name = "TestProfile",
                    IsRanged = false,
                    Range = 18,
                    Attacks = "2",
                    ToHit = 3,
                    ToWound = 3,
                    Damage = "1",
                    UnitId = 1
                }
            );

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.ValidationError, result.GetError.Code);
        }

        [Fact]
        public void ReturnsValidationError_WhenToHitBelowMinimum()
        {
            var result = AttackProfile.Create(
                new AttackProfileData
                {
                    Name = "TestProfile",
                    IsRanged = false,
                    Attacks = "2",
                    ToHit = 1,
                    ToWound = 3,
                    Damage = "1",
                    UnitId = 1
                }
            );

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.ValidationError, result.GetError.Code);
        }

        [Fact]
        public void ReturnsValidationError_WhenToHitAboveMaximum()
        {
            var result = AttackProfile.Create(
                new AttackProfileData
                {
                    Name = "TestProfile",
                    IsRanged = false,
                    Attacks = "2",
                    ToHit = 8,
                    ToWound = 3,
                    Damage = "1",
                    UnitId = 1
                }
            );

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.ValidationError, result.GetError.Code);
        }

        [Fact]
        public void ReturnsValidationError_WhenToWoundBelowMinimum()
        {
            var result = AttackProfile.Create(
                new AttackProfileData
                {
                    Name = "TestProfile",
                    IsRanged = false,
                    Attacks = "2",
                    ToHit = 3,
                    ToWound = 1,
                    Damage = "1",
                    UnitId = 1
                }
            );

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.ValidationError, result.GetError.Code);
        }

        [Fact]
        public void ReturnsValidationError_WhenToWoundAboveMaximum()
        {
            var result = AttackProfile.Create(
                new AttackProfileData
                {
                    Name = "TestProfile",
                    IsRanged = false,
                    Attacks = "2",
                    ToHit = 3,
                    ToWound = 8,
                    Damage = "1",
                    UnitId = 1
                }
            );

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.ValidationError, result.GetError.Code);
        }
    }

    public class Change
    {
        private static AttackProfile CreateValidMeleeProfile() =>
            AttackProfile.Create(
                    new AttackProfileData
                    {
                        Name = "TestProfile",
                        IsRanged = false,
                        Attacks = "2",
                        ToHit = 3,
                        ToWound = 3,
                        Damage = "1",
                        UnitId = 1
                    }
                )
                .GetValue;

        [Fact]
        public void UpdatesAttackProfile_WhenDataIsValid()
        {
            var attackProfile = CreateValidMeleeProfile();

            var result = attackProfile.Change(
                new AttackProfileData
                {
                    Name = "UpdatedProfile",
                    IsRanged = true,
                    Range = 24,
                    Attacks = "3",
                    ToHit = 4,
                    ToWound = 4,
                    Rend = 1,
                    Damage = "2",
                    UnitId = 1
                }
            );

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

            var result = attackProfile.Change(
                new AttackProfileData
                {
                    Name = "UpdatedProfile",
                    IsRanged = true,
                    Attacks = "3",
                    ToHit = 4,
                    ToWound = 4,
                    Rend = 1,
                    Damage = "2",
                    UnitId = 1
                }
            );

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
