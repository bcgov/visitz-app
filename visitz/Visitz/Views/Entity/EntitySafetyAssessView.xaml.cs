using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.Messaging;
using Visitz.Models;
using Visitz.Resources.Localization;
using Visitz.Services;

namespace Visitz.Views.Entity;

public partial class EntitySafetyAssessView : ViewModelContentView, ICaseloadItemHolder, IRecipient<ServiceStateMessage>
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

		var id = SubmitSafetyAssessmentService.MakeId(CaseloadItem);
		WeakReferenceMessenger.Default.Register(this, id);
    }

    protected override void Destroying()
    {
		WeakReferenceMessenger.Default.UnregisterAll(this);

        base.Destroying();
    }

    public async void Receive(ServiceStateMessage message)
    {
		if (message.Status == VisitzService.State.Running)
			// Temporary, to be replaced with better UI/UX
			_ = Toast.Make("Submitting safety assessment").Show();
		if (message.FinishedError)
			await Navigator.CurrentOpenPage.DisplayAlert(
				LocalizedStrings.Error,
				message.Message, 
				LocalizedStrings.Ok);
    }
}