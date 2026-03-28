using AosAdjutant.Api.Features.AttackProfiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AosAdjutant.Api.Database.Configuration;

public sealed class AttackProfileEntityTypeConfiguration : IEntityTypeConfiguration<AttackProfile>
{
    public void Configure(EntityTypeBuilder<AttackProfile> builder)
    {
        builder.ToTable("attack_profile");
        builder.Property(ap => ap.AttackProfileId).HasColumnName("attack_profile_id");
        builder.Property(ap => ap.Name).HasColumnName("name").HasMaxLength(250);
        builder.Property(ap => ap.IsRanged).HasColumnName("is_ranged");
        builder.Property(ap => ap.Range).HasColumnName("range");
        builder.Property(ap => ap.Attacks).HasColumnName("attacks").HasMaxLength(250);
        builder.Property(ap => ap.ToHit).HasColumnName("to_hit");
        builder.Property(ap => ap.ToWound).HasColumnName("to_wound");
        builder.Property(ap => ap.Rend).HasColumnName("rend");
        builder.Property(ap => ap.Damage).HasColumnName("damage").HasMaxLength(250);
        builder.Property(ap => ap.UnitId).HasColumnName("unit_id");

        builder.HasKey(ap => ap.AttackProfileId);

        builder.HasIndex(ap => new { ap.Name, ap.UnitId }).IsUnique();

        builder.HasMany(ap => ap.WeaponEffects)
            .WithMany()
            .UsingEntity(apwe =>
                {
                    apwe.ToTable("attack_profile_weapon_effect");
                    apwe.Property("AttackProfileId").HasColumnName("attack_profile_id");
                    apwe.Property("WeaponEffectsWeaponEffectId").HasColumnName("weapon_effect_id");
                }
            );

        builder.Property(ap => ap.Version).IsRowVersion();
    }
}
