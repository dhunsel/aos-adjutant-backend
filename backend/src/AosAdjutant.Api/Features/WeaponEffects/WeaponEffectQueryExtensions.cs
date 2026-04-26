using System.Linq.Expressions;
using AosAdjutant.Api.Common;

namespace AosAdjutant.Api.Features.WeaponEffects;

public static class WeaponEffectQueryExtensions
{
    // Keep the sorting argument stored as Expression so that the OrderBy overload of IQueryable is used
    // If just returning a delegate, the IEnumerable OrderBy would be used which would sort the results in C#, not in DB
    private static readonly Dictionary<string, Expression<Func<WeaponEffect, object>>> SortColumns =
        new(StringComparer.OrdinalIgnoreCase) { ["name"] = f => f.Name };

    public static IQueryable<WeaponEffect> ApplyFilters(
        this IQueryable<WeaponEffect> query,
        WeaponEffectQuery filter
    )
    {
        return query;
    }

    public static IQueryable<WeaponEffect> ApplySorting(
        this IQueryable<WeaponEffect> query,
        PagedQuery filter
    )
    {
        // Row order without explicit order by is undefined, therefore always fall back on id sorting
        if (filter.SortBy is null || !SortColumns.TryGetValue(filter.SortBy, out var sortExpr))
            return query.OrderBy(we => we.Key);

        return filter.SortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase)
            ? query.OrderByDescending(sortExpr)
            : query.OrderBy(sortExpr);
    }
}
