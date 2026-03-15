using AosAdjutant.Api.Database;
using AosAdjutant.Api.Features.AttackProfiles.WeaponEffects;
using AosAdjutant.Api.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AosAdjutant.Api.Features.AttackProfiles;

[Route("api/attack-profiles")]
[ApiController]
[Tags("Attack Profiles")]
public class AttackProfileController(ApplicationDbContext context) : ControllerBase
{
    [HttpGet("{attackProfileId}")]
    [EndpointSummary("Get an attack profile by ID")]
    [ProducesResponseType<AttackProfileResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AttackProfileResponseDto>> GetAttackProfile([FromRoute] int attackProfileId)
    {
        var attackProfile = await context.AttackProfiles
            .AsNoTracking()
            .Include(ap => ap.WeaponEffects)
            .FirstOrDefaultAsync(ap => ap.AttackProfileId == attackProfileId);

        return attackProfile is null
            ? this.ApiProblem(new AppError(ErrorCode.NotFound, "Attack profile not found."))
            : Ok(
                new AttackProfileResponseDto(
                    attackProfile.AttackProfileId,
                    attackProfile.Name,
                    attackProfile.IsRanged,
                    attackProfile.Range,
                    attackProfile.Attacks,
                    attackProfile.ToHit,
                    attackProfile.ToWound,
                    attackProfile.Rend,
                    attackProfile.Damage,
                    attackProfile.UnitId,
                    attackProfile.Version,
                    attackProfile.WeaponEffects.Select(wp => new WeaponEffectResponseDto(wp.Key, wp.Name)).ToList()
                )
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
        // Both Create and Update should perform validations but are part of different classes.
        // -> Reason to introduce service class which both controller classes call?
        var attackProfile = await context.AttackProfiles.FindAsync(attackProfileId);

        if (attackProfile is null)
            return this.ApiProblem(new AppError(ErrorCode.NotFound, "Attack profile not found."));

        if (attackProfile.Version != attackProfileData.Version)
            return this.ApiProblem(
                new AppError(ErrorCode.ConcurrencyError, "Attack profile was already modified in the background.")
            );

        var isDuplicate = await context.AttackProfiles.AnyAsync(ap =>
            ap.Name == attackProfileData.Name && ap.UnitId == attackProfile.UnitId &&
            ap.AttackProfileId != attackProfileId
        );
        if (isDuplicate)
            return this.ApiProblem(new AppError(ErrorCode.UniqueKeyError, "Attack profile already exists."));

        attackProfile.Name = attackProfileData.Name;
        attackProfile.IsRanged = attackProfileData.IsRanged;
        attackProfile.Range = attackProfileData.Range;
        attackProfile.Attacks = attackProfileData.Attacks;
        attackProfile.ToHit = attackProfileData.ToHit;
        attackProfile.ToWound = attackProfileData.ToWound;
        attackProfile.Rend = attackProfileData.Rend;
        attackProfile.Damage = attackProfileData.Damage;

        await context.SaveChangesAsync();

        return Ok(
            new AttackProfileResponseDto(
                attackProfile.AttackProfileId,
                attackProfile.Name,
                attackProfile.IsRanged,
                attackProfile.Range,
                attackProfile.Attacks,
                attackProfile.ToHit,
                attackProfile.ToWound,
                attackProfile.Rend,
                attackProfile.Damage,
                attackProfile.UnitId,
                attackProfile.Version,
                new List<WeaponEffectResponseDto>()
            )
        );
    }

    [HttpDelete("{attackProfileId}")]
    [EndpointSummary("Delete an attack profile")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteAttackProfile([FromRoute] int attackProfileId)
    {
        var attackProfile = await context.AttackProfiles.FindAsync(attackProfileId);

        if (attackProfile is null)
            return this.ApiProblem(new AppError(ErrorCode.NotFound, "Attack profile not found."));

        context.AttackProfiles.Remove(attackProfile);
        await context.SaveChangesAsync();

        return NoContent();
    }
}
