using System.Linq.Expressions;
using AosAdjutant.Api.Common;

namespace AosAdjutant.Api.Features.AttackProfiles;

public static class AttackProfileQueryExtensions
{
    // Keep the sorting argument stored as Expression so that the OrderBy overload of IQueryable is used
    // If just returning a delegate, the IEnumerable OrderBy would be used which would sort the results in C#, not in DB
    private static readonly Dictionary<
        string,
        Expression<Func<AttackProfile, object>>
    > SortColumns = new(StringComparer.OrdinalIgnoreCase)
    {
        ["name"] = ap => ap.Name,
        ["isRanged"] = ap => ap.IsRanged,
    };

    public static IQueryable<AttackProfile> ApplyFilters(
        this IQueryable<AttackProfile> query,
        AttackProfileQuery filter
    )
    {
        if (filter.IsRanged is not null)
            query = query.Where(ap => ap.IsRanged == filter.IsRanged);

        return query;
    }

    public static IQueryable<AttackProfile> ApplySorting(
        this IQueryable<AttackProfile> query,
        PagedQuery filter
    )
    {
        // Row order without explicit order by is undefined, therefore always fall back on id sorting
        if (filter.SortBy is null || !SortColumns.TryGetValue(filter.SortBy, out var sortExpr))
            return query.OrderBy(ap => ap.AttackProfileId);

        return filter.SortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase)
            ? query.OrderByDescending(sortExpr)
            : query.OrderBy(sortExpr);
    }
}
