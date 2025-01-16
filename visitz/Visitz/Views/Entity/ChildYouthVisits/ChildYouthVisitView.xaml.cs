using Visitz.Views.BaseClasses;
using VisitzModel.Models;

namespace Visitz.Views.Entity.ChildYouthVisits;

public partial class ChildYouthVisitView : ViewModelContentView, ICaseloadItemHolder
{
    public CaseloadItem CaseloadItem
    {
        get => (ViewModel as ChildYouthVisitViewModel).CaseloadItem;
        set => (ViewModel as ChildYouthVisitViewModel).CaseloadItem = value;
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
        // if (await PromptDiscard())
        // {
        // 	await ViewModel.ResetDraftAsync();
        // 	await Navigator.Navigation.PopModalAsync();
        // 	SnackbarHandler.ShowText(LocalizedStrings.DiscardNoteDraft);
        // }
    }
}