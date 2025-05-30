namespace Storefy.BusinessObjects.Dto;

/// <summary>
/// Represents the Data Transfer Object for creating or updating a Publisher.
/// Contains properties necessary for creating a new publisher record or updating an existing one.
/// </summary>
public class CreateUpdatePublisherDto
{
    /// <summary>
    /// Gets or sets the PublisherDto object holding the details of the Publisher.
    /// </summary>
    public PublisherDto Publisher { get; set; }
}
