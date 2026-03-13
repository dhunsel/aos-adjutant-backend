using AosAdjutant.Api.Features.Units;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AosAdjutant.Api.Database.Configuration;

public class UnitEntityTypeConfiguration : IEntityTypeConfiguration<Unit>
{
    public void Configure(EntityTypeBuilder<Unit> builder)
    {
        builder.ToTable("unit");
        builder.Property(u => u.UnitId).HasColumnName("unit_id");
        builder.Property(u => u.Name).HasColumnName("name").HasMaxLength(250);
        builder.Property(u => u.Health).HasColumnName("health");
        builder.Property(u => u.Move).HasColumnName("move").HasMaxLength(250);
        builder.Property(u => u.Save).HasColumnName("save");
        builder.Property(u => u.Control).HasColumnName("control");
        builder.Property(u => u.WardSave).HasColumnName("ward_save");
        builder.Property(u => u.FactionId).HasColumnName("faction_id");

        builder.HasKey(u => u.UnitId);

        builder.HasIndex(u => new { u.FactionId, u.Name }).IsUnique();

        builder.HasMany(u => u.Abilities)
            .WithMany()
            .UsingEntity(ua =>
                {
                    ua.ToTable("unit_ability");
                    ua.Property("UnitId").HasColumnName("unit_id");
                    ua.Property("AbilitiesAbilityId").HasColumnName("ability_id");
                    ua.HasIndex("AbilitiesAbilityId").IsUnique();
                }
            );

        builder.Property(u => u.Version).IsRowVersion();
    }
}
