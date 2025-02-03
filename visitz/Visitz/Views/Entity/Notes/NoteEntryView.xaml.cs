using Visitz.Animations.Haptic;
using Visitz.Resources.Localization;
using Visitz.Views.BaseClasses;
using Visitz.Views.Snackbar;
using VisitzModel.Events;
using VisitzModel.Interfaces;
using VisitzModel.Models;

namespace Visitz.Views.Entity.Notes;

public partial class NoteEntryView : ViewModelContentView, ICaseloadItemHolder
{
	new NoteEntryViewModel ViewModel => base.ViewModel as NoteEntryViewModel;

    public CaseloadItem CaseloadItem
    {
        get => ViewModel.CaseloadItem;
        set => ViewModel.CaseloadItem = value;
    }

    public NoteEntryView() : base(ServiceProvider.GetService<NoteEntryViewModel>())
	{
		InitializeComponent();
		BindingContext = ViewModel;

        ViewModel.DraftError += NoteEntryView_DraftError;
        ViewModel.DraftSaveStateChanged += NoteEntryView_DraftSaveStateChanged;
	}

    protected override void Destroying()
    {
        ViewModel.DraftSaveStateChanged -= NoteEntryView_DraftSaveStateChanged;
        ViewModel.DraftError -= NoteEntryView_DraftError;

        base.Destroying();
    }

    private async void NoteEntryView_DraftError(object sender, DraftErrorEventArgs e)
    {
        await ShowEditorError(e.ErrorMessage);
    }

    private async void NoteEntryView_DraftSaveStateChanged(object sender, DraftSaveStatusEventArgs e)
    {
        await DraftSavedIndicator.SetState(e.State);
    }

    async void NotesEditor_TextChanged(object sender, TextChangedEventArgs e)
    {
        await ViewModel.EditorTextChanged(e);
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
        await vibrateErrorAnim.Animate(NotesEditor);
    }

	private async void Discard_Clicked(object sender, EventArgs e)
	{
		if (await PromptDiscard())
		{
			await ViewModel.ResetDraftAsync();
			await Navigator.Navigation.PopModalAsync();
			SnackbarHandler.ShowText(LocalizedStrings.DiscardNoteDraft);
		}
	}

	private static async Task<bool> PromptDiscard()
	{
		return await Navigator.CurrentOpenPage.DisplayAlert(
			LocalizedStrings.DiscardDraftQuestion,
			LocalizedStrings.DiscardNoteDraftDescription,
			LocalizedStrings.Discard,
			LocalizedStrings.Cancel);
	}
}
