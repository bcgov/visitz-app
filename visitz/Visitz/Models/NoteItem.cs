using Realms;
using System.Globalization;
using VisitzApi.Models;

namespace Visitz.Models
{
    /// <summary>
    /// The business object that would be used by the app source.
    /// </summary>
    public partial class NoteItem : IRealmObject
    {
        /// <summary>
        /// Used app-only to associate Notes with CaseloadItems. As of 2023-06-05 the ICM API does
        /// not return PK/FK information about notes.
        /// </summary>
        [Indexed]
        public string IcmId { get; set; }

        public string NotePeriod { get; set; }
        public string CreatedDate { get; set; }
        public string Content { get; set; }

        public static DateTime NotePeriodDateTimeTransform(NoteItem note)
        {
            return note.NotePeriod?.Length > 0 ? DateTime.Parse(note.NotePeriod) : DateTime.MinValue;
        }

        public static DateTime CreatedDateTimeTransform(NoteItem note)
        {
            return note.CreatedDate?.Length > 0 ? DateTime.Parse(note.CreatedDate) : DateTime.MinValue;
        }

        public string PeriodOrCreatedDate => NotePeriod?.Length > 0 ? NotePeriod : CreatedDate;

        public static NoteItem FromApiEntity(string icmId, NoteEntity note)
        {
            return new NoteItem()
            {
                IcmId = icmId,
                NotePeriod = note.NotePeriod,
                CreatedDate = note.CreatedDate, // TODO use actual DateTime type
                Content = note.Content,
            };
        }

        public static IEnumerable<NoteItem> FromApiEntities(string icmId, IEnumerable<NoteEntity> noteEntities)
        {
            return noteEntities.Select(note => FromApiEntity(icmId, note));
        }

        public static string NotePeriodFrom(DateTime dateTime)
        {
            return dateTime.ToString("MMM yyyy", CultureInfo.InvariantCulture);
        }

        public static string WrapContent(string idir, DateTime dateTime, string content)
        {
            return $"──── {idir} {dateTime} ────\n{content}";
        }
    }
}

