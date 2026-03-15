using AosAdjutant.Api.Database;
using AosAdjutant.Api.Features.AttackProfiles;
using AosAdjutant.Api.Features.AttackProfiles.WeaponEffects;
using AosAdjutant.Api.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AosAdjutant.Api.Features.Units;

[Route("api/units")]
[ApiController]
[Tags("Units")]
public class UnitController(ApplicationDbContext context) : ControllerBase
{
    [HttpGet("{unitId}")]
    [EndpointSummary("Get a unit by ID")]
    [ProducesResponseType<UnitResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UnitResponse>> GetUnit([FromRoute] int unitId)
    {
        var unit = await context.Units.AsNoTracking().FirstOrDefaultAsync(u => u.UnitId == unitId);

        return unit is null
            ? this.ApiProblem(new AppError(ErrorCode.NotFound, "Unit not found."))
            : Ok(
                new UnitResponse(
                    unit.UnitId,
                    unit.Name,
                    unit.Health,
                    unit.Move,
                    unit.Save,
                    unit.Control,
                    unit.WardSave,
                    unit.FactionId,
                    unit.Version
                )
            );
    }

    [HttpPut("{unitId}")]
    [EndpointSummary("Update a unit")]
    [ProducesResponseType<UnitResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UnitResponse>> UpdateUnit([FromRoute] int unitId, [FromBody] ChangeUnitDto unitData)
    {
        var unit = await context.Units.FindAsync(unitId);

        if (unit is null)
            return this.ApiProblem(new AppError(ErrorCode.NotFound, "Unit not found."));

        if (unit.Version != unitData.Version)
            return this.ApiProblem(
                new AppError(ErrorCode.ConcurrencyError, "Unit was already modified in the background.")
            );

        var isDuplicate = await context.Units.AnyAsync(u =>
            u.Name == unitData.Name && u.FactionId == unit.FactionId && u.UnitId != unitId
        );
        if (isDuplicate)
            return this.ApiProblem(new AppError(ErrorCode.UniqueKeyError, "Unit already exists."));

        unit.Name = unitData.Name;
        unit.Health = unitData.Health;
        unit.Move = unitData.Move;
        unit.Save = unitData.Save;
        unit.Control = unitData.Control;
        unit.WardSave = unitData.WardSave;

        await context.SaveChangesAsync();

        return Ok(
            new UnitResponse(
                unit.UnitId,
                unit.Name,
                unit.Health,
                unit.Move,
                unit.Save,
                unit.Control,
                unit.WardSave,
                unit.FactionId,
                unit.Version
            )
        );
    }

    [HttpDelete("{unitId}")]
    [EndpointSummary("Delete a unit")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteUnit([FromRoute] int unitId)
    {
        var unit = await context.Units.FindAsync(unitId);

        if (unit is null)
            return this.ApiProblem(new AppError(ErrorCode.NotFound, "Unit not found."));

        context.Units.Remove(unit);
        await context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("{unitId}/attack-profiles")]
    [EndpointSummary("Create an attack profile under a unit")]
    [ProducesResponseType<AttackProfileResponseDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AttackProfileResponseDto>> CreateAttackProfile(
        [FromRoute] int unitId,
        [FromBody] CreateAttackProfileDto attackProfileData
    )
    {
        var unitExists = await context.Units.AnyAsync(u => u.UnitId == unitId);
        if (!unitExists)
            return this.ApiProblem(new AppError(ErrorCode.NotFound, "Unit not found."));

        var isDuplicate =
            await context.AttackProfiles.AnyAsync(ap => ap.Name == attackProfileData.Name && ap.UnitId == unitId);
        if (isDuplicate)
            return this.ApiProblem(new AppError(ErrorCode.UniqueKeyError, "Attack profile already exists."));

        var newAttackProfile = new AttackProfile
        {
            Name = attackProfileData.Name,
            IsRanged = attackProfileData.IsRanged,
            Range = attackProfileData.Range,
            Attacks = attackProfileData.Attacks,
            ToHit = attackProfileData.ToHit,
            ToWound = attackProfileData.ToWound,
            Rend = attackProfileData.Rend,
            Damage = attackProfileData.Damage,
            UnitId = unitId,
            WeaponEffects = context.WeaponEffects
                .Where(we => attackProfileData.WeaponEffects.Contains(we.Key))
                .ToList()
        };


        // Because of race conditions this might still fail on UK/FK error
        // Ignore for now (won't occur in practice) but revisit in the future
        context.AttackProfiles.Add(newAttackProfile);
        await context.SaveChangesAsync();

        return Created(
            $"api/attack-profiles/{newAttackProfile.AttackProfileId}",
            new AttackProfileResponseDto(
                newAttackProfile.AttackProfileId,
                newAttackProfile.Name,
                newAttackProfile.IsRanged,
                newAttackProfile.Range,
                newAttackProfile.Attacks,
                newAttackProfile.ToHit,
                newAttackProfile.ToWound,
                newAttackProfile.Rend,
                newAttackProfile.Damage,
                newAttackProfile.UnitId,
                newAttackProfile.Version,
                newAttackProfile.WeaponEffects.Select(wp => new WeaponEffectResponseDto(wp.Key, wp.Name)).ToList()
            )
        );
    }

    [HttpGet("{unitId}/attack-profiles")]
    [EndpointSummary("Get all attack profiles for a unit")]
    [ProducesResponseType<List<AttackProfileResponseDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<AttackProfileResponseDto>>> GetAttackProfiles([FromRoute] int unitId)
    {
        var unitExists = await context.Units.AnyAsync(u => u.UnitId == unitId);
        if (!unitExists)
            return this.ApiProblem(new AppError(ErrorCode.NotFound, "Unit not found."));

        var attackProfiles = await context.AttackProfiles
            .AsNoTracking()
            .Where(ap => ap.UnitId == unitId)
            .Select(ap => new AttackProfileResponseDto(
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
            .ToListAsync();
        return Ok(attackProfiles);
    }
}
