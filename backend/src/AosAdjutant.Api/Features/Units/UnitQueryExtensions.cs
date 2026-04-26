using System.Linq.Expressions;
using AosAdjutant.Api.Common;

namespace AosAdjutant.Api.Features.Units;

public static class UnitQueryExtensions
{
    // Keep the sorting argument stored as Expression so that the OrderBy overload of IQueryable is used
    // If just returning a delegate, the IEnumerable OrderBy would be used which would sort the results in C#, not in DB
    private static readonly Dictionary<string, Expression<Func<Unit, object>>> SortColumns = new(
        StringComparer.OrdinalIgnoreCase
    )
    {
        ["name"] = f => f.Name,
    };

    public static IQueryable<Unit> ApplyFilters(this IQueryable<Unit> query, UnitQuery filter)
    {
        return query;
    }

    public static IQueryable<Unit> ApplySorting(this IQueryable<Unit> query, PagedQuery filter)
    {
        // Row order without explicit order by is undefined, therefore always fall back on id sorting
        if (filter.SortBy is null || !SortColumns.TryGetValue(filter.SortBy, out var sortExpr))
            return query.OrderBy(u => u.UnitId);

        return filter.SortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase)
            ? query.OrderByDescending(sortExpr)
            : query.OrderBy(sortExpr);
    }
}
