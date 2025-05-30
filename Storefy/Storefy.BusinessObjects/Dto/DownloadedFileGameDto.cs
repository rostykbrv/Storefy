namespace Storefy.BusinessObjects.Dto;

/// <summary>
/// Represents a data transfer object for a downloaded game file.
/// </summary>
public class DownloadedFileGameDto
{
    /// <summary>
    /// Gets or sets the Name for the game.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the key (game alias) for the game.
    /// </summary>
    public string Alias { get; set; }

    /// <summary>
    /// Gets or sets the Description of the Game.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the Price of the Game.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Gets or sets the list of Game's genres identificators.
    /// </summary>
    public IEnumerable<string> Genres { get; set; }

    /// <summary>
    /// Gets or sets the list of Game's platforms identificators.
    /// </summary>
    public IEnumerable<string> Platforms { get; set; }

    /// <summary>
    /// Gets or sets the Game's publisher identificator.
    /// </summary>
    public string? Publisher { get; set; }
}
