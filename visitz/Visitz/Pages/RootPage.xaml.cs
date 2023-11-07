using Visitz.Views.Caseload;
using Visitz.Views.Navigation;

namespace Visitz.Pages;

public partial class RootPage : ContentPage
{
	public RootPage()
	{
		InitializeComponent();
	}

    private void NavRailView_NavItemSelected(object sender, NavItemSelectedEventArgs e)
    {
        SetContent(new Label()
        {
            Text = e.NavItem.Text,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.FillAndExpand,
            HorizontalTextAlignment = TextAlignment.Center,
        });
    }

    private void SetContent(IView view)
	{
        if (view is View v)
        {
            
#pragma warning disable CS0618 // Type or member is obsolete
// StackLayout with FillAndExpand has so far been the most reliable layout mechanism in MAUI, so we'll
// suppress compiler warnings about it.

            v.HorizontalOptions = LayoutOptions.FillAndExpand;
            v.VerticalOptions = LayoutOptions.FillAndExpand;
#pragma warning restore CS0618 // Type or member is obsolete
        }

        ContentPane.Clear();
        ContentPane.Add(view);
	}
}