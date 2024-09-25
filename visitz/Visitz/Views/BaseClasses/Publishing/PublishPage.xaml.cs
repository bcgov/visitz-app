using Visitz.Extensions;
using Visitz.Views.Surveys;

namespace Visitz.Views.BaseClasses.Publishing;

public partial class PublishPage : VisitzPage
{
    public new PublishViewModel ViewModel => base.ViewModel as PublishViewModel;

	public PublishPage(PublishViewModel publishViewModel) : base(publishViewModel)
	{
		InitializeComponent();
		BindingContext = ViewModel;
	}

    protected override void OnCreated()
    {
        base.OnCreated();

        ViewModel.OnCompleted += PublishPage_OnCompleted;
    }

    protected override void OnDestroyed()
    {
        ViewModel.OnCompleted -= PublishPage_OnCompleted;

        base.OnDestroyed();
    }

    private async void PublishPage_OnCompleted(object sender, EventArgs e)
    {
		await Task.WhenAll(AnimateCountdown(), Task.Delay(PublishViewModel.DismissDuration));

		await TryPopAsync();

		await FeedbackSurveyPage.TryOpen();
	}

	async Task AnimateCountdown()
	{
		DismissProgressBar.IsVisible = true;

		DismissProgressBar.Progress = 1.0d;

		await DismissProgressBar.ProgressTo(0.0d, (uint)PublishViewModel.DismissDuration, Easing.Linear);

		DismissProgressBar.IsVisible = false;
	}

    protected override bool OnBackButtonPressed()
    {
        // Prevent the user from backing out of this screen in favour of using the dismiss button.
        return false;
    }

    private async void PublishStatus_Tapped(object sender, TappedEventArgs e)
    {
        if (ViewModel.ShowPublishErrorIcon && ViewModel.PublishErrorDetail?.Length > 0)
            await this.DisplayErrorAlert(ViewModel.PublishErrorDetail);
    }

    private async void RefreshStatus_Tapped(object sender, TappedEventArgs e)
    {
        if (ViewModel.ShowRefreshErrorIcon && ViewModel.RefreshErrorDetail?.Length > 0)
            await this.DisplayErrorAlert(ViewModel.RefreshErrorDetail);
    }

	async Task TryPopAsync()
	{
		var nav = Navigator.Navigation;

		if (nav.NavigationStack.Contains(this))
			await nav.PopAsync();
		else if (nav.ModalStack.Contains(this))
			await nav.PopModalAsync();
	}

	private async void DismissButton_Clicked(object sender, EventArgs e)
	{
		await TryPopAsync();
    }
}
