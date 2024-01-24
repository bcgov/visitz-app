using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.Messaging;
using Visitz.Messaging;
using Visitz.Models;
using Visitz.Resources.Localization;
using Visitz.Services;

namespace Visitz.Views.Entity;

public partial class EntitySafetyAssessView : ViewModelContentView, ICaseloadItemHolder
{
	protected new EntitySafetyAssessViewModel ViewModel => (EntitySafetyAssessViewModel)base.ViewModel;

    public CaseloadItem CaseloadItem 
	{
		get => ViewModel.CaseloadItem;
		set => ViewModel.CaseloadItem = value;
	}

	public EntitySafetyAssessView() : base(ServiceProvider.GetService<EntitySafetyAssessViewModel>())
	{
		InitializeComponent();
		BindingContext = ViewModel;
	}

    protected override void Creating()
    {
        base.Creating();

		StrongReferenceMessenger.Default.Register<DraftSavedMessage<DraftSavedView.State>>(this, ReceiveAppNavMessage);
    }

    protected override void Destroying()
    {
		StrongReferenceMessenger.Default.UnregisterAll(this);

        base.Destroying();
    }

    private void ReceiveAppNavMessage(object recipient, DraftSavedMessage<DraftSavedView.State> message)
	{
		var thiz = (EntitySafetyAssessView)recipient;

		_ = thiz.DraftSavedIndicator.SetState(message.Value);
	}

    private async void DiscardButton_Clicked(object sender, EventArgs e)
    {
		if (await PromptDiscard())
		{
			ViewModel.Reset();
			await Toast.Make(LocalizedStrings.DiscardedSafetyAssessmentDraft).Show();
		}
    }

	private async static Task<bool> PromptDiscard()
	{
        return await Navigator.CurrentOpenPage.DisplayAlert(
			LocalizedStrings.DiscardDraftQuestion,
            LocalizedStrings.DiscardSafetyAssessmentDraftDescription,
            LocalizedStrings.Discard,
            LocalizedStrings.Cancel);
    }
}