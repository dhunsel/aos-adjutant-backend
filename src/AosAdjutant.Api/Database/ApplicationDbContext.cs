using AosAdjutant.Api.Database.Configuration;
using AosAdjutant.Api.Features.Abilities;
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        new FactionEntityTypeConfiguration().Configure(modelBuilder.Entity<Faction>());
        new BattleFormationEntityTypeConfiguration().Configure(modelBuilder.Entity<BattleFormation>());
        new AbilityEntityTypeConfiguration().Configure(modelBuilder.Entity<Ability>());
        new UnitEntityTypeConfiguration().Configure(modelBuilder.Entity<Unit>());

        base.OnModelCreating(modelBuilder);
    }
}
