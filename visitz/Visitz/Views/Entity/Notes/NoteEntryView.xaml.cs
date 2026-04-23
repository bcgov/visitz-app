using Visitz.Animations.Haptic;
using Visitz.Resources.Localization;
using Visitz.Views.BaseClasses;
using Visitz.Views.Snackbar;
using VisitzModel.Events;
using VisitzModel.Interfaces;
using VisitzModel.Models.Caseload;

namespace Visitz.Views.Entity.Notes;

public partial class NoteEntryView : ViewModelContentView, IBusinessObjectHolder
{
    bool _disposed;

    new NoteEntryViewModel ViewModel => base.ViewModel as NoteEntryViewModel;

    public IBusinessObject BusinessObject
    {
        get => ViewModel.BusinessObject;
        set => ViewModel.BusinessObject = value;
    }

    public NoteEntryView()
        : base(ServiceProvider.GetService<NoteEntryViewModel>())
    {
        InitializeComponent();
        BindingContext = ViewModel;

        ViewModel.DraftError += NoteEntryView_DraftError;
        ViewModel.SaveStateHandler.SaveStateChanged += NoteEntryView_DraftSaveStateChanged;
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            ViewModel.SaveStateHandler.SaveStateChanged -= NoteEntryView_DraftSaveStateChanged;
            ViewModel.SaveStateHandler.Dispose();
            ViewModel.DraftError -= NoteEntryView_DraftError;

            _disposed = true;
        }

        base.Dispose(disposing);
    }

    private async void NoteEntryView_DraftError(object? sender, DraftErrorEventArgs e)
    {
        await ShowEditorError(e.ErrorMessage);
    }

    private async void NoteEntryView_DraftSaveStateChanged(object? sender, DraftSaveStatusEventArgs e)
    {
        await DraftSavedIndicator.SetState(e.State);
    }

    async void NotesEditor_TextChanged(object? sender, TextChangedEventArgs e)
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

    private async void Discard_Clicked(object? sender, EventArgs e)
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
        return await Navigator.CurrentOpenPage.DisplayAlertAsync(
            LocalizedStrings.DiscardDraftQuestion,
            LocalizedStrings.DiscardNoteDraftDescription,
            LocalizedStrings.Discard,
            LocalizedStrings.Cancel
        );
    }

    private void NotesEditor_Loaded(object? sender, EventArgs e)
    {
#if WINDOWS
        NotesEditor.Focus();

        if (!string.IsNullOrEmpty(NotesEditor.Text))
            NotesEditor.CursorPosition = NotesEditor.Text.Length;
#endif
    }
}
