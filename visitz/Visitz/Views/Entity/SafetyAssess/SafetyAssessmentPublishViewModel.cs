using CommunityToolkit.Mvvm.Messaging;
using Realms;
using Visitz.Resources.Localization;
using Visitz.Services;
using Visitz.Services.Base;
using Visitz.Services.Messages;
using Visitz.Services.SafetyAssessments;
using Visitz.Storage;
using Visitz.Views.BaseClasses.Publishing;
using Visitz.Views.Debugging;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.People;
using VisitzModel.Models.SafetyAssess;

namespace Visitz.Views.Entity.SafetyAssess;

internal partial class SafetyAssessmentPublishViewModel : PublishViewModel, IRecipient<ServiceStateMessage>
{
    string? getAssessmentsServiceId;

    string? submitAssessmentsServiceId;

    RecordServiceInfo? recordServiceInfo;

    SafetyAssessment? Assessment
    {
        get;
        set
        {
            field = value;
            var date = field?.DateOfAssessment?.ToString(SafetyAssessment.DateFormat);

            if (field != null)
                Title = string.Format(LocalizedStrings.PublishSATitle, field.FamilyName, date);
        }
    }

    Realm? Realm { get; set; }

    public IBusinessObject? BusinessObject { get; set; }

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        if (BusinessObject == null)
            throw new InvalidOperationException("BusinessObject is null");

        Realm = await VisitzRealms.GetSafetyAssessmentDraftRealmAsync();
        Assessment =
            SafetyAssessment.FindByIncidentNumber(Realm, BusinessObject.FileNumberBinding)
            ?? throw new InvalidOperationException(
                $"Safety assessment for '{BusinessObject.FileNumberBinding}' missing"
            );

        IcmContact keyPlayer =
            BusinessObject.GetKeyPlayer() ?? throw new InvalidOperationException("Missing key player");
        recordServiceInfo = new RecordServiceInfo(
            BusinessObject.EntityType,
            BusinessObject.EntitySubtype,
            BusinessObject.Id,
            Assessment.IncidentNumber,
            keyPlayer.FirstName,
            keyPlayer.LastName
        );

        submitAssessmentsServiceId = SubmitSafetyAssessmentService.MakeId(Assessment.IncidentNumber);
        getAssessmentsServiceId = GetSafetyAssessmentsService.MakeId(recordServiceInfo);

        WeakReferenceMessenger.Default.Register(this, submitAssessmentsServiceId);
        WeakReferenceMessenger.Default.Register(this, getAssessmentsServiceId);

        Wait(LocalizedStrings.LoginToSubmitSA);

        Publish();
    }

    bool disposed;

    protected override void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
            WeakReferenceMessenger.Default.UnregisterAll(this);

            Realm?.Dispose();

            disposed = true;
        }

        base.Dispose(disposing);
    }

    public override void Publish()
    {
        ArgumentNullException.ThrowIfNull(Assessment);

        var msg = SubmitSafetyAssessmentService.MakeStartMessage(Assessment);
        WeakReferenceMessenger.Default.Send(msg);
    }

    private void CallGetService()
    {
        ArgumentNullException.ThrowIfNull(recordServiceInfo);

        var startMessage = GetSafetyAssessmentsService.MakeStartMessage(recordServiceInfo);
        WeakReferenceMessenger.Default.Send(startMessage);
    }

    public async void Receive(ServiceStateMessage message)
    {
        if (message.ServiceId == submitAssessmentsServiceId)
        {
            if (message.Status == VisitzService.State.Running)
                Publishing(LocalizedStrings.PublishingSAToICM);
            else if (message.FinishedSuccess)
            {
                Published(LocalizedStrings.SAPublishedSuccess);
                await DiscardSentDraft();
                CallGetService();
            }
            else if (message.FinishedCancelled)
                Cancel(LocalizedStrings.LoginToSubmitSA);
            else if (message.FinishedError)
                PublishError(LocalizedStrings.FailedToPublishToIcm, message.Message);
        }
        else if (message.ServiceId == getAssessmentsServiceId)
        {
            if (message.Status == VisitzService.State.Running)
                Refreshing(LocalizedStrings.RefreshingSAs);
            else if (message.FinishedSuccess)
            {
                Refreshed(LocalizedStrings.RefreshedSAsOnDevice);
                Complete();
            }
            else if (message.FinishedError)
                RefreshError(LocalizedStrings.FailedToRefreshSAs, message.Message);
        }
    }

    private async Task DiscardSentDraft()
    {
        if (DebugOptions.Default.KeepSafetyAssessmentDraftOnPublish)
            return;

        ArgumentNullException.ThrowIfNull(Assessment);

        await AssessmentDraft.TryDeleteAsync(Assessment);
    }
}
