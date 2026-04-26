using System.Diagnostics.CodeAnalysis;

namespace AosAdjutant.Api.Common;

[SuppressMessage(
    "SonarAnalyzer",
    "S6964",
    Justification = "Because of defaults the issue won't occur."
)]
public abstract record PagedQuery
{
    public int Page { get; set; } = 1;
    private int _pageSize = 20;
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = Math.Clamp(value, 1, 100);
    }
    public string? SortBy { get; set; }
    public string SortDirection { get; set; } = "asc";
}
