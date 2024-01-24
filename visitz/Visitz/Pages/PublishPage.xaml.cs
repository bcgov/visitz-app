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

        ViewModel.OnCompleted += PublishPage_OnPublished;
    }

    protected override void OnDestroyed()
    {
        ViewModel.OnCompleted -= PublishPage_OnPublished;

        base.OnDestroyed();
    }

    private async void PublishPage_OnPublished(object sender, EventArgs e)
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
}