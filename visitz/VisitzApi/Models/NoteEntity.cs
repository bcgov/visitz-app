using System.Text.Json.Serialization;

namespace VisitzApi.Models
{
    public class NoteEntity
    {
        private const string NotesKey = "notes";

        public string NotePeriod { get; set; } // Intentionally a string, not DateTime

        public string CreatedDate { get; set; }

        [JsonPropertyName(NotesKey)]
        public string Content { get; set; }

        public static DateTime NotePeriodDateTimeTransform(NoteEntity note)
        {
            return note.NotePeriod?.Length > 0 ? DateTime.Parse(note.NotePeriod) : DateTime.MinValue;
        }

        public static DateTime CreatedDateTimeTransform(NoteEntity note)
        {
            return note.CreatedDate?.Length > 0 ? DateTime.Parse(note.CreatedDate) : DateTime.MinValue;
        }
    }
}
