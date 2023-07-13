using System.Text.Json.Serialization;

namespace VisitzApi.Models
{
    public class SubmitNoteEntity
    {
        [JsonPropertyName("entityNumber")]
        public string EntityNumber { get; set; }

        [JsonPropertyName("entityType")]
        public string EntityType { get; set; }

        [JsonPropertyName("notePeriod")]
        public string NotePeriod { get; set; }

        [JsonPropertyName("notes")]
        public string Content { get; set; }

        [JsonPropertyName("createdBy")]
        public string CreatedBy { get; set; }
    }
}
