using CommunityToolkit.Mvvm.ComponentModel;
using Visitz.Models;

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
        public string idSubtitle;

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            CaseIncident = query["caseIncident"] as CaseloadItem;

            IdSubtitle =
                CaseIncident?.EntityType
                + " "
                + CaseIncident?.CaseIncidentNumber;
        }
    }
}

