using Realms;
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
    }
}

