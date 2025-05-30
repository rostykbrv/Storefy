namespace Storefy.BusinessObjects.Dto;

/// <summary>
/// Represents the Data Transfer Object for creating or updating a Game.
/// Contains properties necessary for creating a new game record or updating an existing one.
/// </summary>
public class CreateUpdateGameDto
{
    /// <summary>
    /// Gets or sets the GameDto object holding the details of the Game.
    /// </summary>
    public GameDto Game { get; set; }

    /// <summary>
    /// Gets or sets the list of Game's Genres identificators.
    /// </summary>
    public ICollection<string>? Genres { get; set; }

    /// <summary>
    /// Gets or sets the list of Game's Platforms identificators.
    /// </summary>
    public ICollection<string>? Platforms { get; set; }

    /// <summary>
    /// Gets or sets the Game's Image.
    /// </summary>
    public string Image { get; set; }

    /// <summary>
    /// Gets or sets the Publisher identificator of the Game.
    /// </summary>
    public string? Publisher { get; set; }

    /// <summary>
    /// Gets or sets the Publisher name of the Game.
    /// </summary>
    public string? PublisherName { get; set; }

    /// <summary>
    /// Gets or sets the list of Game's Genre names.
    /// </summary>
    public List<string> GenreNames { get; set; } = new List<string>();
}
