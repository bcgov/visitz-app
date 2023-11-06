using Visitz.Models;
using Visitz.Pages;

namespace Visitz.Views.Navigation;

public partial class NavRailView : ContentView
{
	public event EventHandler<NavItemSelectedEventArgs> NavItemSelected;

	public NavRailView()
	{
		InitializeComponent();
	}

    private void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
		NavItemSelected?.Invoke(this, new NavItemSelectedEventArgs()
		{
			NavItem = e.CurrentSelection[0] as NavItem,
		});
    }

    private async void AvatarView_Tapped(object sender, TappedEventArgs e)
    {
		await Navigator.GoToPage<SessionPage>(modal: true);
    }
}