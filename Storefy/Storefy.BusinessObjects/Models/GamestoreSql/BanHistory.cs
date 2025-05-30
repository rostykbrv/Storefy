using System.ComponentModel.DataAnnotations;

namespace Storefy.BusinessObjects.Models.GameStoreSql;

/// <summary>
/// Represents the Ban history model class.
/// </summary>
public class BanHistory
{
    /// <summary>
    /// Gets or sets the unique identification number of the Ban.
    /// </summary>
    [Key]
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the banned user's identification number.
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// Gets or sets the banned user's entity.
    /// </summary>
    public User User { get; set; }

    /// <summary>
    /// Gets or sets the date where ban was created.
    /// </summary>
    public DateTime BanStarted { get; set; }

    /// <summary>
    /// Gets or sets the data where ban ends.
    /// </summary>
    public DateTime BanEnds { get; set; }
}
