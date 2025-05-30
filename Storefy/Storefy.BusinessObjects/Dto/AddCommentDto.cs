using Storefy.BusinessObjects.Models.GameStoreSql;

namespace Storefy.BusinessObjects.Dto;

/// <summary>
/// Represents a data transfer object for adding a commentary.
/// </summary>
public class AddCommentDto
{
    /// <summary>
    /// Gets or sets the action, which expressed
    /// what should be done with comment.
    /// </summary>
    public string Action { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of Parent comment.
    /// </summary>
    public string? ParentId { get; set; }

    /// <summary>
    /// Gets or sets the comment to be created.
    /// </summary>
    public Comment Comment { get; set; }
}
