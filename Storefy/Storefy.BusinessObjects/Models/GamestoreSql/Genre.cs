using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace Storefy.BusinessObjects.Models.GameStoreSql;

/// <summary>
/// Represents the Genre model class.
/// </summary>
public class Genre
{
    /// <summary>
    /// Gets or sets the unique identification number of the Genre.
    /// </summary>
    [Key]
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identification number of the Genre.
    /// </summary>
    public string? CategoryId { get; set; }

    /// <summary>
    /// Gets or sets the name of the genre.
    /// </summary>
    [Required]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the identification number of parent Genre for the nested Genre.
    /// </summary>
    public string? ParentGenreId { get; set; }

    /// <summary>
    /// Gets or sets the relation between ParentGenre and NestedGenre.
    /// </summary>
    public virtual Genre? ParentGenre { get; set; }

    /// <summary>
    /// Gets or sets the description of Genre.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the picture of Genre.
    /// </summary>
    public string? Picture { get; set; }

    /// <summary>
    ///  Gets or sets the collection of Games that the Genres belongs to.
    /// </summary>
    [JsonIgnore]
    [BsonIgnore]
    public virtual ICollection<Game> Games { get; set; }

    public virtual ICollection<GenreTranslation> GenreTranslations { get; set; } = new List<GenreTranslation>();
}
