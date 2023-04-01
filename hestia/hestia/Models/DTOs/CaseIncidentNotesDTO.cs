using System;
namespace hestia.Models.DTOs
{
    public class Note
    {
        public string notePeriod { get; set; }
        public string createdDate { get; set; }
        public string notes { get; set; }
    }

    public class ResponseGetNotes
    {
        public PayLoad payLoad { get; set; }

        public class PayLoad
        {
            public string entityNumber { get; set; }
            public string entityType { get; set; }
            public List<Note> notes { get; set; }
        }
    }

    /// <summary>
    /// The data transfer object that would be used by the networking module to deserialize CaseIncidentNotes API response.
    /// </summary>
    public class CaseIncidentNotesDTO
    {
        public ResponseGetNotes responseGetNotes { get; set; }
    }
}

