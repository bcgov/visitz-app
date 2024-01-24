using Visitz.ViewModels;

namespace Visitz.Pages;

public partial class PublishPage : VisitzPage
{
	public PublishPage(PublishViewModel publishViewModel) : base(publishViewModel)
	{
		InitializeComponent();
		BindingContext = ViewModel;
	}

    protected override bool OnBackButtonPressed()
    {
        // Prevent the user from backing out of this screen in favour of using the dismiss button.
        return false;
    }
}