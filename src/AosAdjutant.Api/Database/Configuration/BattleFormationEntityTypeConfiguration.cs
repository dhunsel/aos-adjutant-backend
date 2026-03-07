using AosAdjutant.Api.Features.Factions;
using AosAdjutant.Api.Features.Factions.BattleFormations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AosAdjutant.Api.Database.Configuration;

public class BattleFormationEntityTypeConfiguration : IEntityTypeConfiguration<BattleFormation>
{
    public void Configure(EntityTypeBuilder<BattleFormation> builder)
    {
        builder.ToTable("battle_formation");
        builder.Property(bf => bf.BattleFormationId).HasColumnName("battle_formation_id");
        builder.Property(bf => bf.Name).HasColumnName("name").HasMaxLength(250);
        builder.Property(bf => bf.FactionId).HasColumnName("faction_id");
        builder.Property(bf => bf.AbilityId).HasColumnName("ability_id");

        builder.HasKey(bf => bf.BattleFormationId);

        builder.HasIndex(bf => new { bf.FactionId, bf.Name }).IsUnique();

        builder.HasOne(bf => bf.Faction)
            .WithMany(f => f.BattleFormations)
            .HasForeignKey(bf => bf.FactionId)
            .IsRequired();

        builder.HasOne(bf => bf.Ability).WithOne().HasForeignKey<BattleFormation>(bf => bf.AbilityId).IsRequired();

        builder.Property(bf => bf.Version).IsRowVersion();
    }
}
