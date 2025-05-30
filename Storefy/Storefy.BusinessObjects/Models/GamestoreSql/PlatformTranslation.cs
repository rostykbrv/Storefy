using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Storefy.BusinessObjects.Models.GameStoreSql;
public class PlatformTranslation
{
    [Key]
    public string Id { get; set; }

    public string Type { get; set; }

    public string LanguageId { get; set; }

    [JsonIgnore]
    public virtual Language Language { get; set; }

    public string PlatformId { get; set; }

    [JsonIgnore]
    public virtual Platform Platform { get; set; }
}
