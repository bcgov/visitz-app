using CommunityToolkit.Mvvm.Messaging;
using Realms;
using Visitz.Models.SafetyAssess;
using Visitz.Resources.Localization;
using Visitz.Services;

namespace Visitz.ViewModels;

internal partial class SafetyAssessmentPublishViewModel : PublishViewModel, IRecipient<ServiceStateMessage>
{
    private SafetyAssessment assessment;

    public SafetyAssessment Assessment 
    {
        get => assessment;
        set
        {
            assessment = value;
            var date = assessment.DateOfAssessment.ToString(SafetyAssessment.DateFormat);

            Title = string.Format(LocalizedStrings.PublishSATitle, assessment.FamilyName, date);
        }
    }

    public override void PageCreated()
    {
        base.PageCreated();

        WeakReferenceMessenger.Default.Register(this, SubmitSafetyAssessmentService.MakeId(Assessment.IncidentNumber));

        Wait(LocalizedStrings.LoginToSubmitSA);

        Publish();
    }

    public override void PageDestroyed()
    {
        WeakReferenceMessenger.Default.UnregisterAll(this);

        base.PageDestroyed();
    }

    public override void Publish()
    {
        var msg = SubmitSafetyAssessmentService.MakeStartMessage(Assessment);
        WeakReferenceMessenger.Default.Send(msg);
    }

    public async void Receive(ServiceStateMessage message)
    {
        if (message.Status == VisitzService.State.Running)
            Publishing(LocalizedStrings.PublishingSAToICM);
        else if (message.FinishedSuccess)
        {
            Published(LocalizedStrings.SAPublishedSuccess);
            await Complete();
        }
        else if (message.FinishedCancelled)
            Cancel(LocalizedStrings.LoginToSubmitSA);
        else if (message.FinishedError)
            PublishError(LocalizedStrings.FailedToPublishToIcm, message.Message);
    }
}
