namespace Storefy.BusinessObjects.Models.GameStoreSql;

/// <summary>
/// Represents paginated results for game listings.
/// </summary>
public class GamePageResult
{
    /// <summary>
    /// Gets or sets the array of games for the current page.
    /// </summary>
    public Game[] Games { get; set; }

    /// <summary>
    /// Gets or sets the total number of pages available.
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// Gets or sets the number of the current page.
    /// </summary>
    public int CurrentPage { get; set; }
}
