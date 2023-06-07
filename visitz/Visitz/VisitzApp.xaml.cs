using Visitz.Views;

namespace Visitz;

public partial class VisitzApp : Application
{
    public static IServiceProvider VisitzServices => Current.Handler.MauiContext.Services;

    public static INavigation Navigation => Current.MainPage.Navigation;

    public VisitzApp()
    {
        InitializeComponent();

        // TODO: Get this working with the DI system
        // DI setup has been disabled for now in VisitzScreens
        MainPage = new NavigationPage(new CaseloadPage(new CaseloadViewModel()));
    }

    protected async override void OnStart()
    {
        base.OnStart();

        await AppLockPage.TryPrompt();
    }

    protected async override void OnResume()
    {
        base.OnResume();

        await AppLockPage.TryPrompt();
    }
}

