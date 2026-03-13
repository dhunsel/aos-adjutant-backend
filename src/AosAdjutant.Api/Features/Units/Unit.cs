using AosAdjutant.Api.Features.Abilities;

namespace AosAdjutant.Api.Features.Units;

public class Unit
{
    public int UnitId { get; set; }
    public required string Name { get; set; }
    public int Health { get; set; }
    public required string Move { get; set; }
    public int Save { get; set; }
    public int Control { get; set; }
    public int? WardSave { get; set; }
    public int FactionId { get; set; }
    public uint Version { get; set; }

    public ICollection<Ability> Abilities { get; } = new List<Ability>();
}
