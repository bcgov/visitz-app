using Realms;
using System.Globalization;
using VisitzApi.Models;

namespace VisitzModel.Models
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
        /// Used app-only to uniquely ID NoteItems.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The ICM API doesn't provide a NoteItem's actual primary key ID, so this value is made using <c>IcmId</c>,
        /// <c>NotePeriod</c>, and <c>CreatedDate</c>.
        /// </para>
        /// <para>
        /// <i>This may become an issue</i>—if two note objects are created in the same second for a given ICM entity, this PK
        /// unique assertion will fail. While possible, this is also unlikely: new note objects are typically only 
        /// created either monthly or when a ~16000 character limit is reached.
        /// </para>
        /// </remarks>
        [PrimaryKey]
        public string FullID { get; set; }

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

        public string PeriodOrPageNumber => NotePeriod?.Length > 0 ? NotePeriod : $"Page {PageNumber}";

        public static string MakeFullID(string icmId, NoteEntity note)
        {
            return MakeFullID(icmId, note.NotePeriod, note.CreatedDate);
        }

        public static string MakeFullID(string icmId, string notePeriod, string createdDate)
        {
            return $"{icmId}-{notePeriod}-{createdDate}";
        }

        public static NoteItem FromApiEntity(string icmId, NoteEntity note, int pageNumber)
        {
            return new NoteItem()
            {
                FullID = MakeFullID(icmId, note),
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

		public static async Task UpsertNotesAsync(Realm realm, string entityId, IEnumerable<NoteItem> newNotes)
		{
			var currentNotes = GetNotesByEntityId(realm, entityId);
			var deletedNotes = currentNotes.ExceptBy(newNotes.Select(NoteSelector), NoteSelector);

			await realm.WriteAsync(() =>
			{
				foreach (var deletedNote in deletedNotes)
					realm.Remove(deletedNote);

				realm.Add(newNotes, update: true);
			});
		}

		static string NoteSelector(NoteItem note) => note.FullID;

		public static string NotePeriodFrom(DateTime dateTime)
        {
            return NotePeriodFrom(new DateTimeOffset(dateTime));
        }

        public static string NotePeriodFrom(DateTimeOffset dateTime)
        {
            return dateTime.ToString(IcmNotePeriodDateFormat, CultureInfo.InvariantCulture);
        }

        public static string WrapContent(string idir, DateTime dateTime, string content)
        {
            var timestamp = dateTime.ToString(NoteWrapperTimestampFormat, CultureInfo.InvariantCulture);
            return $"{Separator} {idir} {timestamp} {Separator}\n{content}";
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

