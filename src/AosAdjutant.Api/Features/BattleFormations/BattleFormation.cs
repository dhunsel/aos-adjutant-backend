using AosAdjutant.Api.Features.Abilities;

namespace AosAdjutant.Api.Features.BattleFormations;

public class BattleFormation
{
    public int BattleFormationId { get; set; }
    public required string Name { get; set; }
    public int FactionId { get; set; }
    public uint Version { get; set; }

    public ICollection<Ability> Abilities { get; } = new List<Ability>();
}
