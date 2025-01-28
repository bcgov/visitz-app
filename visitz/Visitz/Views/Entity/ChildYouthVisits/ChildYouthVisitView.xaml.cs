using Visitz.Animations.Haptic;
using Visitz.Resources.Localization;
using Visitz.Views.BaseClasses;
using Visitz.Views.Snackbar;
using VisitzModel.Events;
using VisitzModel.Interfaces;
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
        ViewModel.DraftError += AddVisit_DraftError;
    }
    private async void AddVisit_DraftError(object sender, DraftErrorEventArgs e)
    {
        await ShowEditorError(e.ErrorMessage);
    }

    public async Task ShowEditorError(string text)
    {
        await Task.WhenAll(ShowErrorText(text), AnimateEditorError());
    }

    private async Task ShowErrorText(string text)
    {
        if (EditorError.IsVisible)
            return;

        EditorError.Text = text;
        EditorError.Show = true;

        await Task.Delay(2000);

        EditorError.Show = false;
    }

    private async Task AnimateEditorError()
    {
        var vibrateErrorAnim = new ErrorVibrateAnimation();
        await vibrateErrorAnim.Animate(VisitsEditor);
    }

    private async void Discard_Clicked(object sender, EventArgs e)
    {
        if (await PromptDiscard())
        {
            await Navigator.Navigation.PopModalAsync();
            SnackbarHandler.ShowText(LocalizedStrings.DiscardedVisitDraft);
        }
    }

    void VisitsEditor_TextChanged(object sender, TextChangedEventArgs e)
    {
        ViewModel.EditorTextChanged(e);
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