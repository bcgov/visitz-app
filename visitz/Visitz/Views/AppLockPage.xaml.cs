using Visitz.ViewModels;

namespace Visitz.Views;

public partial class AppLockPage : VisitzPage
{
    /// <summary>
    /// Back button behavior disabled on purpose. We don't want to let users avoid this authentication check.
    /// </summary>
    private static readonly bool BackButtonEnabled = false;

    public static bool IsOpen => Shell.Current?.CurrentPage?.GetType() == typeof(AppLockPage);

    public AppLockPage(AppLockViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    public static async Task TryPrompt()
    {
        if (IsOpen)
            return;

        var lockPage = Application.Current
            .Handler
            .MauiContext
            .Services
            .GetRequiredService<AppLockPage>();

        await Shell.Current.Navigation.PushModalAsync(lockPage, false);
    }

    protected override bool OnBackButtonPressed()
    {
        return BackButtonEnabled;
    }
}
