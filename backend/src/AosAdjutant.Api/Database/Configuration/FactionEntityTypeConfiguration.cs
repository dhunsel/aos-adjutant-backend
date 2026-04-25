using AosAdjutant.Api.Features.Factions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AosAdjutant.Api.Database.Configuration;

public sealed class FactionEntityTypeConfiguration : IEntityTypeConfiguration<Faction>
{
    public void Configure(EntityTypeBuilder<Faction> builder)
    {
        builder.ToTable("faction");
        builder.Property(f => f.FactionId).HasColumnName("faction_id");
        builder.Property(f => f.Name).HasColumnName("name").HasMaxLength(250);
        builder
            .Property(f => f.GrandAlliance)
            .HasColumnName("grand_alliance")
            .HasConversion<string>()
            .HasMaxLength(250);

        builder.HasKey(f => f.FactionId);

        builder.HasIndex(f => f.Name).IsUnique();

        builder
            .HasMany(f => f.BattleFormations)
            .WithOne()
            .HasForeignKey(bf => bf.FactionId)
            .IsRequired();
        builder.HasMany(f => f.Units).WithOne().HasForeignKey(u => u.FactionId).IsRequired();

        builder
            .HasMany(f => f.Abilities)
            .WithMany()
            .UsingEntity(fa =>
            {
                fa.ToTable("faction_ability");
                fa.Property("FactionId").HasColumnName("faction_id");
                fa.Property("AbilitiesAbilityId").HasColumnName("ability_id");
                fa.HasIndex("AbilitiesAbilityId").IsUnique();
            });

        builder.Property(f => f.Version).IsRowVersion();
    }
}
