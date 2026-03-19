using AosAdjutant.Api.Shared;
using Microsoft.AspNetCore.Mvc;

namespace AosAdjutant.Api.Features.Abilities;

[Route("api/abilities")]
[ApiController]
[Tags("Abilities")]
public class AbilityController(AbilityService abilityService) : ControllerBase
{
    [HttpPost]
    [EndpointSummary("Create a generic ability")]
    [ProducesResponseType<AbilityResponseDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AbilityResponseDto>> CreateAbility([FromBody] CreateAbilityDto abilityData)
    {
        var newAbilityResult = await abilityService.CreateGenericAbility(abilityData);
        return newAbilityResult.Match(
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
    [EndpointSummary("Get all generic abilities")]
    [ProducesResponseType<List<AbilityResponseDto>>(StatusCodes.Status200OK)]
    public Task<ActionResult<List<AbilityResponseDto>>> GetAbilities()
    {
        throw new NotImplementedException();
    }

    [HttpGet("{abilityId}")]
    [EndpointSummary("Get an ability by ID")]
    [ProducesResponseType<AbilityResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AbilityResponseDto>> GetAbility([FromRoute] int abilityId)
    {
        var abilityResult = await abilityService.GetAbility(abilityId);
        return abilityResult.Match(
            a => Ok(
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

    [HttpPut("{abilityId}")]
    [EndpointSummary("Update an ability")]
    [ProducesResponseType<AbilityResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AbilityResponseDto>> ChangeAbility(
        [FromRoute] int abilityId,
        [FromBody] ChangeAbilityDto abilityData
    )
    {
        var abilityResult = await abilityService.UpdateAbility(abilityId, abilityData);
        return abilityResult.Match(
            a => Ok(
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

    [HttpDelete("{abilityId}")]
    [EndpointSummary("Delete an ability")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteAbility([FromRoute] int abilityId)
    {
        var deleteResult = await abilityService.DeleteAbility(abilityId);
        return deleteResult.Match(NoContent, this.ApiProblem);
    }
}
