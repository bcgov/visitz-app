using CommunityToolkit.Mvvm.ComponentModel;
using Visitz.Models.BOs;

namespace Visitz.ViewModels
{
    /// <summary>
    /// The business logic for the cases and incidents details rendering goes here.
    /// </summary>
	public partial class CaseloadItemDetailsViewModel : VisitzViewModel, IQueryAttributable
    {
        [ObservableProperty]
        public CaseloadItem caseIncident;

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            CaseIncident = query["caseIncident"] as CaseloadItem;
        }
    }
}

