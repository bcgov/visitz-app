using CommunityToolkit.Mvvm.Messaging;
using Visitz.Resources.Localization;
using Visitz.Services;
using Visitz.Services.Base;
using Visitz.Services.Messages;
using Visitz.Services.Notes;
using Visitz.Storage;
using Visitz.Views.BaseClasses.Publishing;
using VisitzApi.Models.Notes;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.Notes;

namespace Visitz.Views.Entity.Notes;

public partial class NotePublishViewModel : PublishViewModel, IRecipient<ServiceStateMessage>
{
    private SubmitNoteEntity submitNoteEntity;
    private RecordServiceInfo parentInfo;

    private string submitAndGetNotesServiceId;
    private string submitNotesServiceId;
    private string getNotesServiceId;

    public void Init(IBusinessObject businessObject, SubmitNoteEntity submitNote)
    {
        Title = businessObject.DisplayName;
        submitNoteEntity = submitNote;
        parentInfo = new(businessObject);

        var id = businessObject.FileNumber;
        var notePeriod = submitNoteEntity.NotePeriod;

        submitAndGetNotesServiceId = SubmitAndGetNotesService.MakeId(id, notePeriod);
        submitNotesServiceId = SubmitNoteService.MakeId(id, notePeriod);
        getNotesServiceId = GetNotesService.MakeId(id);
    }

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        Wait(LocalizedStrings.LoginToSubmitNotes);

        WeakReferenceMessenger.Default.Register(this, submitAndGetNotesServiceId);
        WeakReferenceMessenger.Default.Register(this, submitNotesServiceId);
        WeakReferenceMessenger.Default.Register(this, getNotesServiceId);

        Publish();
    }

    bool disposed;

    protected override void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
            WeakReferenceMessenger.Default.UnregisterAll(this);
            disposed = true;
        }

        base.Dispose(disposing);
    }

    public override void Publish()
    {
        WeakReferenceMessenger.Default.Send(SubmitAndGetNotesService.MakeStartMessage(submitNoteEntity, parentInfo));
    }

    public async void Receive(ServiceStateMessage message)
    {
        if (message.ServiceId == submitAndGetNotesServiceId)
        {
            if (message.Status == VisitzService.State.Running)
                Publishing(LocalizedStrings.PublishingNotesToIcm);
            else if (message.FinishedSuccess)
                Complete();
            else if (message.FinishedError)
                PublishError(LocalizedStrings.FailedToPublishToIcm, message.Message);
            else if (message.FinishedCancelled)
                Cancel(LocalizedStrings.LoginToSubmitNotes);
        }
        else if (message.ServiceId == submitNotesServiceId)
        {
            if (message.FinishedSuccess)
            {
                Published(LocalizedStrings.NotesPublishedToIcm);
                await DiscardPublishedDraft();
            }
            if (message.FinishedError)
                PublishError(LocalizedStrings.FailedToPublishToIcm, message.Message);
        }
        else if (message.ServiceId == getNotesServiceId)
        {
            if (message.Status == VisitzService.State.Running)
                Refreshing(LocalizedStrings.RefreshingNotes);
            else if (message.FinishedSuccess)
                Refreshed(LocalizedStrings.RefreshedNotesOnDevice);
            else if (message.FinishedError)
                RefreshError(LocalizedStrings.FailedToRefreshNotes, message.Message);
        }
    }

    private async Task DiscardPublishedDraft()
    {
        using var realm = await VisitzRealms.GetNoteDraftsRealmAsync();
        await NoteDraft.Delete(realm, submitNoteEntity.EntityNumber);
    }
}
