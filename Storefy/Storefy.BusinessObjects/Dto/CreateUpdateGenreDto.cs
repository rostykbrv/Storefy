namespace Storefy.BusinessObjects.Dto;

/// <summary>
/// Represents the Data Transfer Object for creating or updating a Genre.
/// Contains properties necessary for creating a new genre record or updating an existing one.
/// </summary>
public class CreateUpdateGenreDto
{
    /// <summary>
    /// Gets or sets the GenreDto object holding the details of the Genre.
    /// </summary>
    public GenreDto Genre { get; set; }
}
