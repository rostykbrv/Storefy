namespace Storefy.BusinessObjects.Dto;

/// <summary>
/// Represents the Data Transfer Object for a Genre.
/// </summary>
public class GenreDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the Genre.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the Name of the Genre.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the Parent Genre Id.
    /// This property establish relationship among main genres and subgenres.
    /// </summary>
    public string? ParentGenreId { get; set; }
}
