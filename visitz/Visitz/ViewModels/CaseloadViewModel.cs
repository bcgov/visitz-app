using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using Visitz.Models;
using Visitz.Pages;
using Visitz.Storage;
using Visitz.Authentication.Keycloak;
using CommunityToolkit.Mvvm.Messaging;
using Visitz.Services;

namespace Visitz.ViewModels
{
    /// <summary>
    /// The business logic for the cases and incidents list rendering goes here.
    /// </summary>
    public partial class CaseloadViewModel : VisitzViewModel, IRecipient<ServiceStateMessage>
    {
        [ObservableProperty]
        public IEnumerable<CaseloadItem> caseload;

        [ObservableProperty]
        public bool isRefreshing;

        public override async void PageCreated()
        {
            WeakReferenceMessenger.Default.Register(this, GetCaseloadService.MakeId());

            var realm = await IcmDataRealm.GetAsync();
            Caseload = realm.All<CaseloadItem>();
        }

        [RelayCommand]
        public async Task RefreshCaseload()
        {
            var info = await VisitzSessionInfo.GetAsync();
            WeakReferenceMessenger.Default.Send(GetCaseloadService.MakeStartMessage(info.Idir));
        }

        [RelayCommand]
        public async void GoToNotes(CaseloadItem caseloadItem)
        {
            await NotesPage.Open(VisitzPage, caseloadItem.CaseIncidentNumber);
        }

        public async Task OpenDebugOptionsPage()
        {
            if (DebugOptions.Enabled)
                await NavigateTo<DebugOptionsPage>();
        }

        public void Receive(ServiceStateMessage message)
        {
            IsRefreshing = message.Status == VisitzService.State.StartPending 
                        || message.Status == VisitzService.State.Running;
        }
    }
}

