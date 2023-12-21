#if !MACCATALYST
using CommunityToolkit.Maui.Core.Platform;
using CommunityToolkit.Mvvm.Messaging;
using Visitz.Resources.Localization;
using Visitz.Services;
#endif

using Visitz.ViewModels;
using Visitz.Views.SegmentedButtons;

namespace Visitz.Views.Caseload;

public partial class CaseloadView : ViewModelContentView, IRecipient<ServiceStateMessage>
{
    protected new CaseloadViewModel ViewModel => (CaseloadViewModel)base.ViewModel;

    public CaseloadView() : base(ServiceProvider.GetService<CaseloadViewModel>())
    {
        InitializeComponent();
        BindingContext = ViewModel;
    }

    protected override void Creating()
    {
        base.Creating();

        WeakReferenceMessenger.Default.Register(this, GetAllDataForOfflineService.MakeId());
    }

    protected override void Destroying()
    {
        WeakReferenceMessenger.Default.UnregisterAll(this);

        base.Destroying();
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
        ViewModel.SearchCaseload();
    }

    private void ListOptionsButton_Clicked(object sender, EventArgs e)
    {
        OptionsLayout.IsVisible = !OptionsLayout.IsVisible;
    }

    private void ClearFilterButton_Clicked(object sender, EventArgs e)
    {
        ViewModel.ActivatedFilterOption = SegmentedOptions.Empty;
    }

    public async void Receive(ServiceStateMessage message)
    {
        if (message.FinishedError)
            await Navigator.CurrentOpenPage.DisplayAlert(
                LocalizedStrings.Error,
                message.Message,
                LocalizedStrings.Ok);
    }
}
