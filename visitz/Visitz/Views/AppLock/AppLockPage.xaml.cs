using Visitz.Views.BaseClasses;

namespace Visitz.Views.AppLock;

public partial class AppLockPage : VisitzPage
{
    /// <summary>
    /// Back button behavior disabled on purpose. We don't want to let users avoid this authentication check.
    /// </summary>
    public static readonly bool BackButtonEnabled = false;

    public static bool IsOpen => Navigator.CurrentOpenModal?.GetType() == typeof(AppLockPage);

    public AppLockPage(AppLockViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    public static async Task TryPrompt()
    {
        if (IsOpen)
            return;

        var lockPage = ServiceProvider.Current.GetRequiredService<AppLockPage>();
        await Navigator.Navigation.PushModalAsync(lockPage, false);
    }

    protected override bool OnBackButtonPressed()
    {
        return BackButtonEnabled;
    }

    private async void Unlock_Clicked(object sender, EventArgs e)
    {
        await ((AppLockViewModel)ViewModel).TryPromptAuthentication();
    }
}
