using AosAdjutant.Api.Features.Abilities;
using AosAdjutant.Api.Features.BattleFormations;
using AosAdjutant.Api.Features.Units;

namespace AosAdjutant.Api.Features.Factions;

public class Faction
{
    public int FactionId { get; set; }
    public required string Name { get; set; }
    public uint Version { get; set; }

    public ICollection<BattleFormation> BattleFormations { get; } = new List<BattleFormation>();
    public ICollection<Unit> Units { get; } = new List<Unit>();
    public ICollection<Ability> Abilities { get; } = new List<Ability>();
}
