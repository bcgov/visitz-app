using Visitz.ViewModels;

namespace Visitz.Pages;

public partial class AppLockPage : VisitzPage
{
    /// <summary>
    /// Back button behavior disabled on purpose. We don't want to let users avoid this authentication check.
    /// </summary>
    private static readonly bool BackButtonEnabled = false;

    public static bool IsOpen => VisitzApp.CurrentOpenModal?.GetType() == typeof(AppLockPage);

    public AppLockPage(AppLockViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    public static async Task TryPrompt()
    {
        if (IsOpen)
            return;

        var lockPage = VisitzApp.VisitzServices.GetRequiredService<AppLockPage>();
        await VisitzApp.Navigation.PushModalAsync(lockPage, false);
    }

    protected override bool OnBackButtonPressed()
    {
        return BackButtonEnabled;
    }

    private async void Unlock_Clicked(object sender, EventArgs e)
    {
        await ((AppLockViewModel)ViewModel).PromptAuthentication();
    }
}
