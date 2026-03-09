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

        return newAbilityResult.IsSuccess
            ? Created(
                $"api/abilities/{newAbilityResult.GetValue.AbilityId}",
                new AbilityResponseDto(
                    newAbilityResult.GetValue.AbilityId,
                    newAbilityResult.GetValue.Name,
                    newAbilityResult.GetValue.Reaction,
                    newAbilityResult.GetValue.Declaration,
                    newAbilityResult.GetValue.Effect,
                    newAbilityResult.GetValue.Phase,
                    newAbilityResult.GetValue.Restriction,
                    newAbilityResult.GetValue.Turn,
                    newAbilityResult.GetValue.Version
                )
            )
            : this.ApiProblem(newAbilityResult.GetError);
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
        return abilityResult.IsSuccess ? Ok(abilityResult.GetValue) : this.ApiProblem(abilityResult.GetError);
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
        return abilityResult.IsSuccess ? Ok(abilityResult.GetValue) : this.ApiProblem(abilityResult.GetError);
    }

    [HttpDelete("{abilityId}")]
    [EndpointSummary("Delete an ability")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteAbility([FromRoute] int abilityId)
    {
        var deleteResult = await abilityService.DeleteAbility(abilityId);
        return deleteResult.IsSuccess ? NoContent() : this.ApiProblem(deleteResult.GetError);
    }
}
