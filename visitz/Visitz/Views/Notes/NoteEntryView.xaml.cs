using Visitz.Animations.Haptic;
using Visitz.ViewModels;
using VisitzModel.Events;
using VisitzModel.Models;

namespace Visitz.Views.Notes;

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
        DraftSavedView.State state = e.DraftSaved
            ? DraftSavedView.State.Saved
            : DraftSavedView.State.Saving;

        await DraftSavedIndicator.SetState(state);
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
}
