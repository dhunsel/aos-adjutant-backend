using AosAdjutant.Api.Features.WeaponEffects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AosAdjutant.Api.Database.Configuration;

public class WeaponEffectEntityTypeConfiguration : IEntityTypeConfiguration<WeaponEffect>
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

        builder.Property(we => we.Version).IsRowVersion();
    }
}
