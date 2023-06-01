using VisitzApi.Models;

namespace Visitz.Models
{
    /// <summary>
    /// The business object that would be used by the app source.
    /// </summary>
    public class NoteItem
    {
        public string NotePeriod { get; set; }
        public string CreatedDate { get; set; }
        public string Content { get; set; }

        public NoteItem(NoteEntity note)
        {
            NotePeriod = note.NotePeriod;
            CreatedDate = note.CreatedDate; // TODO use actual DateTime type
            Content = note.Content;
        }
    }
}

