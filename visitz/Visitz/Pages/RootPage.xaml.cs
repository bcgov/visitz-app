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
        ContentPane.Clear();
        ContentPane.Add(view);
	}
}