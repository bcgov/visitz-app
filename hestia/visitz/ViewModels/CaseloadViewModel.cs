using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using visitzApi;
using visitz.Models.BOs;

namespace visitz.ViewModels
{
    /// <summary>
    /// The business logic for the cases and incidents list rendering goes here.
    /// </summary>
    public partial class CaseloadViewModel : BaseViewModel
    {
        public ObservableCollection<CaseloadItem> Caseload { get; set; } = new();

        [ObservableProperty]
        public CaseloadItem selectedCaseIncident;

        private readonly Vpi visitzApi;

        public CaseloadViewModel(Vpi visitzApi)
        {
            this.visitzApi = visitzApi;
        }

        public async void FetchCasesAndIncidents()
        {
            try
            {
                // TODO: Worker ID should be collected from current JWT Access Token field "idir_username"
                var caseloadContent = await visitzApi.GetCaseloadAsync("CGWRK68");

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

