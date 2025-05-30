using System.Text.Json.Serialization;

namespace Storefy.BusinessObjects.Models.GameStoreSql;

/// <summary>
/// Represents the Commentary model class.
/// </summary>
public class Comment
{
    /// <summary>
    /// Gets or sets the unique identification number of the Game.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the user.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the body text of the comment.
    /// </summary>
    public string Body { get; set; }

    /// <summary>
    /// Gets or sets the date of comment added.
    /// </summary>
    public DateTime CommentAdded { get; set; } = DateTime.Now;

    /// <summary>
    /// Gets or sets the unique identification of the Comment of the Game.
    /// </summary>
    public string? CommentId { get; set; }

    /// <summary>
    /// Gets or sets the collection of child comments (replies and quotes).
    /// </summary>
    public ICollection<Comment> ChildComments { get; set; } = new List<Comment>();

    /// <summary>
    /// Gets or sets the Game of the Comment.
    /// </summary>
    [JsonIgnore]
    public virtual Game? Game { get; set; }
}
