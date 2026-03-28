using AosAdjutant.Api.Features.Abilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AosAdjutant.Api.Database.Configuration;

public sealed class AbilityEntityTypeConfiguration : IEntityTypeConfiguration<Ability>
{
    public void Configure(EntityTypeBuilder<Ability> builder)
    {
        builder.ToTable("ability");
        builder.Property(f => f.AbilityId).HasColumnName("ability_id");
        builder.Property(f => f.Name).HasColumnName("name").HasMaxLength(250);
        builder.Property(f => f.Reaction).HasColumnName("reaction").HasMaxLength(250);
        builder.Property(f => f.Declaration).HasColumnName("declaration").HasMaxLength(250);
        builder.Property(f => f.Effect).HasColumnName("effect").HasMaxLength(250);
        builder.Property(f => f.Phase).HasColumnName("phase").HasConversion<string>().HasMaxLength(250);
        builder.Property(f => f.Restriction).HasColumnName("restriction").HasConversion<string>().HasMaxLength(250);
        builder.Property(f => f.Turn).HasColumnName("turn").HasConversion<string>().HasMaxLength(250);
        builder.Property(f => f.IsGeneric).HasColumnName("is_generic");

        builder.HasKey(f => f.AbilityId);

        builder.Property(a => a.Version).IsRowVersion();
    }
}
