using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.Messaging;
using Visitz.Messaging;
using Visitz.Models;
using Visitz.Resources.Localization;
using Visitz.Services;
using Visitz.Utilities;

namespace Visitz.Views.Entity;

public partial class EntitySafetyAssessView : ViewModelContentView, ICaseloadItemHolder, IRecipient<ServiceStateMessage>
{
	protected new EntitySafetyAssessViewModel ViewModel => (EntitySafetyAssessViewModel)base.ViewModel;

    public CaseloadItem CaseloadItem 
	{
		get => ViewModel.CaseloadItem;
		set => ViewModel.CaseloadItem = value;
	}

	private readonly Debouncer _debouncer = new(TimeSpan.FromMilliseconds(700.0d));

	public EntitySafetyAssessView() : base(ServiceProvider.GetService<EntitySafetyAssessViewModel>())
	{
		InitializeComponent();
		BindingContext = ViewModel;
	}

    protected override void Creating()
    {
        base.Creating();

		var id = SubmitSafetyAssessmentService.MakeId(CaseloadItem);
		WeakReferenceMessenger.Default.Register(this, id);
		StrongReferenceMessenger.Default.Register<DraftSavedMessage<DraftSavedView.State>>(this, ReceiveAppNavMessage);
    }

    protected override void Destroying()
    {
		StrongReferenceMessenger.Default.UnregisterAll(this);
		WeakReferenceMessenger.Default.UnregisterAll(this);

		_debouncer?.Dispose();

        base.Destroying();
    }

    public async void Receive(ServiceStateMessage message)
    {
		if (message.Status == VisitzService.State.Running)
			// Temporary, to be replaced with better UI/UX
			_ = Toast.Make("Submitting safety assessment").Show();

		if (message.FinishedSuccess)
            await Navigator.CurrentOpenPage.DisplayAlert(
                "Success",
                "Safety assessment was submitted successfully.",
                LocalizedStrings.Ok);

        if (message.FinishedError)
			await Navigator.CurrentOpenPage.DisplayAlert(
				LocalizedStrings.Error,
				message.Message, 
				LocalizedStrings.Ok);
    }

    private void ReceiveAppNavMessage(object recipient, DraftSavedMessage<DraftSavedView.State> message)
	{
		var thiz = (EntitySafetyAssessView)recipient;

		_ = thiz.DraftSavedIndicator.SetState(message.Value);

		_ = _debouncer.Debounce(() =>
		{
			DraftSavedView.State endState = message.Value.Equals(DraftSavedView.State.None)
				? DraftSavedView.State.None
				: DraftSavedView.State.Saved;

			_ = DraftSavedIndicator.SetState(endState);
		});
	}
}