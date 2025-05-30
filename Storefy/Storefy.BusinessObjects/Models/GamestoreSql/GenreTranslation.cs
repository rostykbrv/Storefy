using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Storefy.BusinessObjects.Models.GameStoreSql;
public class GenreTranslation
{
    [Key]
    public string Id { get; set; }

    public string Name { get; set; }

    public string LanguageId { get; set; }

    [JsonIgnore]
    public virtual Language Language { get; set; }

    public string GenreId { get; set; }

    [JsonIgnore]
    public virtual Genre Genre { get; set; }
}
