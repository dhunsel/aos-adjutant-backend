using AosAdjutant.Api.Features.Abilities;

namespace AosAdjutant.Api.Features.Factions.BattleFormations;

public class BattleFormation
{
    public int BattleFormationId { get; set; }
    public required string Name { get; set; }
    public int FactionId { get; set; }
    public int AbilityId { get; set; }
    public uint Version { get; set; }

    public Faction? Faction { get; set; }
    public Ability? Ability { get; set; }
}
