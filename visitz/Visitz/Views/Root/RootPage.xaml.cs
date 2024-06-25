using CommunityToolkit.Mvvm.Messaging;
using Visitz.Animations;
using Visitz.Views.BaseClasses;
using Visitz.Views.Snackbar;
using VisitzModel.Messaging;
using VisitzModel.Models.Navigation;

namespace Visitz.Views.Root;

public partial class RootPage : VisitzPage, ISnackbarPresenter
{
	VisitzSnackbar Snackbar { get; set; }

	public RootPage() : base(ServiceProvider.GetService<RootViewModel>())
	{
		InitializeComponent();
        BindingContext = ViewModel;

        StrongReferenceMessenger.Default.Register<AppNavMessage>(this, ReceiveAppNavMessage);
	}

    private void ReceiveAppNavMessage(object recipient, AppNavMessage message)
    {
        if (message.Value is NavItem nav)
        {
            var content = (ContentView)ServiceProvider.GetService(nav.ContentViewType);

            if (content == null)
                throw new InvalidOperationException("Requested navigation item was null");

            SetContent(content);
        }
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

	public void SetSnackbar(VisitzSnackbar snackbar)
	{
		if (Snackbar != null)
			Snackbar.ShouldClose -= Snackbar_ShouldClose;

		Snackbar = snackbar;
		SnackbarContainer.Content = Snackbar;
		SnackbarContainer.IsVisible = Snackbar != null;

		if (Snackbar != null)
		{
			Snackbar.ShouldClose += Snackbar_ShouldClose;
			_ = new VisibilityAnimation(showView: true, 150).Animate(Snackbar);
		}
	}

	public void Snackbar_ShouldClose(object sender, EventArgs e)
	{
		_ = AnimateCloseSnackbar();
	}

	private async Task AnimateCloseSnackbar()
	{
		await new VisibilityAnimation(showView: false, 150).Animate(Snackbar);
		SetSnackbar(null);
	}
}
