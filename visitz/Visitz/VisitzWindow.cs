using Visitz.Views.AppLock;
using Visitz.Views.User;

namespace Visitz;

public partial class VisitzWindow : Window
{
    public VisitzWindow() { }

    public VisitzWindow(Page page) : base(page) { }

    protected async override void OnCreated()
    {
        base.OnCreated();

#if WINDOWS
        ApplyDefaultWindowLayout(this);
#endif

        await SessionPage.TryOpenAsync(animated: false);

        await AppLockPage.TryPrompt(promptOnAppearing: true);
    }

    protected async override void OnStopped()
    {
        base.OnStopped();

        await AppLockPage.TryPrompt(promptOnAppearing: false);
    }

#if WINDOWS
    private static partial Window ApplyDefaultWindowLayout(Window window);

    partial void TryRunAutoRefresh();
#endif

    protected override async void OnActivated()
    {
        base.OnActivated();

        await SessionPage.TryOpenAsync(animated: false);

#if WINDOWS
        // Run in OnActivated instead of OnResumed to respond to window focus events
        TryRunAutoRefresh();
#endif
    }
}
