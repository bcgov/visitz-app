using System;

namespace hestia.Models.BOs
{
    /// <summary>
    /// The business object that would be used by the app source.
    /// </summary>
    public class CaseNoteBO
    {
        public string NotePeriod { get; set; }
        public string CreatedDate { get; set; }
        public string Notes { get; set; }

        public static CaseNoteBO ToBO(DTOs.Note dto)
        {
            return new CaseNoteBO()
            {
                NotePeriod = dto.notePeriod,
                CreatedDate = dto.createdDate,
                Notes = dto.notes
            };
        }
    }
}

