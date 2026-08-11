using Visitz.Animations.Haptic;
using Visitz.Storage;
using Visitz.Views.BaseClasses;
using Visitz.Views.Debugging;
using VisitzModel.Events;
using VisitzModel.Extensions;
using VisitzModel.Models.Notes;

namespace Visitz.Views.Entity.Notes;

public partial class NoteEntryView : IcmRecordContentView<NoteEntryViewModel>
{
    bool _disposed;

    public NoteEntryView()
        : base(ServiceProvider.GetService<NoteEntryViewModel>())
    {
        InitializeComponent();
        BindingContext = ViewModel;

        ViewModel.DraftError += NoteEntryView_DraftError;

        if (DebugOptions.Default.Enabled)
            AddDebugContextMenu();
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
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

    private void NotesEditor_Loaded(object? sender, EventArgs e)
    {
#if WINDOWS
        NotesEditor.Focus();

        if (!string.IsNullOrEmpty(NotesEditor.Text))
            NotesEditor.CursorPosition = NotesEditor.Text.Length;
#endif
    }

    void AddDebugContextMenu()
    {
        MenuFlyoutItem item = new() { Text = "Write without upload" };
        item.Clicked += async (s, e) =>
        {
            var dataRealm = await VisitzRealms.GetIcmDataRealmAsync();
            if (
                ViewModel != null
                && NoteItem.GetNotesByParent(dataRealm, BusinessObject.EntityType, BusinessObject.Id).LastOrDefault()
                    is NoteItem latest
            )
            {
                await dataRealm.CommitAsync(() => latest.Content += ViewModel.NoteDraft.Draft);
            }
        };

        MenuFlyout menu = [item];
        FlyoutBase.SetContextFlyout(this, menu);
    }
}
