using Visitz.Pages;
using Visitz.Services;

namespace Visitz;

public partial class VisitzApp : Application
{
    public static INavigation Navigation => Current.MainPage.Navigation;

    public static Page CurrentOpenModal
    {
        get
        {
            int last = Navigation.ModalStack.Count - 1;
            return last >= 0 ? Navigation.ModalStack[last] : null;
        }
    }

    public ServiceHandler ServiceHandler { get; private set; }

    public VisitzApp()
    {
        InitializeComponent();

        // TODO: Get this working with the DI system
        // DI setup has been disabled for now in VisitzScreens
        MainPage = new NavigationPage(CaseloadPage.GetInstance());
    }

    protected async override void OnStart()
    {
        ConsoleTrace.TraceMethod(this);

        base.OnStart();
        
        ServiceHandler = ServiceProvider.Current.GetService<ServiceHandler>();

        await AppLockPage.TryPrompt();
    }

    protected async override void OnResume()
    {
        ConsoleTrace.TraceMethod(this);

        base.OnResume();
        
        await AppLockPage.TryPrompt();
    }
}
