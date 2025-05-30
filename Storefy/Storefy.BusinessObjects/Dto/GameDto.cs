namespace Storefy.BusinessObjects.Dto;

/// <summary>
/// Represents the Data Transfer Object for Game.
/// This class is used for data transfer operations related to a Game.
/// </summary>
public class GameDto
{
    /// <summary>
    /// Gets or sets the unique identification number of the Game.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the key (game alias) for the game.
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// Gets or sets the Name for the game.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the Description of the Game.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the Price of the Game.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Gets or sets the number of Units in Stock for the Game.
    /// </summary>
    public int UnitInStock { get; set; }

    /// <summary>
    /// Gets or sets the Discontinued status of the Game.
    /// </summary>
    public int Discontinued { get; set; }

    /// <summary>
    /// Gets or sets the image url of the Game.
    /// </summary>
    public string ImageUrl { get; set; }

    /// <summary>
    /// Gets or sets the type of ordered game.
    /// </summary>
    public string CopyType { get; set; }

    /// <summary>
    /// Gets or sets the date of game's release.
    /// </summary>
    public string ReleasedDate { get; set; }

    /// <summary>
    /// Gets or sets the game size.
    /// </summary>
    public double GameSize { get; set; }
}
