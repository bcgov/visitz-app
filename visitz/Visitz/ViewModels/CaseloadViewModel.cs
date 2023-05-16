using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using VisitzApi;
using Visitz.Models.BOs;
using VisitzApi.ErrorHandling;
using System.ComponentModel;
using Visitz.Services.Networking;
using Visitz.Views;

namespace Visitz.ViewModels
{
    /// <summary>
    /// The business logic for the cases and incidents list rendering goes here.
    /// </summary>
    public partial class CaseloadViewModel : VisitzViewModel
    {
        public ObservableCollection<CaseloadItem> Caseload { get; set; } = new();

        [ObservableProperty]
        public CaseloadItem selectedCaseIncident;

        private Vpi Vpi { get; }

        public CaseloadViewModel(Vpi visitzApi)
        {
            Vpi = visitzApi;
        }

        public override void PageCreated()
        {
            PropertyChanged += CaseloadViewModel_PropertyChanged;
        }

        public override async void PageStarted()
        {
            // TODO: Do a proper Token check here: Is access_token expired, is refresh_token expired?
            if (TokenHolder.AccessToken != null)
                await FetchCasesAndIncidents();
        }

        private async void CaseloadViewModel_PropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName.Equals(nameof(SelectedCaseIncident)))
                if (SelectedCaseIncident is not null)
                    await NavigateToNotesPage(SelectedCaseIncident);
        }

        private async Task NavigateToNotesPage(CaseloadItem caseIncident)
        {
            await NavigateTo(typeof(NotesPage), new Dictionary<string, object> 
            { 
                { "caseIncident", caseIncident } 
            });
        }

        public async Task FetchCasesAndIncidents()
        {
            try
            {
                // TODO: Worker ID should be collected from current JWT Access Token field "idir_username"
                var caseloadContent = await Vpi.GetCaseloadAsync("CGWRK68");

                Caseload.Clear();

                foreach (var item in caseloadContent)
                    Caseload.Add(new CaseloadItem(item));
            }
            catch (VisitzApiException ex)
            {
                // TODO: Make actual error UI/UX to show this error
                Console.WriteLine(ex.Message);
            }
        }

        [RelayCommand]
        void GoToNotes(CaseloadItem caseloadItem)
        {
            SelectedCaseIncident = caseloadItem;
        }
    }
}

