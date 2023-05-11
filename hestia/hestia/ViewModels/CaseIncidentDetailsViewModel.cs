using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using visitz.Models.BOs;

namespace visitz.ViewModels
{
    /// <summary>
    /// The business logic for the cases and incidents details rendering goes here.
    /// </summary>
	public partial class CaseIncidentDetailsViewModel : BaseViewModel, IQueryAttributable
    {
        [ObservableProperty]
        public CaseloadItem caseIncident;

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            CaseIncident = query["caseIncident"] as CaseloadItem;
        }
    }
}

