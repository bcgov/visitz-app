using Visitz.Animations;
using Visitz.Animations.Haptic;
using Visitz.Models;
using Visitz.ViewModels;

namespace Visitz.Pages;

public partial class NoteEntryPage : VisitzPage
{
    public NoteEntryPage(NoteEntryViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    public static async Task Open(Page fromPage, CaseloadItem caseIncident, NoteItem noteItem)
    {
        await NavigateTo<NoteEntryPage>(fromPage, new Dictionary<string, object>
        {
            { NoteEntryViewModel.NoteItemKey, noteItem },
            { NoteEntryViewModel.CaseIncidentKey, caseIncident }
        });
    }

    void NotesEditor_TextChanged(object sender, TextChangedEventArgs e)
    {
        ((NoteEntryViewModel)BindingContext).EditorTextChanged(e);
    }

    public async Task ShowEditorError(string text)
    {
        await Task.WhenAll(ShowErrorText(text), AnimateEditorError());
    }

    private async Task ShowErrorText(string text)
    {
        if (ErrorLabel.IsVisible)
            return;

        ErrorLabel.Text = "❌ " + text;

        var fadeIn = new VisibilityAnimation(true, 100, Easing.CubicIn);
        var fadeOut = new VisibilityAnimation(false, 100, Easing.CubicOut);

        await fadeIn.Animate(ErrorLabel);

        await Task.Delay(2000);

        await fadeOut.Animate(ErrorLabel);
    }

    public async Task SetDraftSavedPromptVisible(bool visible)
    {
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            if (DraftSavedTagView.IsVisible == visible)
                return;

            var visibilityAnimation = new VisibilityAnimation(visible, 100, Easing.CubicInOut);
            await visibilityAnimation.Animate(DraftSavedTagView);
        });
    }

    public async Task SetSavingDraftPromptVisible(bool visible)
    {
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            if (SavingDraftTagView.IsVisible == visible)
                return;

            var visibilityAnimation = new VisibilityAnimation(visible, 100, Easing.CubicInOut);
            await visibilityAnimation.Animate(SavingDraftTagView);
        });
    }

    private async Task AnimateEditorError()
    {
        var vibrateErrorAnim = new ErrorVibrateAnimation();
        await vibrateErrorAnim.Animate(NotesEditor);
    }

    private void FocusBottom()
    {
        NotesEditor.CursorPosition = NotesEditor.Text?.Length ?? 0;
        NotesEditor.Focus();
    }

    void Scroll_To_Bottom_Clicked(object sender, EventArgs e)
    {
        FocusBottom();
    }
}