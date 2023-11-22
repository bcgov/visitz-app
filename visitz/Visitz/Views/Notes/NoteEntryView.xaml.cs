using Visitz.Animations;
using Visitz.Animations.Haptic;
using Visitz.Models;
using Visitz.ViewModels;

namespace Visitz.Views.Notes;

public partial class NoteEntryView : ViewModelContentView, ICaseloadItemHolder
{
    public CaseloadItem CaseloadItem
    {
        get => (ViewModel as ICaseloadItemHolder).CaseloadItem;
        set => (ViewModel as ICaseloadItemHolder).CaseloadItem = value;
    }

    public NoteEntryView() : base(ServiceProvider.GetService<NoteEntryViewModel>())
	{
		InitializeComponent();
		BindingContext = ViewModel;
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
        int end = NotesEditor.Text?.Length ?? 0;

        // Move the cursor an extra time to ensure the Editor viewport is always moved to the cursor.
        // (if cursor is already at 'end', setting it to 'end' again won't move the viewport)
        NotesEditor.CursorPosition = Math.Max(0, end - 1);

        NotesEditor.CursorPosition = end;
        NotesEditor.Focus();
    }

    void Scroll_To_Bottom_Clicked(object sender, EventArgs e)
    {
        FocusBottom();
    }
}