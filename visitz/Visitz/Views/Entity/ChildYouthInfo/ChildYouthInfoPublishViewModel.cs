using CommunityToolkit.Mvvm.Messaging;
using Visitz.Resources.Localization;
using Visitz.Services;
using Visitz.Services.Base;
using Visitz.Services.Messages;
using Visitz.Services.People;
using Visitz.Views.BaseClasses.Publishing;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.People;
using VisitzModel.Resources.Localization;

namespace Visitz.Views.Entity.ChildYouthInfo;

internal partial class ChildYouthInfoPublishViewModel : PublishViewModel, IRecipient<ServiceStateMessage>
{
    bool _disposed;

    IDraftItem? ChildYouthInfoDraft
    {
        get;
        set
        {
            field = value;

            if (value is ContactEducationDraft edu)
            {
                Title = edu.ContactEducation?.SchoolNameBinding ?? string.Empty;
                RefreshTitle = GeneralStrings.EducationInfo;
            }
            else if (value is ContactMedicalBehavioralDraft med)
            {
                Title = med.ContactMedicalBehavioral?.CategoryBinding ?? string.Empty;
                RefreshTitle = GeneralStrings.MedicalBehavioralInfo;
            }
            else if (value is ContactLanguageDraft lang)
            {
                Title = lang.ContactLanguage?.LanguageNameBinding ?? string.Empty;
                RefreshTitle = GeneralStrings.LanguageInfo;
            }
            else
                throw new InvalidOperationException();
        }
    }

    string RefreshTitle { get; set; } = string.Empty;

    IBusinessObject? BusinessObject { get; set; }

    IcmContact? KeyPlayer { get; set; }

    RecordServiceInfo? RecordServiceInfo { get; set; }

    string PublishId { get; set; } = string.Empty;

    string RefreshId { get; set; } = string.Empty;

    StartServiceMessage? StartRefreshMessage { get; set; }

    public void SetPayload(IBusinessObject businessObject, IDraftItem childYouthInfoDraft)
    {
        BusinessObject = businessObject;
        RecordServiceInfo = new(businessObject);
        KeyPlayer = businessObject.GetKeyPlayer() ?? throw new InvalidOperationException("Missing key player");
        ChildYouthInfoDraft = childYouthInfoDraft;
    }

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        ArgumentNullException.ThrowIfNull(BusinessObject);
        ArgumentNullException.ThrowIfNull(KeyPlayer);
        ArgumentNullException.ThrowIfNull(RecordServiceInfo);
        ArgumentNullException.ThrowIfNull(ChildYouthInfoDraft);

        IcmContact? parentContact = BusinessObject.GetKeyPlayer();
        ArgumentNullException.ThrowIfNull(parentContact);

        PublishId = SubmitChildYouthInfoService.MakeId(
            RecordServiceInfo.Id,
            RecordServiceInfo.Type,
            KeyPlayer.Id,
            ChildYouthInfoDraft
        );

        if (ChildYouthInfoDraft is ContactEducationDraft edu && edu.ContactEducation != null)
        {
            RefreshId = GetContactEducationService.MakeId(edu.ContactEducation.ParentContactIdBinding);
            StartRefreshMessage = GetContactEducationService.MakeStartMessage(parentContact);
        }
        else if (ChildYouthInfoDraft is ContactMedicalBehavioralDraft med && med.ContactMedicalBehavioral != null)
        {
            RefreshId = GetContactMedicalBehavioralService.MakeId(med.ContactMedicalBehavioral.ParentContactIdBinding);
            StartRefreshMessage = GetContactMedicalBehavioralService.MakeStartMessage(parentContact);
        }
        else if (ChildYouthInfoDraft is ContactLanguageDraft lang && lang.ContactLanguage != null)
        {
            RefreshId = GetContactLanguagesService.MakeId(lang.ContactLanguage.ParentContactIdBinding);
            StartRefreshMessage = GetContactLanguagesService.MakeStartMessage(parentContact);
        }
        else
            throw new InvalidOperationException();

        WeakReferenceMessenger.Default.Register(this, PublishId);
        WeakReferenceMessenger.Default.Register(this, RefreshId);

        Wait(string.Format(LocalizedStrings.LoginToPublish, Title));

        Publish();
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            WeakReferenceMessenger.Default.UnregisterAll(this);
            _disposed = true;
        }
        base.Dispose(disposing);
    }

    public override void Publish()
    {
        ArgumentNullException.ThrowIfNull(KeyPlayer);
        ArgumentNullException.ThrowIfNull(RecordServiceInfo);
        ArgumentNullException.ThrowIfNull(ChildYouthInfoDraft);

        var startMessage = SubmitChildYouthInfoService.MakeStartMessage(
            RecordServiceInfo,
            KeyPlayer.Id,
            ChildYouthInfoDraft
        );
        WeakReferenceMessenger.Default.Send(startMessage);
    }

    public void Receive(ServiceStateMessage message)
    {
        if (message.ServiceId == PublishId)
            HandlePublishMessage(message);
        else if (message.ServiceId == RefreshId)
            HandleRefreshMessage(message);
    }

    void HandlePublishMessage(ServiceStateMessage message)
    {
        if (message.Status == VisitzService.State.Running)
        {
            Publishing(string.Format(LocalizedStrings.PublishingToIcm, Title));
        }
        else if (message.FinishedSuccess)
        {
            Published(string.Format(LocalizedStrings.PublishSuccess, Title));

            if (StartRefreshMessage != null)
                WeakReferenceMessenger.Default.Send(StartRefreshMessage);
            else
            {
                RefreshError(
                    string.Format(LocalizedStrings.FailedToRefreshItems, RefreshTitle),
                    "Unexpected null for refresh message"
                );
                Complete();
            }
        }
        else if (message.FinishedCancelled)
        {
            Cancel(string.Format(LocalizedStrings.LoginToPublish, Title));
        }
        else if (message.FinishedError)
        {
            PublishError(LocalizedStrings.FailedToPublishToIcm, message.Message);
        }
    }

    void HandleRefreshMessage(ServiceStateMessage message)
    {
        if (message.Status == VisitzService.State.Running)
        {
            Refreshing(string.Format(LocalizedStrings.RefreshingItems, RefreshTitle));
        }
        else if (message.FinishedSuccess)
        {
            Refreshed(string.Format(LocalizedStrings.RefreshedItems, RefreshTitle));
            Complete();
        }
        else if (message.FinishedError)
        {
            RefreshError(string.Format(LocalizedStrings.FailedToRefreshItems, RefreshTitle), message.Message);
        }
    }
}
