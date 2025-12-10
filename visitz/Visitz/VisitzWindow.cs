using Microsoft.Extensions.Logging;
using Visitz.Views.AppLock;
using Visitz.Views.User;

namespace Visitz;

public partial class VisitzWindow : Window
{
    public bool IsActivated { get; private set; }

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
#endif

    protected override void OnActivated()
    {
        base.OnActivated();

        IsActivated = true;
    }

    protected override void OnDeactivated()
    {
        base.OnDeactivated();

        IsActivated = false;
    }
}
