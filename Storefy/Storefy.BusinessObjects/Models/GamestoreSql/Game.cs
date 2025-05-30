using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Storefy.BusinessObjects.Models.GameStoreSql;

/// <summary>
/// Represents the Game model class.
/// </summary>
public class Game
{
    /// <summary>
    /// Gets or sets the unique identification number of the Game.
    /// </summary>
    [Key]
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identification number of the Game.
    /// </summary>
    public string? GameId { get; set; }

    /// <summary>
    /// Gets or sets the key (game alias) for the game.
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// Gets or sets the name of the game.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the unique identification of the Publisher of the Game.
    /// </summary>
    public string? PublisherId { get; set; }

    /// <summary>
    /// Gets or sets the Publisher of the Game.
    /// </summary>
    [JsonIgnore]
    public virtual Publisher? Publisher { get; set; }

    /// <summary>
    /// Gets or sets the quantity per unit of the Game.
    /// </summary>
    public string? QuantityPerUnit { get; set; }

    /// <summary>
    /// Gets or sets the Price of the Game.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Gets or sets the number of Units in Stock for the Game.
    /// </summary>
    public int UnitInStock { get; set; }

    /// <summary>
    /// Gets or sets the number of Units on order for the Game.
    /// </summary>
    public int UnitsOnOrder { get; set; }

    /// <summary>
    /// Gets or sets the number of reordered level for the Game.
    /// </summary>
    public int ReorderLevel { get; set; }

    /// <summary>
    /// Gets or sets the Discontinued status of the Game.
    /// </summary>
    public int Discontinued { get; set; }

    /// <summary>
    /// Gets or sets the image url of the Game.
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Gets or sets the type of ordered game.
    /// </summary>
    public string? CopyType { get; set; }

    /// <summary>
    /// Gets or sets the date of game's release.
    /// </summary>
    public string? ReleasedDate { get; set; }

    /// <summary>
    /// Gets or sets the game size.
    /// </summary>
    public double GameSize { get; set; }

    /// <summary>
    /// Gets or sets the description for the game.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the Discount value of the Game.
    /// </summary>
    public double Discount { get; set; }

    /// <summary>
    /// Gets or sets the collection of Platforms on which the Game is available.
    /// </summary>
    public virtual ICollection<Platform> Platforms { get; set; } = new List<Platform>();

    /// <summary>
    /// Gets or sets the collection of Genres that the Game belongs to.
    /// </summary>
    public virtual ICollection<Genre> Genres { get; set; } = new List<Genre>();

    /// <summary>
    /// Gets or sets the collection of OrderDetails related to this Game instance.
    /// </summary>
    [JsonIgnore]
    public virtual ICollection<OrderDetails> OrderDetails { get; set; } = new List<OrderDetails>();

    /// <summary>
    /// Gets or sets the collection of Comments related to this Game instance.
    /// </summary>
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    /// <summary>
    /// Gets or sets the Date when game was added.
    /// </summary>
    public DateTime DateAdded { get; set; }

    /// <summary>
    /// Gets or sets the amount of game views.
    /// </summary>
    public int ViewCount { get; set; }

    public virtual ICollection<GameTranslation> GameTranslations { get; set; } = new List<GameTranslation>();
}
