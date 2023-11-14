using Visitz.Authentication.Keycloak;
using Visitz.Models;
using Visitz.Pages;
namespace Visitz.Views.Navigation;

public partial class NavRailView : ViewModelContentView
{
    public event EventHandler<NavItemSelectedEventArgs> NavItemSelected;

	public NavRailView() : base(ServiceProvider.GetService<NavRailViewModel>())
	{
		InitializeComponent();
        BindingContext = ViewModel;
	}

    protected override async void Creating()
    {
        base.Creating();

        VisitzSession.SessionChanged += VisitzSession_SessionChanged;
        await SetInitials();
    }

    protected override void Destroying()
    {
        VisitzSession.SessionChanged -= VisitzSession_SessionChanged;

        base.Destroying();
    }

    private async Task SetInitials()
    {
        var info = await VisitzSessionInfo.GetAsync();

        Avatar.Text = info.UserInitials;
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

    private async void VisitzSession_SessionChanged(object sender, Authentication.Keycloak.Events.SessionChangedEventArgs e)
    {
        await SetInitials();
    }
}