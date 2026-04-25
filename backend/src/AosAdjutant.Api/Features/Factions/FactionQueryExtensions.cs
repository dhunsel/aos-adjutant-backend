namespace AosAdjutant.Api.Features.Factions;

public static class FactionQueryExtensions
{
    public static IQueryable<Faction> ApplyFilters(
        this IQueryable<Faction> query,
        FactionQueryFilter filter
    )
    {
        if (filter.GrandAlliance is not null)
            query = query.Where(f => f.GrandAlliance == filter.GrandAlliance);

        return query;
    }
}
