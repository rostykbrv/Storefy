using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Storefy.BusinessObjects.Models.GameStoreSql;
public class Language
{
    [Key]
    public string Id { get; set; }

    public string LanguageCode { get; set; }

    public string LanguageName { get; set; }

    [JsonIgnore]
    public virtual ICollection<GameTranslation> GameTranslations { get; set; }

    [JsonIgnore]
    public virtual ICollection<GenreTranslation> GenreTranslations { get; set; }

    [JsonIgnore]
    public virtual ICollection<PlatformTranslation> PlatformTranslations { get; set; }
}
