using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using hestia.Models.BOs;

namespace hestia.ViewModels
{
    /// <summary>
    /// The business logic for the cases and incidents details rendering goes here.
    /// </summary>
	public partial class CaseIncidentDetailsViewModel : BaseViewModel, IQueryAttributable
    {
        [ObservableProperty]
        public ListCaseIncident2 caseIncident;

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            CaseIncident = query["caseIncident"] as ListCaseIncident2;
        }
    }
}

