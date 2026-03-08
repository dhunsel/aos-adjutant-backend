using AosAdjutant.Api.Shared;
using Microsoft.AspNetCore.Mvc;

namespace AosAdjutant.Api.Features.Abilities;

[Route("api/abilities")]
[ApiController]
public class AbilityController(AbilityService abilityService) : ControllerBase
{
    [HttpPost]
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
    public Task<ActionResult<List<AbilityResponseDto>>> GetAbilities()
    {
        throw new NotImplementedException();
    }

    [HttpGet("{abilityId}")]
    public async Task<ActionResult<AbilityResponseDto>> GetAbility([FromRoute] int abilityId)
    {
        var abilityResult = await abilityService.GetAbility(abilityId);
        return abilityResult.IsSuccess ? Ok(abilityResult.GetValue) : this.ApiProblem(abilityResult.GetError);
    }

    [HttpPut("{abilityId}")]
    public async Task<ActionResult<AbilityResponseDto>> ChangeAbility(
        [FromRoute] int abilityId,
        [FromBody] ChangeAbilityDto abilityData
    )
    {
        var abilityResult = await abilityService.UpdateAbility(abilityId, abilityData);
        return abilityResult.IsSuccess ? Ok(abilityResult.GetValue) : this.ApiProblem(abilityResult.GetError);
    }

    [HttpDelete("{abilityId}")]
    public async Task<ActionResult> DeleteAbility([FromRoute] int abilityId)
    {
        var deleteResult = await abilityService.DeleteAbility(abilityId);
        return deleteResult.IsSuccess ? NoContent() : this.ApiProblem(deleteResult.GetError);
    }
}
