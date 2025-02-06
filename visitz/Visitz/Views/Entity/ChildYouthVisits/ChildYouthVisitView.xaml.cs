using Visitz.Animations.Haptic;
using Visitz.Device;
using Visitz.Resources.Localization;
using Visitz.Views.BaseClasses;
using Visitz.Views.Snackbar;
using VisitzModel.Events;
using VisitzModel.Interfaces;
using VisitzModel.Models;

namespace Visitz.Views.Entity.ChildYouthVisits;

public partial class ChildYouthVisitView : ViewModelContentView, ICaseloadItemHolder
{
    private bool _disposed;

    private SoftKeyboardOpenHandler _keyboardOpenHandler;

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
        ViewModel.SaveStateHandler.SaveStateChanged += ViewModel_DraftSaveStateChanged;

        _keyboardOpenHandler = new SoftKeyboardOpenHandler();
        _keyboardOpenHandler.KeyboardStateChanged += OnKeyboardStateChanged;
    }

    private void OnKeyboardStateChanged(object sender, KeyboardStateChangedEventArgs e)
    {
        ViewModel.ResizeForLandscapeKeyboard = !e.IsKeyboardOpen;
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            ViewModel.SaveStateHandler.SaveStateChanged -= ViewModel_DraftSaveStateChanged;
            ViewModel.SaveStateHandler.Dispose();
            _keyboardOpenHandler.Dispose();

            _disposed = true;
        }

        base.Dispose(disposing);
    }

    private async void ViewModel_DraftSaveStateChanged(object sender, DraftSaveStatusEventArgs e)
    {
        await DraftSavedIndicator.SetState(e.State);
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
            ViewModel.DiscardDraft();
            await Navigator.Navigation.PopModalAsync();
            SnackbarHandler.ShowText(LocalizedStrings.DiscardedVisitDraft);
        }
    }

    private static async Task<bool> PromptDiscard()
    {
        return await Navigator.CurrentOpenPage.DisplayAlert(
            LocalizedStrings.DiscardDraftQuestion,
            LocalizedStrings.DiscardVisitDraftDescription,
            LocalizedStrings.Discard,
            LocalizedStrings.Cancel);
    }

    private async void VisitsEditor_EmojiEntered(object sender, EventArgs e)
    {
        await ShowEditorError(LocalizedStrings.InvalidEntry);
    }

    private async void VisitsEditor_SuggestedMaxLengthExceeded(object sender, EventArgs e)
    {
        await ShowEditorError(LocalizedStrings.CharacterLimitReached);
    }
}
