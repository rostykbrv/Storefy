namespace Storefy.BusinessObjects.Dto;

/// <summary>
/// Represents the Data Transfer Object for a Publisher.
/// </summary>
public class PublisherDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the Publisher.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the company name of the Publisher.
    /// </summary>
    public string CompanyName { get; set; }

    /// <summary>
    /// Gets or sets the Description of the Publisher.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the HomePage URL of the Publisher company.
    /// </summary>
    public string? HomePage { get; set; }
}
