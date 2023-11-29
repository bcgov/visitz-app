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
        private static readonly string NoteWrapperTimestampFormat = "yyyy-MMM-dd hh:mm:ss tt";
        private static readonly string Separator = "────";

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
        
        public DateTimeOffset NotePeriodDateTime { get; set; }
        public DateTimeOffset CreatedDateTime { get; set; }

        public static bool EqualByDates(NoteItem lhs, NoteItem rhs)
        {
            return lhs != null && rhs != null
                && lhs.NotePeriod?.Trim().ToLower() == rhs.NotePeriod?.Trim().ToLower()
                && lhs.CreatedDate?.Trim().ToLower() == rhs.CreatedDate?.Trim().ToLower();
        }

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
                CreatedDate = note.CreatedDate,
                Content = note.Content,
                PageNumber = pageNumber,
                NotePeriodDateTime = note.NotePeriod?.Length > 0 
                    ? DateTimeOffset.Parse(note.NotePeriod) 
                    : DateTimeOffset.MinValue,
                CreatedDateTime = note.CreatedDate?.Length > 0 
                    ? DateTimeOffset.Parse(note.CreatedDate)
                    : DateTimeOffset.MinValue,
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
            var timestamp = dateTime.ToString(NoteWrapperTimestampFormat, CultureInfo.InvariantCulture);
            return $"{Separator} {idir} {timestamp} {Separator}\n{content}";
        }

        public static string ToStringLite(NoteItem note)
        {
            return $"null: {note == null}, " +
                $"NotePeriod: {note?.NotePeriod}, " +
                $"CreatedDate: {note?.CreatedDate}, " +
                $"IsValid: {note?.IsValid}";
        }

        public static NoteItem GetLatestByEntityId(Realm realm, string entityId)
        {
            return GetNotesByEntityId(realm, entityId)
                .LastOrDefault();
        }

        public static IQueryable<NoteItem> GetNotesByEntityId(Realm realm, string entityId)
        {
            return realm
                .All<NoteItem>()
                .Where(item => item.IcmId == entityId)
                .Filter($"TRUEPREDICATE SORT({nameof(NotePeriodDateTime)} ASC, {nameof(CreatedDateTime)} ASC)");
        }
    }
}

