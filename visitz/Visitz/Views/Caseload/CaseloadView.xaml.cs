#if !MACCATALYST
using CommunityToolkit.Maui.Core.Platform;
#endif

using Visitz.ViewModels;

namespace Visitz.Views.Caseload;

public partial class CaseloadView : ViewModelContentView
{
    protected new CaseloadViewModel ViewModel => (CaseloadViewModel)base.ViewModel;

    public CaseloadView() : base(ServiceProvider.GetService<CaseloadViewModel>())
    {
        InitializeComponent();
        BindingContext = ViewModel;
    }

    protected override async void Creating()
    {
        base.Creating();

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

#if !MACCATALYST
        await CaseloadSearchBar.HideKeyboardAsync(CancellationToken.None);
#endif
    }

    private void CaseloadSearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (CaseloadSearchBar.Text?.Length == 0)
            ViewModel.SearchCaseload();
    }
}
