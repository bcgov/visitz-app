using System.Text.Json.Serialization;

namespace hestiapi.Models
{
    public class NoteEntity
    {
        private const string NotesKey = "notes";

        public string NotePeriod { get; set; } // Intentionally a string, not DateTime

        public DateTime CreatedDate { get; set; }

        [JsonPropertyName(NotesKey)]
        public string Content { get; set; }
    }
}
