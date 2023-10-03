using Realms;

namespace Visitz.Models
{
    public partial class NoteDraft : IRealmObject
    {
        [PrimaryKey]
        public string CaseIncidentAndCreatedDateID { get; set; }

        public string Draft { get; set; }

        public static string MakeId(string caseIncidentNumber)
        {
            // For now (2023-10-03), we will only hold one draft per caseIncidentNumber.
            return $"{caseIncidentNumber}";
        }

        public static (IQueryable<NoteDraft> noteDraftQuery, IDisposable queryToken) Subscribe(
            Realm realm,
            string caseIncidentAndCreatedDateID,
            NotificationCallbackDelegate<NoteDraft> callbackDelegate)
        {
            var noteDraftQuery = realm.All<NoteDraft>()
                .Where(draft => draft.CaseIncidentAndCreatedDateID == caseIncidentAndCreatedDateID);

            return (noteDraftQuery, noteDraftQuery.SubscribeForNotifications(callbackDelegate));
        }
    }
}

