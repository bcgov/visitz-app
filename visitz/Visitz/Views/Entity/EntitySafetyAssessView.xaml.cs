using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.Messaging;
using Visitz.Resources.Localization;
using Visitz.ViewModels.Entity;
using VisitzModel.Messaging;
using VisitzModel.Models;

namespace Visitz.Views.Entity;

public partial class EntitySafetyAssessView : ViewModelContentView, ICaseloadItemHolder
{
	protected new EntitySafetyAssessViewModel ViewModel => (EntitySafetyAssessViewModel)base.ViewModel;

	// It's preferable to use lifecycle methods to determine when auto-scrolling is allowed, but MAUI's lifecycles can
	// be unreliable--so we'll use a time-delayed bool.
	private bool canAutoScroll;

    public CaseloadItem CaseloadItem 
	{
		get => ViewModel.CaseloadItem;
		set => ViewModel.CaseloadItem = value;
	}

	public EntitySafetyAssessView() : base(ServiceProvider.GetService<EntitySafetyAssessViewModel>())
	{
		InitializeComponent();
		BindingContext = ViewModel;
		
		DelayCanAutoScroll();
	}

	private async void DelayCanAutoScroll()
	{
		await Task.Delay(1500);
		canAutoScroll = true;
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
			SnackbarHandler.ShowText(LocalizedStrings.DiscardedSafetyAssessmentDraft);
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

    private async void SomeChildrenPlaced_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
		if (canAutoScroll && e.Value)
		{
			await Task.Delay(100);
			await MainScrollView.ScrollToAsync(ChildrenInCareSection.X, ChildrenInCareSection.Y, true);
		}
    }
}