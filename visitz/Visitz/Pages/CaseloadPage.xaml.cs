using CommunityToolkit.Maui.Core.Platform;
using Visitz.ViewModels;

namespace Visitz.Pages;

public partial class CaseloadPage : VisitzPage
{
    private static CaseloadPage Instance { get; set; }

    private new CaseloadViewModel ViewModel => base.ViewModel as CaseloadViewModel;

    public static CaseloadPage GetInstance()
    {
        Instance ??= new CaseloadPage(new CaseloadViewModel());
        return Instance;
    }

    public CaseloadPage(CaseloadViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnCreated()
    {
        base.OnCreated();

        // TODO: Clean this up so starting-indices are set correctly instead of using delays.
        // Only using a delay because NOT using one brings nothing but issues. I've tried
        // different lifecycle functions to place this code but none work as well as this.
        await Task.Delay(100);
        SortPicker.SelectedIndex = 0;
        FilterPicker.SelectedIndex = 0;
    }

    private void Picker_SelectedIndexChanged(object sender, EventArgs e)
    {
        ViewModel.ApplyCaseloadQuery();
    }

    private async void CaseloadSearchBar_SearchButtonPressed(object sender, EventArgs e)
    {
        ViewModel.SearchCaseload();
        await CaseloadSearchBar.HideKeyboardAsync(CancellationToken.None);
    }

    private void CaseloadSearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (CaseloadSearchBar.Text?.Length == 0)
            ViewModel.SearchCaseload();
    }
}
