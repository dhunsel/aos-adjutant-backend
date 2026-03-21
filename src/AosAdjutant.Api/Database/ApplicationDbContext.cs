using AosAdjutant.Api.Database.Configuration;
using AosAdjutant.Api.Features.Abilities;
using AosAdjutant.Api.Features.AttackProfiles;
using AosAdjutant.Api.Features.WeaponEffects;
using AosAdjutant.Api.Features.Factions;
using AosAdjutant.Api.Features.BattleFormations;
using AosAdjutant.Api.Features.Units;
using Microsoft.EntityFrameworkCore;

namespace AosAdjutant.Api.Database;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Faction> Factions { get; set; }
    public DbSet<BattleFormation> BattleFormations { get; set; }
    public DbSet<Ability> Abilities { get; set; }
    public DbSet<Unit> Units { get; set; }
    public DbSet<AttackProfile> AttackProfiles { get; set; }
    public DbSet<WeaponEffect> WeaponEffects { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        new FactionEntityTypeConfiguration().Configure(modelBuilder.Entity<Faction>());
        new BattleFormationEntityTypeConfiguration().Configure(modelBuilder.Entity<BattleFormation>());
        new AbilityEntityTypeConfiguration().Configure(modelBuilder.Entity<Ability>());
        new UnitEntityTypeConfiguration().Configure(modelBuilder.Entity<Unit>());
        new AttackProfileEntityTypeConfiguration().Configure(modelBuilder.Entity<AttackProfile>());
        new WeaponEffectEntityTypeConfiguration().Configure(modelBuilder.Entity<WeaponEffect>());

        base.OnModelCreating(modelBuilder);
    }
}
