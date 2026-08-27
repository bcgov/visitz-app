using Visitz.Animations;
using Visitz.Views.Snackbar;

namespace Visitz.Views.Debugging;

public partial class DebugOptionsPage : ContentPage, ISnackbarPresenter
{
    VisitzSnackbar? Snackbar { get; set; }

    public static bool IsOpen => Navigator.CurrentOpenPage?.GetType() == typeof(DebugOptionsPage);

    public DebugOptionsPage()
    {
        InitializeComponent();
    }

    public static async Task TryOpen(Page? fromPage = null)
    {
        if (DebugOptions.Default.Enabled && !IsOpen)
            await Navigator.GoToPage<DebugOptionsPage>(fromPage);
    }

    public void SetSnackbar(VisitzSnackbar? snackbar)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Snackbar?.ShouldClose -= Snackbar_ShouldClose;

            Snackbar = snackbar;
            SnackbarContainer.Content = Snackbar;
            SnackbarContainer.IsVisible = Snackbar != null;

            if (Snackbar != null)
            {
                Snackbar.ShouldClose += Snackbar_ShouldClose;
                _ = new VisibilityAnimation(showView: true, 150).Animate(Snackbar);
            }
        });
    }

    public void Snackbar_ShouldClose(object? sender, EventArgs e)
    {
        _ = AnimateCloseSnackbar();
    }

    private async Task AnimateCloseSnackbar()
    {
        if (Snackbar != null)
            await new VisibilityAnimation(showView: false, 150).Animate(Snackbar);

        SetSnackbar(null);
    }
}
