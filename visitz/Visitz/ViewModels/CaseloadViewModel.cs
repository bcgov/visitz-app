using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using VisitzApi;
using Visitz.Models.BOs;
using VisitzApi.ErrorHandling;
using System.ComponentModel;
using Visitz.Services.Networking;
using Visitz.Views;
using Visitz.Services;

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

        public bool IsRefreshing { get; set; }

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
            
        }

        private async void CaseloadViewModel_PropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName.Equals(nameof(SelectedCaseIncident)))
                if (SelectedCaseIncident is not null)
                    await NavigateToNotesPage(SelectedCaseIncident);
        }

        private static async Task NavigateToNotesPage(CaseloadItem caseIncident)
        {
            await NavigateTo(typeof(NotesPage), new Dictionary<string, object> 
            { 
                { "caseIncident", caseIncident } 
            });
        }

        public async Task TryFetchCasesAndIncidents()
        {
            if (await VisitzSession.GetValidSessionAsync())
                await FetchCasesAndIncidents();
        }

        public async Task FetchCasesAndIncidents()
        {
            try
            {
                var info = await VisitzSessionInfo.GetAsync();
                var caseloadContent = await Vpi.GetCaseloadAsync(info.Idir);

                Caseload.Clear();

                foreach (var item in caseloadContent)
                    Caseload.Add(new CaseloadItem(item));
            }
            catch (VisitzApiException ex)
            {
                // TODO: Implement proper error logging/handling (show to user? store errors somewhere?)
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                // TODO: Implement proper error logging/handling (show to user? store errors somewhere?)
                Console.WriteLine(ex.StackTrace);
            }
        }

        public async Task RefreshCaseload()
        {
            if (!IsRefreshing)
            {
                IsRefreshing = true;

                await TryFetchCasesAndIncidents();
                
                IsRefreshing = false;
            }
        }

        [RelayCommand]
        void GoToNotes(CaseloadItem caseloadItem)
        {
            SelectedCaseIncident = caseloadItem;
        }

        public static async Task OpenDebugOptionsPage()
        {
            if (DebugOptions.Enabled)
                await NavigateTo(typeof(DebugOptionsPage));
        }
    }
}

