using Visitz.Pages;

namespace Visitz.Views.Navigation;

public partial class NavRailView : ViewModelContentView
{
	public NavRailView() : base(ServiceProvider.GetService<NavRailViewModel>())
	{
		InitializeComponent();
        BindingContext = ViewModel;
	}

    private async void AvatarView_Tapped(object sender, TappedEventArgs e)
    {
		await Navigator.GoToPage<SessionPage>(modal: true);
    }
}