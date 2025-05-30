using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Storefy.BusinessObjects.Models.GameStoreSql;
public class GameTranslation
{
    [Key]
    public string Id { get; set; }

    public string Description { get; set; }

    public string CopyType { get; set; }

    public string ReleasedDate { get; set; }

    public string LanguageId { get; set; }

    [JsonIgnore]
    public virtual Language Language { get; set; }

    public string GameId { get; set; }

    [JsonIgnore]
    public virtual Game Game { get; set; }
}
