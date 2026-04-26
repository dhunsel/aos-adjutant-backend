using System.Linq.Expressions;
using AosAdjutant.Api.Common;

namespace AosAdjutant.Api.Features.BattleFormations;

public static class BattleFormationQueryExtensions
{
    // Keep the sorting argument stored as Expression so that the OrderBy overload of IQueryable is used
    // If just returning a delegate, the IEnumerable OrderBy would be used which would sort the results in C#, not in DB
    private static readonly Dictionary<
        string,
        Expression<Func<BattleFormation, object>>
    > SortColumns = new(StringComparer.OrdinalIgnoreCase) { ["name"] = bf => bf.Name };

    public static IQueryable<BattleFormation> ApplyFilters(
        this IQueryable<BattleFormation> query,
        BattleFormationQuery filter
    )
    {
        return query;
    }

    public static IQueryable<BattleFormation> ApplySorting(
        this IQueryable<BattleFormation> query,
        PagedQuery filter
    )
    {
        // Row order without explicit order by is undefined, therefore always fall back on id sorting
        if (filter.SortBy is null || !SortColumns.TryGetValue(filter.SortBy, out var sortExpr))
            return query.OrderBy(bf => bf.BattleFormationId);

        return filter.SortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase)
            ? query.OrderByDescending(sortExpr)
            : query.OrderBy(sortExpr);
    }
}
