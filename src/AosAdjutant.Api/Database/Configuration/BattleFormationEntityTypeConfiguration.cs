using AosAdjutant.Api.Features.BattleFormations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AosAdjutant.Api.Database.Configuration;

public sealed class BattleFormationEntityTypeConfiguration : IEntityTypeConfiguration<BattleFormation>
{
    public void Configure(EntityTypeBuilder<BattleFormation> builder)
    {
        builder.ToTable("battle_formation");
        builder.Property(bf => bf.BattleFormationId).HasColumnName("battle_formation_id");
        builder.Property(bf => bf.Name).HasColumnName("name").HasMaxLength(250);
        builder.Property(bf => bf.FactionId).HasColumnName("faction_id");

        builder.HasKey(bf => bf.BattleFormationId);

        builder.HasIndex(bf => new { bf.FactionId, bf.Name }).IsUnique();

        builder.HasMany(bf => bf.Abilities)
            .WithMany()
            .UsingEntity(bfa =>
                {
                    bfa.ToTable("battle_formation_ability");
                    bfa.Property("BattleFormationId").HasColumnName("battle_formation_id");
                    bfa.Property("AbilitiesAbilityId").HasColumnName("ability_id");
                    bfa.HasIndex("AbilitiesAbilityId").IsUnique();
                }
            );

        builder.Property(bf => bf.Version).IsRowVersion();
    }
}
