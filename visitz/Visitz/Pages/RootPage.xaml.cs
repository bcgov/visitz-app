using CommunityToolkit.Mvvm.Messaging;
using Visitz.Messaging;
using Visitz.Models;

namespace Visitz.Pages;

public partial class RootPage : ContentPage
{
	public RootPage()
	{
		InitializeComponent();

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
}