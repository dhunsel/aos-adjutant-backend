using AosAdjutant.Api.Features.Abilities;
using AosAdjutant.Api.Shared;
using Microsoft.AspNetCore.Mvc;

namespace AosAdjutant.Api.Features.Factions;

[Route("api/factions/{factionId}/abilities")]
[ApiController]
[Tags("Factions")]
public class FactionAbilityController(FactionService factionService) : ControllerBase
{
    [HttpPost]
    [EndpointSummary("Create an ability for a faction")]
    [ProducesResponseType<AbilityResponseDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AbilityResponseDto>> CreateAbility(
        [FromRoute] int factionId,
        [FromBody] CreateAbilityDto abilityData
    )
    {
        var abilityResult = await factionService.CreateFactionAbility(factionId, abilityData);
        return abilityResult.Match(
            a => Created(
                $"api/abilities/{a.AbilityId}",
                new AbilityResponseDto(
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
            ),
            this.ApiProblem
        );
    }

    [HttpGet]
    [EndpointSummary("Get all abilities for a faction")]
    [ProducesResponseType<List<AbilityResponseDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<AbilityResponseDto>>> GetAbilities([FromRoute] int factionId)
    {
        var abilitiesResult = await factionService.GetFactionAbilities(factionId);
        return abilitiesResult.Match(
            abilities => Ok(
                abilities.Select(a => new AbilityResponseDto(
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
            ),
            this.ApiProblem
        );
    }
}
