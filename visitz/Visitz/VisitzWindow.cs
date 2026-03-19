using Visitz.Views.AppLock;
using Visitz.Views.User;

namespace Visitz;

public partial class VisitzWindow : Window
{
    public VisitzWindow() { }

    public VisitzWindow(Page page)
        : base(page) { }

    protected override async void OnCreated()
    {
        base.OnCreated();
#if WINDOWS
        ApplyDefaultWindowLayout(this);
#endif
        await SessionPage.TryOpenAsync(animated: false);
        await AppLockPage.TryPrompt(promptOnAppearing: true);
    }

    protected override async void OnStopped()
    {
        base.OnStopped();

        await AppLockPage.TryPrompt(promptOnAppearing: false);
    }

#if WINDOWS
    private static partial Window ApplyDefaultWindowLayout(Window window);

    partial void TryRunAutoRefresh();

    partial void OnWindowFocusChanged(bool focused);
#endif

    protected override async void OnActivated()
    {
        base.OnActivated();

        await SessionPage.TryOpenAsync(animated: false);

#if WINDOWS
        // Run in OnActivated instead of OnResumed to respond to window focus events
        TryRunAutoRefresh();
        OnWindowFocusChanged(true);
#endif
    }

#if WINDOWS
    protected override void OnDeactivated()
    {
        base.OnDeactivated();
        OnWindowFocusChanged(false);
    }
#endif
}
