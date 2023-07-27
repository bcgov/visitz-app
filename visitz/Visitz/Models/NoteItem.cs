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
        private static readonly string IcmNotePeriodDateFormat = "MMM yyyy";
        /// <summary>
        /// Used app-only to associate Notes with CaseloadItems. As of 2023-06-05 the ICM API does
        /// not return PK/FK information about notes.
        /// </summary>
        [Indexed]
        public string IcmId { get; set; }

        public string NotePeriod { get; set; }
        public string CreatedDate { get; set; }
        public string Content { get; set; }
        public int PageNumber { get; set; }

        public static DateTime NotePeriodDateTimeTransform(NoteItem note, bool ascending)
        {
            var defaultValue = ascending ? DateTime.MinValue : DateTime.MaxValue;
            return note.NotePeriod?.Length > 0 ? DateTime.Parse(note.NotePeriod) : defaultValue;
        }

        public static DateTime CreatedDateTimeTransform(NoteItem note, bool ascending)
        {
            var defaultValue = ascending ? DateTime.MinValue : DateTime.MaxValue;
            return note.CreatedDate?.Length > 0 ? DateTime.Parse(note.CreatedDate) : defaultValue;
        }

        public string PeriodOrPageNumber => NotePeriod?.Length > 0 ? NotePeriod : $"Page {PageNumber}";
        public bool ShowTitleIcon => NotePeriod?.Length > 0;

        public static NoteItem FromApiEntity(string icmId, NoteEntity note, int pageNumber)
        {
            return new NoteItem()
            {
                IcmId = icmId,
                NotePeriod = note.NotePeriod,
                CreatedDate = note.CreatedDate, // TODO use actual DateTime type
                Content = note.Content,
                PageNumber = pageNumber
            };
        }

        public static IEnumerable<NoteItem> FromApiEntities(string icmId, IEnumerable<NoteEntity> noteEntities)
        {
            return noteEntities
                .OrderBy(item => NoteEntity.NotePeriodDateTimeTransform(item, true))
                .ThenBy(item => NoteEntity.CreatedDateTimeTransform(item, true))
                .Select((note, index) => FromApiEntity(icmId, note, index + 1));
        }

        public static string NotePeriodFrom(DateTime dateTime)
        {
            return dateTime.ToString(IcmNotePeriodDateFormat, CultureInfo.InvariantCulture);
        }

        public static bool IsCurrentNotePeriod(NoteItem note)
        {
            return NotePeriodFrom(DateTime.Now).ToLower() == note?.NotePeriod?.ToLower();
        }

        public static string WrapContent(string idir, DateTime dateTime, string content)
        {
            return $"──── {idir} {dateTime} ────\n{content}";
        }
    }
}

