using Visitz.Resources.Localization;
using Visitz.Views.BaseClasses;
using Visitz.Views.Snackbar;
using VisitzModel.Models;

namespace Visitz.Views.Entity.ChildYouthVisits;

public partial class ChildYouthVisitView : ViewModelContentView, ICaseloadItemHolder
{
    public new ChildYouthVisitViewModel ViewModel => base.ViewModel as ChildYouthVisitViewModel;
    public CaseloadItem CaseloadItem
    {
        get => ViewModel.CaseloadItem;
        set => ViewModel.CaseloadItem = value;
    }

    public ChildYouthVisitView() : base(ServiceProvider.GetService<ChildYouthVisitViewModel>())
    {
        InitializeComponent();
        BindingContext = ViewModel;
    }

    public ChildYouthVisitView(VisitzViewModel viewModel) : base(viewModel)
    {
    }

    private async void Discard_Clicked(object sender, EventArgs e)
    {
        if (await PromptDiscard())
        {
        	// await ViewModel.ResetDraftAsync();
        	await Navigator.Navigation.PopModalAsync();
        	SnackbarHandler.ShowText(LocalizedStrings.DiscardNoteDraft);
        }
    }

    async void VisitsEditor_TextChanged(object sender, TextChangedEventArgs e)
    {
        await ViewModel.EditorTextChanged(e);
    }

    private static async Task<bool> PromptDiscard()
	{
		return await Navigator.CurrentOpenPage.DisplayAlert(
			LocalizedStrings.DiscardDraftQuestion,
			LocalizedStrings.DiscardVisitDraftDescription,
			LocalizedStrings.Discard,
			LocalizedStrings.Cancel);
	}
}