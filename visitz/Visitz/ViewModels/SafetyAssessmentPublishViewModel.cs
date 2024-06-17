using CommunityToolkit.Mvvm.Messaging;
using Visitz.Resources.Localization;
using Visitz.Services;
using Visitz.ViewModels.Entity;
using Visitz.Views.Entity;
using VisitzModel.Messaging;
using VisitzModel.Models;
using VisitzModel.Models.Navigation;
using VisitzModel.Models.SafetyAssess;

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

    public CaseloadItem CaseloadItem {  get; set; }

    public override void Create()
    {
        base.Create();

        WeakReferenceMessenger.Default.Register(this, SubmitSafetyAssessmentService.MakeId(Assessment.IncidentNumber));

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

    public async void Receive(ServiceStateMessage message)
    {
        if (message.Status == VisitzService.State.Running)
            Publishing(LocalizedStrings.PublishingSAToICM);
        else if (message.FinishedSuccess)
        {
            Published(LocalizedStrings.SAPublishedSuccess);
            await DiscardSentDraft();
            RedirectToDetails();
            await Complete();
        }
        else if (message.FinishedCancelled)
            Cancel(LocalizedStrings.LoginToSubmitSA);
        else if (message.FinishedError)
            PublishError(LocalizedStrings.FailedToPublishToIcm, message.Message);
    }

    private async Task DiscardSentDraft()
    {
        await AssessmentDraft.TryDeleteAsync(Assessment);
    }

    private void RedirectToDetails()
    {
		var detailsNav = new EntityNavItem() { ContentViewType = typeof(EntityDetailsView) };
        StrongReferenceMessenger.Default.Send(new EntityNavMessage(detailsNav, CaseloadItem));
    }
}
