using Visitz.Models;

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

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {

    }
}