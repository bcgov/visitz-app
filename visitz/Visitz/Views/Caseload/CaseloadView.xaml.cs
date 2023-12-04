#if !MACCATALYST
using CommunityToolkit.Maui.Core.Platform;
#endif

using CommunityToolkit.Maui.Views;
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

    private async void ListOptionsButton_Clicked(object sender, EventArgs e)
    {
        // TODO: implement expandable section for list options (filter & sort)
    }
}
