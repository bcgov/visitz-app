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

    protected override void OnAppearing()
    {
        base.OnAppearing();

        RootGrid.KeyboardAppearanceEvent += Grid_KeyboardAppearanceEvent;
    }

    protected override void OnDisappearing()
    {
        RootGrid.KeyboardAppearanceEvent -= Grid_KeyboardAppearanceEvent;

        base.OnDisappearing();
    }

    void Grid_KeyboardAppearanceEvent(object sender, EventArgs e)
    {
        UpdateLayout(Width, Height);
    }

    public static async Task Open(Page fromPage, CaseloadItem caseIncident, NoteItem noteItem)
    {
        await NavigateTo<NoteEntryPage>(fromPage, new Dictionary<string, object>
        {
            { NoteEntryViewModel.NoteItemKey, noteItem },
            { NoteEntryViewModel.CaseIncidentKey, caseIncident }
        });
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        UpdateLayout(width, height);
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

            var visiblityAnimation = new VisibilityAnimation(visible, 100, Easing.CubicInOut);
            await visiblityAnimation.Animate(DraftSavedTagView);
        });
    }

    public async Task SetSavingDraftPromptVisible(bool visible)
    {
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            if (SavingDraftTagView.IsVisible == visible)
                return;

            var visiblityAnimation = new VisibilityAnimation(visible, 100, Easing.CubicInOut);
            await visiblityAnimation.Animate(SavingDraftTagView);
        });
    }

    private async Task AnimateEditorError()
    {
        var vibrateErrorAnim = new ErrorVibrateAnimation();
        await vibrateErrorAnim.Animate(NotesEditor);
    }

    private void UpdateLayout(double width, double height)
    {
        var resizableRowHeight = height;
        resizableRowHeight -= (
            TitleRow.Height.Value
            + DescriptionRow.Height.Value
            + RootGrid.RowSpacing
            + RootGrid.Padding.Top
            + RootGrid.Padding.Bottom
            + RootGrid.KeyboardHeight
        );
        if (resizableRowHeight > 0)
        {
            // This was done because of a Grid layout issue. (18 June 2023)
            // Issue: ScrollView inside a Grid's row breaks the Grid's layout and
            // goes past the device screen's visible area to a certain extent.
            // Fix: Setting the row height manually seems to prevent the scroll from going beyond the limits.
            EditorRow.Height = resizableRowHeight;
            EditorScroll.HeightRequest = resizableRowHeight;
        }
        FocusBottom();
    }

    private async void FocusBottom()
    {
        NotesEditor.Focus();
        NotesEditor.CursorPosition = NotesEditor.Text?.Length ?? 0;
        await Task.Delay(200);
        await EditorScroll.ScrollToAsync(NotesEditor, ScrollToPosition.End, true);
    }

    void Scroll_To_Bottom_Clicked(object sender, EventArgs e)
    {
        FocusBottom();
    }
}