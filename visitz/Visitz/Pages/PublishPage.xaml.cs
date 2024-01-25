using Visitz.Resources.Localization;
using Visitz.ViewModels;

namespace Visitz.Pages;

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

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        if (ViewModel.ShowPublishErrorIcon && ViewModel.PublishErrorDetail?.Length > 0)
            await DisplayAlert(LocalizedStrings.Error, ViewModel.PublishErrorDetail, LocalizedStrings.Ok);
    }
}