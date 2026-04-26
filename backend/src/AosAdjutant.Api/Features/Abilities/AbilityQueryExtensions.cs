using System.Linq.Expressions;
using AosAdjutant.Api.Common;

namespace AosAdjutant.Api.Features.Abilities;

public static class AbilityQueryExtensions
{
    // Keep the sorting argument stored as Expression so that the OrderBy overload of IQueryable is used
    // If just returning a delegate, the IEnumerable OrderBy would be used which would sort the results in C#, not in DB
    private static readonly Dictionary<string, Expression<Func<Ability, object>>> SortColumns = new(
        StringComparer.OrdinalIgnoreCase
    )
    {
        ["name"] = a => a.Name,
        ["phase"] = a => a.Phase,
    };

    public static IQueryable<Ability> ApplyFilters(
        this IQueryable<Ability> query,
        AbilityQuery filter
    )
    {
        if (filter.Phase is not null)
            query = query.Where(a => a.Phase == filter.Phase);

        return query;
    }

    public static IQueryable<Ability> ApplySorting(
        this IQueryable<Ability> query,
        PagedQuery filter
    )
    {
        // Row order without explicit order by is undefined, therefore always fall back on id sorting
        if (filter.SortBy is null || !SortColumns.TryGetValue(filter.SortBy, out var sortExpr))
            return query.OrderBy(a => a.AbilityId);

        return filter.SortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase)
            ? query.OrderByDescending(sortExpr)
            : query.OrderBy(sortExpr);
    }
}
