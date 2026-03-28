using AosAdjutant.Api.Features.WeaponEffects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AosAdjutant.Api.Database.Configuration;

public sealed class WeaponEffectEntityTypeConfiguration : IEntityTypeConfiguration<WeaponEffect>
{
    public void Configure(EntityTypeBuilder<WeaponEffect> builder)
    {
        builder.ToTable("weapon_effect");
        builder.Property(we => we.WeaponEffectId).HasColumnName("weapon_effect_id");
        builder.Property(we => we.Key).HasColumnName("key").HasMaxLength(250);
        builder.Property(we => we.Name).HasColumnName("name").HasMaxLength(250);

        builder.HasKey(we => we.WeaponEffectId);

        builder.HasIndex(we => we.Key).IsUnique();
        builder.HasIndex(we => we.Name).IsUnique();

        builder.HasData(
            new WeaponEffect { WeaponEffectId = 1, Key = "crit_2_hits", Name = "Crit (2 Hits)" },
            new WeaponEffect { WeaponEffectId = 2, Key = "crit_auto_wound", Name = "Crit (Auto-wound)" },
            new WeaponEffect { WeaponEffectId = 3, Key = "crit_mortal", Name = "Crit (Mortal)" },
            new WeaponEffect { WeaponEffectId = 4, Key = "charge_1_damage", Name = "Charge (+1 Damage)" },
            new WeaponEffect { WeaponEffectId = 5, Key = "companion", Name = "Companion" },
            new WeaponEffect { WeaponEffectId = 6, Key = "shoot_in_combat", Name = "Shoot in Combat" },
            new WeaponEffect { WeaponEffectId = 7, Key = "anti_monster", Name = "Anti-Monster (+1 Rend)" },
            new WeaponEffect { WeaponEffectId = 8, Key = "anti_hero", Name = "Anti-Hero (+1 Rend)" },
            new WeaponEffect { WeaponEffectId = 9, Key = "anti_infantry", Name = "Anti-Infantry (+1 Rend)" },
            new WeaponEffect { WeaponEffectId = 10, Key = "anti_cavalry", Name = "Anti-Cavalry (+1 Rend)" },
            new WeaponEffect { WeaponEffectId = 11, Key = "anti_wizard", Name = "Anti-Wizard (+1 Rend)" },
            new WeaponEffect { WeaponEffectId = 12, Key = "anti_manifestation", Name = "Anti-Manifestation (+1 Rend)" },
            new WeaponEffect { WeaponEffectId = 13, Key = "anti_charge", Name = "Anti-Charge (+1 Rend)" }
        );
    }
}
