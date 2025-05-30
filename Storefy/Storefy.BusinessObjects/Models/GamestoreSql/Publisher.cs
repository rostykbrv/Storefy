using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace Storefy.BusinessObjects.Models.GameStoreSql;

/// <summary>
/// Represents the Publisher model class.
/// </summary>
public class Publisher
{
    /// <summary>
    /// Gets or sets the unique identification number of the Publisher.
    /// </summary>
    [Key]
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identification number of the Publisher.
    /// </summary>
    public string? SupplierId { get; set; }

    /// <summary>
    /// Gets or sets the company name of the Publisher.
    /// </summary>
    public string CompanyName { get; set; }

    /// <summary>
    /// Gets or sets the contact name of the Publisher.
    /// </summary>
    public string? ContactName { get; set; }

    /// <summary>
    /// Gets or sets the contact title of the Publisher.
    /// </summary>
    public string? ContactTitle { get; set; }

    /// <summary>
    /// Gets or sets the Publisher's address.
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Gets or sets the Publisher's city.
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// Gets or sets the Publisher's region.
    /// </summary>
    public string? Region { get; set; }

    /// <summary>
    /// Gets or sets the Publisher's country.
    /// </summary>
    public string? Country { get; set; }

    /// <summary>
    /// Gets or sets the Publisher's phone.
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Gets or sets the Publisher's fax.
    /// </summary>
    public string? Fax { get; set; }

    /// <summary>
    /// Gets or sets the optional description of the Publisher.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the home page URL of the Publisher.
    /// </summary>
    public string? HomePage { get; set; }

    /// <summary>
    /// Gets or sets the collection of Games published by the Publisher.
    /// </summary>
    [JsonIgnore]
    [BsonIgnore]
    public virtual ICollection<Game> Games { get; set; }
}
