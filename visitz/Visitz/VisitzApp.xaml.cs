using Visitz.Views;

namespace Visitz;

public partial class VisitzApp : Application
{
    public VisitzApp()
    {
        InitializeComponent();

        MainPage = new VisitzShell();
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

