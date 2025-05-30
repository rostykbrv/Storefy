using MongoDB.Bson;

namespace Storefy.BusinessObjects.Models;

/// <summary>
/// Represents the ChangesLog model class.
/// </summary>
public class ChangesLog
{
    /// <summary>
    /// Gets or sets the date of the ChangeLog.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Gets or sets the action of the ChangesLog.
    /// </summary>
    public string Action { get; set; }

    /// <summary>
    /// Gets or sets the EntityType of the ChangesLog.
    /// </summary>
    public string EntityType { get; set; }

    /// <summary>
    /// Gets or sets the OldObject of the ChangesLog.
    /// </summary>
    public BsonDocument OldObject { get; set; }

    /// <summary>
    /// Gets or sets the NewObject of the ChangesLog.
    /// </summary>
    public BsonDocument NewObject { get; set; }
}
