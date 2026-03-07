using AosAdjutant.Api.Database;
using AosAdjutant.Api.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AosAdjutant.Api.Features.Abilities;

[Route("api/abilities")]
[ApiController]
public class AbilityController(ApplicationDbContext context) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<AbilityResponseDto>> CreateAbility([FromBody] CreateAbilityDto abilityData)
    {
        var newAbility = new Ability
        {
            Name = abilityData.Name,
            Reaction = abilityData.Reaction,
            Declaration = abilityData.Declaration,
            Effect = abilityData.Effect,
            Phase = abilityData.Phase,
            Restriction = abilityData.Restriction,
            Turn = abilityData.Turn
        };

        context.Abilities.Add(newAbility);
        await context.SaveChangesAsync();

        return Created(
            $"api/abilities/{newAbility.AbilityId}",
            new AbilityResponseDto(
                newAbility.AbilityId,
                newAbility.Name,
                newAbility.Reaction,
                newAbility.Declaration,
                newAbility.Effect,
                newAbility.Phase,
                newAbility.Restriction,
                newAbility.Turn,
                newAbility.Version
            )
        );
    }

    [HttpGet]
    public async Task<ActionResult<List<AbilityResponseDto>>> GetAbilities()
    {
        var abilities = await context.Abilities
            .AsNoTracking()
            .Select(a => new AbilityResponseDto(
                    a.AbilityId,
                    a.Name,
                    a.Reaction,
                    a.Declaration,
                    a.Effect,
                    a.Phase,
                    a.Restriction,
                    a.Turn,
                    a.Version
                )
            )
            .ToListAsync();

        return Ok(abilities);
    }

    [HttpGet("{abilityId}")]
    public async Task<ActionResult<AbilityResponseDto>> GetAbility([FromRoute] int abilityId)
    {
        var ability = await context.Abilities.AsNoTracking().FirstOrDefaultAsync(a => a.AbilityId == abilityId);

        return ability is null
            ? this.ApiProblem(new AppError(ErrorCode.NotFound, "Ability not found."))
            : Ok(
                new AbilityResponseDto(
                    ability.AbilityId,
                    ability.Name,
                    ability.Reaction,
                    ability.Declaration,
                    ability.Effect,
                    ability.Phase,
                    ability.Restriction,
                    ability.Turn,
                    ability.Version
                )
            );
    }

    [HttpPut("{abilityId}")]
    public async Task<ActionResult<AbilityResponseDto>> ChangeAbility(
        [FromRoute] int abilityId,
        [FromBody] ChangeAbilityDto abilityData
    )
    {
        var ability = await context.Abilities.FindAsync(abilityId);

        if (ability is null)
            return this.ApiProblem(new AppError(ErrorCode.NotFound, "Ability not found."));

        if (ability.Version != abilityData.Version)
            return this.ApiProblem(
                new AppError(ErrorCode.ConcurrencyError, "Ability was already modified in the background.")
            );

        ability.Name = abilityData.Name;
        ability.Reaction = abilityData.Reaction;
        ability.Declaration = abilityData.Declaration;
        ability.Effect = abilityData.Effect;
        ability.Phase = abilityData.Phase;
        ability.Restriction = abilityData.Restriction;
        ability.Turn = abilityData.Turn;

        await context.SaveChangesAsync();

        return Ok(
            new AbilityResponseDto(
                ability.AbilityId,
                ability.Name,
                ability.Reaction,
                ability.Declaration,
                ability.Effect,
                ability.Phase,
                ability.Restriction,
                ability.Turn,
                ability.Version
            )
        );
    }

    [HttpDelete("{abilityId}")]
    public async Task<ActionResult> DeleteAbility([FromRoute] int abilityId)
    {
        var ability = await context.Abilities.FindAsync(abilityId);

        if (ability is null)
            return this.ApiProblem(new AppError(ErrorCode.NotFound, "Ability not found."));

        context.Abilities.Remove(ability);
        await context.SaveChangesAsync();

        return NoContent();
    }
}
