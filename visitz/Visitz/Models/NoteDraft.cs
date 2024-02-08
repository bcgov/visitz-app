using Realms;
using VisitzModel.Extensions;

namespace Visitz.Models
{
    public partial class NoteDraft : IRealmObject
    {
        [PrimaryKey]
        public string CaseIncidentAndCreatedDateID { get; set; }

        public string Draft { get; set; }

        public string DraftBinding
        {
            get => IsValid ? Draft : default;
            set
            {
                bool canSet = !value?.ContainsUnicodeSurrogatesAndOtherSymbols() ?? true;
                
                if (canSet)
                    this.Commit(() => Draft = value);

                RaisePropertyChanged(nameof(DraftBinding));
            }
        }

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

        public static NoteDraft FindByEntityId(Realm realm, string entityId)
        {
            return realm.Find<NoteDraft>(MakeId(entityId));
        }

        public static async Task Delete(Realm realm, string entityNumber)
        {
            var draft = FindByEntityId(realm, entityNumber);

            if (draft != null)
                await realm.WriteAsync(() => realm.Remove(draft));
        }
    }
}

