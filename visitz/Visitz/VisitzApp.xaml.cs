using Visitz.Views;

namespace Visitz;

public partial class VisitzApp : Application
{
    public static IServiceProvider VisitzServices => Current.Handler.MauiContext.Services;

    public VisitzApp()
    {
        InitializeComponent();

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

