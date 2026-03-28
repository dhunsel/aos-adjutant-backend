using AosAdjutant.Api.Common;
using AosAdjutant.Api.Features.WeaponEffects;
using Microsoft.AspNetCore.Mvc;

namespace AosAdjutant.Api.Features.AttackProfiles;

[Route("api/attack-profiles")]
[ApiController]
[Tags("Attack Profiles")]
public sealed class AttackProfileController(AttackProfileService attackProfileService) : ControllerBase
{
    [HttpGet("{attackProfileId}")]
    [EndpointSummary("Get an attack profile by ID")]
    [ProducesResponseType<AttackProfileResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AttackProfileResponseDto>> GetAttackProfile([FromRoute] int attackProfileId)
    {
        var attackProfileResult = await attackProfileService.GetAttackProfile(attackProfileId);
        return attackProfileResult.Match(
            ap => Ok(
                new AttackProfileResponseDto(
                    ap.AttackProfileId,
                    ap.Name,
                    ap.IsRanged,
                    ap.Range,
                    ap.Attacks,
                    ap.ToHit,
                    ap.ToWound,
                    ap.Rend,
                    ap.Damage,
                    ap.UnitId,
                    ap.Version,
                    ap.WeaponEffects.Select(wp => new WeaponEffectResponseDto(wp.Key, wp.Name)).ToList()
                )
            ),
            this.ApiProblem
        );
    }

    [HttpPut("{attackProfileId}")]
    [EndpointSummary("Update an attack profile")]
    [ProducesResponseType<AttackProfileResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AttackProfileResponseDto>> UpdateAttackProfile(
        [FromRoute] int attackProfileId,
        [FromBody] ChangeAttackProfileDto attackProfileData
    )
    {
        var attackProfileResult = await attackProfileService.UpdateAttackProfile(attackProfileId, attackProfileData);
        return attackProfileResult.Match(
            ap => Ok(
                new AttackProfileResponseDto(
                    ap.AttackProfileId,
                    ap.Name,
                    ap.IsRanged,
                    ap.Range,
                    ap.Attacks,
                    ap.ToHit,
                    ap.ToWound,
                    ap.Rend,
                    ap.Damage,
                    ap.UnitId,
                    ap.Version,
                    ap.WeaponEffects.Select(wp => new WeaponEffectResponseDto(wp.Key, wp.Name)).ToList()
                )
            ),
            this.ApiProblem
        );
    }

    [HttpDelete("{attackProfileId}")]
    [EndpointSummary("Delete an attack profile")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteAttackProfile([FromRoute] int attackProfileId)
    {
        var deleteResult = await attackProfileService.DeleteAttackProfile(attackProfileId);
        return deleteResult.Match(NoContent, this.ApiProblem);
    }
}
