using CommunityToolkit.Mvvm.Messaging;
using Visitz.Resources.Localization;
using Visitz.Services;
using Visitz.Services.Base;
using Visitz.Services.SafetyAssessments;
using Visitz.Views.BaseClasses.Publishing;
using Visitz.Views.Debugging;
using VisitzModel.Extensions.EntityTypes;
using VisitzModel.Models;
using VisitzModel.Models.SafetyAssess;

namespace Visitz.Views.Entity.SafetyAssess;

internal partial class SafetyAssessmentPublishViewModel : PublishViewModel, IRecipient<ServiceStateMessage>
{
    string getAssessmentsServiceId;
    string submitAssessmentsServiceId;
    RecordServiceInfo recordServiceInfo;
    private SafetyAssessment assessment;

    public SafetyAssessment Assessment
    {
        get => assessment;
        set
        {
            assessment = value;
            var date = assessment.DateOfAssessment?.ToString(SafetyAssessment.DateFormat);

            Title = string.Format(LocalizedStrings.PublishSATitle, assessment.FamilyName, date);
        }
    }

    public CaseloadItem CaseloadItem { get; set; }

    public override void Create()
    {
        base.Create();

        recordServiceInfo = new RecordServiceInfo(
                CaseloadItem.EntityType.ParseEntityType(),
                CaseloadItem.RowId,
                Assessment.IncidentNumber,
                CaseloadItem.KeyPlayer.FirstName,
                CaseloadItem.KeyPlayer.LastName);

        submitAssessmentsServiceId = SubmitSafetyAssessmentService.MakeId(Assessment.IncidentNumber);
        getAssessmentsServiceId = GetSafetyAssessmentsService.MakeId(recordServiceInfo);

        WeakReferenceMessenger.Default.Register(this, submitAssessmentsServiceId);
        WeakReferenceMessenger.Default.Register(this, getAssessmentsServiceId);

        Wait(LocalizedStrings.LoginToSubmitSA);

        Publish();
    }

    public override void Destroy()
    {
        WeakReferenceMessenger.Default.UnregisterAll(this);

        base.Destroy();
    }

    public override void Publish()
    {
        var msg = SubmitSafetyAssessmentService.MakeStartMessage(Assessment);
        WeakReferenceMessenger.Default.Send(msg);
    }

    private void CallGetService()
    {
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
                CallGetService();
                await DiscardSentDraft();
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
        if (DebugOptions.KeepSafetyAssessmentDraftOnPublish)
            return;

        await AssessmentDraft.TryDeleteAsync(Assessment);
    }
}
