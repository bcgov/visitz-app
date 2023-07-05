using CommunityToolkit.Mvvm.ComponentModel;
using Visitz.Models;
using Visitz.Resources.Localization;
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

        [ObservableProperty]
        public string entityTypeDescriptor;

        public override async void PageCreated()
        {
            caseIncidentId = Parameters[CaseIncidentIdKey] as string;

            using var realm = await IcmDataRealm.GetAsync();

            CaseloadItem = realm.Find<CaseloadItem>(caseIncidentId);

            // NOTE: A VerticalStackLayout won't initiate a data load on its own
            // so we need to run the Realm READ operation manually here.
            FamilyMembers = CaseloadItem.FamilyMembers
                .OrderByDescending(fm => fm.IsKeyPlayer)
                .ToList();

            IdSubtitle =
                CaseloadItem.EntityType
                + " "
                + CaseloadItem.CaseIncidentNumber;

            EntityTypeDescriptor = CaseloadItem.EntityType == IcmEntity.Case
                ? LocalizedStrings.CaseType 
                : LocalizedStrings.Type;
        }
    }
}

