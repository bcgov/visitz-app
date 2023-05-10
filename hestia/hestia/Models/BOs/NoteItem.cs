using hestiapi.Models;
using System;

namespace hestia.Models.BOs
{
    /// <summary>
    /// The business object that would be used by the app source.
    /// </summary>
    public class NoteItem
    {
        public string NotePeriod { get; set; }
        public string CreatedDate { get; set; }
        public string Notes { get; set; }

        public NoteItem(NoteEntity note)
        {
            NotePeriod = note.NotePeriod;
            CreatedDate = note.CreatedDate.ToLongDateString(); // TODO use actual DateTime type
            Notes = note.Content;
        }
    }
}

