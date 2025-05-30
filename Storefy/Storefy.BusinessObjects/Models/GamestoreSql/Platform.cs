using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace Storefy.BusinessObjects.Models.GameStoreSql;

/// <summary>
/// Represents the Platform model class.
/// </summary>
public class Platform
{
    /// <summary>
    /// Gets or sets the unique identification number of the Platform.
    /// </summary>
    [Key]
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the type of the platform.
    /// </summary>
    [Required]
    public string Type { get; set; }

    /// <summary>
    /// Gets or sets the relation between Platform and Game.
    /// </summary>
    [JsonIgnore]
    [BsonIgnore]
    public virtual ICollection<Game> Games { get; set; }

    public virtual ICollection<PlatformTranslation> PlatformTranslations { get; set; } = new List<PlatformTranslation>();
}
