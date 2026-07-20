using Visitz.Animations.Haptic;
using Visitz.Device;
using Visitz.Resources.Localization;
using Visitz.Views.BaseClasses;

namespace Visitz.Views.Entity.ChildYouthVisits;

public partial class ChildYouthVisitView : IcmRecordContentView<ChildYouthVisitViewModel>
{
    private bool _disposed;
    private bool _isKeyboardOpen;

    private SoftKeyboardOpenHandler _keyboardOpenHandler;

    public ChildYouthVisitView()
        : base(ServiceProvider.GetService<ChildYouthVisitViewModel>())
    {
        InitializeComponent();
        BindingContext = ViewModel;

        _keyboardOpenHandler = new SoftKeyboardOpenHandler();
        _keyboardOpenHandler.KeyboardStateChanged += OnKeyboardStateChanged;
        DeviceDisplay.MainDisplayInfoChanged += OnMainDisplayInfoChanged;
    }

    private void OnKeyboardStateChanged(object? sender, KeyboardStateChangedEventArgs e)
    {
        _isKeyboardOpen = e.IsKeyboardOpen;
        CheckAndApplyOrientation(_isKeyboardOpen);
    }

    private void OnMainDisplayInfoChanged(object? sender, DisplayInfoChangedEventArgs e)
    {
        CheckAndApplyOrientation(_isKeyboardOpen);
    }

    private void CheckAndApplyOrientation(bool isKeyboardOpen)
    {
        bool hideForm =
            VisitsEditor.IsFocused
            && DeviceDisplay.MainDisplayInfo.Orientation == DisplayOrientation.Landscape
            && isKeyboardOpen;

        ViewModel.ShowFullForm = !hideForm;
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

    private async void VisitsEditor_EmojiEntered(object? sender, EventArgs e)
    {
        await ShowEditorError(LocalizedStrings.InvalidEntry);
    }

    private async void VisitsEditor_SuggestedMaxLengthExceeded(object? sender, EventArgs e)
    {
        await ShowEditorError(LocalizedStrings.CharacterLimitReached);
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            ViewModel.SaveStateHandler.Dispose();
            _keyboardOpenHandler.Dispose();
            DeviceDisplay.MainDisplayInfoChanged -= OnMainDisplayInfoChanged;

            _disposed = true;
        }

        base.Dispose(disposing);
    }
}
