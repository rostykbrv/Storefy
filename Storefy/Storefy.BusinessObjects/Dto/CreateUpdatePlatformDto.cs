namespace Storefy.BusinessObjects.Dto;

/// <summary>
/// Represents the Data Transfer Object for creating or updating a Platform.
/// Contains properties necessary for creating a new platform record or updating an existing one.
/// </summary>
public class CreateUpdatePlatformDto
{
    /// <summary>
    /// Gets or sets the PlatformDto object holding the details of the Platform.
    /// </summary>
    public PlatformDto Platform { get; set; }
}
