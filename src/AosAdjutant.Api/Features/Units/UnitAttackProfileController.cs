using AosAdjutant.Api.Features.AttackProfiles;
using AosAdjutant.Api.Features.WeaponEffects;
using AosAdjutant.Api.Shared;
using Microsoft.AspNetCore.Mvc;

namespace AosAdjutant.Api.Features.Units;

[Route("api/units/{unitId}/attack-profiles")]
[ApiController]
[Tags("Attack Profiles")]
public class UnitAttackProfileController(AttackProfileService attackProfileService) : ControllerBase
{
    [HttpPost]
    [EndpointSummary("Create an attack profile under a unit")]
    [ProducesResponseType<AttackProfileResponseDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AttackProfileResponseDto>> CreateAttackProfile(
        [FromRoute] int unitId,
        [FromBody] CreateAttackProfileDto attackProfileData
    )
    {
        var attackProfileResult = await attackProfileService.CreateAttackProfile(unitId, attackProfileData);
        return attackProfileResult.Match(
            ap => Created(
                $"api/attack-profiles/{ap.AttackProfileId}",
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

    [HttpGet]
    [EndpointSummary("Get all attack profiles for a unit")]
    [ProducesResponseType<List<AttackProfileResponseDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<AttackProfileResponseDto>>> GetAttackProfiles([FromRoute] int unitId)
    {
        var attackProfilesResult = await attackProfileService.GetUnitAttackProfiles(unitId);
        return attackProfilesResult.Match(
            attackProfiles => Ok(
                attackProfiles.Select(ap => new AttackProfileResponseDto(
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
                )
            ),
            this.ApiProblem
        );
    }
}
