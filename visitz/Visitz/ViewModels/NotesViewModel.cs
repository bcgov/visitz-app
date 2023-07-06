using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Visitz.Models;
using Visitz.Pages;
using Visitz.Services;
using Visitz.Storage;

namespace Visitz.ViewModels
{
    /// <summary>
    /// The business logic for the cases notes rendering goes here.
    /// </summary>
	public partial class NotesViewModel : VisitzViewModel, IRecipient<ServiceStateMessage>
    {
        public static readonly string CaseIncidentIdKey = "caseIncidentId";

        public string caseIncidentId;

        [ObservableProperty]
        public CaseloadItem caseIncident;

        [ObservableProperty]
        public IEnumerable<NoteItem> notes;

        [ObservableProperty]
        public bool isRefreshing;

        public override async void PageCreated()
        {
            caseIncidentId = Parameters[CaseIncidentIdKey] as string;

            WeakReferenceMessenger.Default.Register(this, GetNotesService.MakeId(caseIncidentId));

            var realm = await IcmDataRealm.GetAsync();

            CaseIncident = realm.Find<CaseloadItem>(caseIncidentId);

            Notes = realm
                .All<NoteItem>()
                .Where(note => note.IcmId == caseIncidentId);
        }

        [RelayCommand]
        public async Task CaseDetailsTapped()
        {
            await CaseloadItemDetailsPage.Open(VisitzPage, CaseIncident.CaseIncidentNumber);
        }

        [RelayCommand]
        public void RefreshNotes()
        {
            if (CaseIncident == null)
            {
                IsRefreshing = false;
                return;
            }
            var entityTuple = (CaseIncident.CaseIncidentNumber, CaseIncident.EntityType);
            WeakReferenceMessenger.Default.Send(GetNotesService.MakeStartMessage(entityTuple));
        }

        public void Receive(ServiceStateMessage message)
        {
            IsRefreshing = message.Status == VisitzService.State.Running;
        }
    }
}

