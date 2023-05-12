using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Visitz.Models.BOs;

namespace Visitz.ViewModels
{
    /// <summary>
    /// The business logic for the cases and incidents details rendering goes here.
    /// </summary>
	public partial class CaseIncidentDetailsViewModel : VisitzViewModel, IQueryAttributable
    {
        [ObservableProperty]
        public CaseloadItem caseIncident;

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            CaseIncident = query["caseIncident"] as CaseloadItem;
        }
    }
}

