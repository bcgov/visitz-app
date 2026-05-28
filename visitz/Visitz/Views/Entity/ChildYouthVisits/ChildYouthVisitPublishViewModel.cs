using CommunityToolkit.Mvvm.Messaging;
using Realms;
using Visitz.Resources.Localization;
using Visitz.Services.Messages;
using Visitz.Services.Visits;
using Visitz.Storage;
using Visitz.Views.BaseClasses.Publishing;
using VisitzModel.Extensions;
using VisitzModel.Models.InPersonVisits;
using ServiceState = Visitz.Services.Base.VisitzService.State;

namespace Visitz.Views.Entity.ChildYouthVisits;

internal partial class ChildYouthVisitPublishViewModel : PublishViewModel, IRecipient<ServiceStateMessage>
{
    bool _disposed;

    public string BusinessObjectId { get; set; }

    private PersonVisit _visit;

    public PersonVisit Visit
    {
        get => _visit;
        set
        {
            if (_visit != null)
                WeakReferenceMessenger.Default.UnregisterAll(this);

            _visit = value;

            if (value != null)
            {
                _getVisitsId = GetVisitsService.MakeId(value.ParentId);
                WeakReferenceMessenger.Default.Register(this, _getVisitsId);

                _postVisitId = PostVisitService.MakeId(value);
                WeakReferenceMessenger.Default.Register(this, _postVisitId);

                _postAndRefreshId = PostAndRefreshVisitService.MakeId(value);
                WeakReferenceMessenger.Default.Register(this, _postAndRefreshId);
            }
            else
                WeakReferenceMessenger.Default.UnregisterAll(this);
        }
    }

    string _getVisitsId;
    string _postVisitId;
    string _postAndRefreshId;

    Realm VisitDraftRealm { get; set; }

    public ChildYouthVisitPublishViewModel()
    {
        Wait(LocalizedStrings.LoginToSubmitVisit);
    }

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        VisitDraftRealm = await VisitzRealms.GetPersonVisitDraftsRealmAsync();
        Visit = VisitDraftRealm.Find<PersonVisitDraft>(BusinessObjectId).Visit;

        Publish();
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            VisitDraftRealm?.Dispose();
            VisitDraftRealm = null;

            WeakReferenceMessenger.Default.UnregisterAll(this);

            _disposed = true;
        }

        base.Dispose(disposing);
    }

    public override void Publish()
    {
        WeakReferenceMessenger.Default.Send(PostAndRefreshVisitService.MakeStartMessage(Visit));
    }

    public async void Receive(ServiceStateMessage message)
    {
        if (message.ServiceId == _postAndRefreshId)
        {
            if (message.Status == ServiceState.Running)
                Publishing(LocalizedStrings.PublishingVisit);
            else if (message.FinishedSuccess)
                Complete();
            else if (message.FinishedError)
                PublishError(LocalizedStrings.FailedToPublishToIcm, message.Message);
            else if (message.FinishedCancelled)
                Cancel(LocalizedStrings.LoginToSubmitVisit);
        }
        else if (message.ServiceId == _postVisitId)
        {
            if (message.FinishedSuccess)
            {
                Published(LocalizedStrings.VisitPublishedToIcm);
                await DiscardPublishedDraft();
            }
            else if (message.FinishedError)
                PublishError(LocalizedStrings.FailedToPublishToIcm, message.Message);
        }
        else if (message.ServiceId == _getVisitsId)
        {
            if (message.Status == ServiceState.Running)
                Refreshing(LocalizedStrings.RefreshingVisits);
            else if (message.FinishedSuccess)
                Refreshed(LocalizedStrings.RefreshedVisitsOnDevice);
            else if (message.FinishedError)
                RefreshError(LocalizedStrings.FailedToRefreshVisits, message.Message);
        }
    }

    async Task DiscardPublishedDraft()
    {
        await VisitDraftRealm.WriteAsync(() => VisitDraftRealm.DeleteByIds<PersonVisitDraft>([Visit.ParentId]));
    }
}
