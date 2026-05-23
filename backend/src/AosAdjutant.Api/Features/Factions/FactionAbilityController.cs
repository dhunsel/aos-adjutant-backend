using AosAdjutant.Api.Common;
using AosAdjutant.Api.Features.Abilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AosAdjutant.Api.Features.Factions;

[Route("api/factions/{factionId:int}/abilities")]
[ApiController]
[Tags("Factions")]
public sealed class FactionAbilityController(FactionService factionService) : ControllerBase
{
    [HttpPost]
    [Authorize(Policy = "RequireAdmin")]
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
            a =>
                CreatedAtAction(
                    nameof(AbilityController.GetAbility),
                    "Ability",
                    new { abilityId = a.AbilityId },
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
    [Authorize]
    [EndpointSummary("Get all abilities for a faction")]
    [ProducesResponseType<PaginatedResponse<AbilityResponseDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaginatedResponse<AbilityResponseDto>>> GetAbilities(
        [FromRoute] int factionId,
        [FromQuery] AbilityQuery abilityQuery
    )
    {
        var abilitiesResult = await factionService.GetFactionAbilities(factionId, abilityQuery);
        return abilitiesResult.Match(
            abilities =>
                Ok(
                    abilities.Map(a => new AbilityResponseDto(
                        a.AbilityId,
                        a.Name,
                        a.Reaction,
                        a.Declaration,
                        a.Effect,
                        a.Phase,
                        a.Restriction,
                        a.Turn,
                        a.Version
                    ))
                ),
            this.ApiProblem
        );
    }
}
