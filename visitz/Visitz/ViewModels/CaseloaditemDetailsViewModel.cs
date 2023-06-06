using CommunityToolkit.Mvvm.ComponentModel;
using Visitz.Models;
using Visitz.Storage;

namespace Visitz.ViewModels
{
    /// <summary>
    /// The business logic for the cases and incidents details rendering goes here.
    /// </summary>
	public partial class CaseloadItemDetailsViewModel : VisitzViewModel, IQueryAttributable
    {
        public static readonly string CaseIncidentIdKey = "caseIncidentId";

        private string caseIncidentId;

        [ObservableProperty]
        public CaseloadItem caseIncident;

        [ObservableProperty]
        public IList<FamilyMember> familyMembers;

        [ObservableProperty]
        public string idSubtitle;

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            caseIncidentId = query[CaseIncidentIdKey] as string;
        }

        public override async void PageCreated()
        {
            using var realm = await VisitzRealm.GetAsync();

            CaseIncident = realm.Find<CaseloadItem>(caseIncidentId);
            
            // NOTE: A VerticalStackLayout won't initiate a data load on its own
            // so we need to run the Realm READ operation manually here.
            FamilyMembers = CaseIncident.FamilyMembers.ToList();

            IdSubtitle =
                CaseIncident.EntityType
                + " "
                + CaseIncident.CaseIncidentNumber;
        }
    }
}

