using Realms;
using VisitzModel.Extensions;
using VisitzModel.Models.Drafts;

namespace VisitzModel.Models
{
    public partial class NoteDraft : IRealmObject, IDraftItem
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

		// TODO: Returning LocalNow is only for testing during development
		public DateTime LastUpdated { get => DateTimeExtensions.LocalNow; set { } }

		public string Preview { get => Draft; set { } }

		// TODO: Returning CaseIncidentAndCreatedDateID is only for testing during development
		public string Name { get => CaseIncidentAndCreatedDateID; set { } }

		public static string MakeId(string caseIncidentNumber)
        {
            // For now (2023-10-03), we will only hold one draft per caseIncidentNumber.
            return $"{caseIncidentNumber}";
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
