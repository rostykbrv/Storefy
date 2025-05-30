namespace Storefy.BusinessObjects.Models.GameStoreSql;

/// <summary>
/// Represents filter options for game's page.
/// </summary>
public class FilterOptions
{
    /// <summary>
    /// Gets or sets the genres for filtering.
    /// </summary>
    public string[]? Genres { get; set; }

    /// <summary>
    /// Gets or sets the platforms for filtering.
    /// </summary>
    public string[]? Platforms { get; set; }

    /// <summary>
    /// Gets or sets the publishers for filtering.
    /// </summary>
    public string[]? Publishers { get; set; }

    /// <summary>
    /// Gets or sets the maximum price for filtering.
    /// </summary>
    public decimal? MaxPrice { get; set; }

    /// <summary>
    /// Gets or sets the minimum price for filtering.
    /// </summary>
    public decimal? MinPrice { get; set; }

    /// <summary>
    /// Gets or sets the game name for filtering.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the publishing date for filtering.
    /// </summary>
    public string? DatePublishing { get; set; }

    /// <summary>
    /// Gets or sets the sorting option.
    /// </summary>
    public string? Sort { get; set; }

    /// <summary>
    /// Gets or sets the page number for pagination.
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// Gets or sets the page count for pagination.
    /// </summary>
    public int? PageCount { get; set; }

    /// <summary>
    /// Gets or sets the trigger for filtering or sorting.
    /// </summary>
    public string Trigger { get; set; }
}
