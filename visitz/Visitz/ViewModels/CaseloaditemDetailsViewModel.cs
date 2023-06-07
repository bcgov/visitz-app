using CommunityToolkit.Mvvm.ComponentModel;
using Visitz.Models;
using Visitz.Storage;

namespace Visitz.ViewModels
{
    /// <summary>
    /// The business logic for the cases and incidents details rendering goes here.
    /// </summary>
	public partial class CaseloadItemDetailsViewModel : VisitzViewModel
    {
        public static readonly string CaseIncidentIdKey = "caseIncidentId";

        private string caseIncidentId;

        [ObservableProperty]
        public CaseloadItem caseloadItem;

        [ObservableProperty]
        public IList<FamilyMember> familyMembers;

        [ObservableProperty]
        public string idSubtitle;

        public override async void PageCreated()
        {
            caseIncidentId = Parameters[CaseIncidentIdKey] as string;

            using var realm = await VisitzRealm.GetAsync();

            CaseloadItem = realm.Find<CaseloadItem>(caseIncidentId);
            
            // NOTE: A VerticalStackLayout won't initiate a data load on its own
            // so we need to run the Realm READ operation manually here.
            FamilyMembers = CaseloadItem.FamilyMembers.ToList();

            IdSubtitle =
                CaseloadItem.EntityType
                + " "
                + CaseloadItem.CaseIncidentNumber;
        }
    }
}

