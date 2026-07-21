using Visitz.Device;
using Visitz.Views.BaseClasses;

namespace Visitz.Views.Entity.ChildYouthVisits;

#nullable enable

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
            VisitsEditor.EditorView.IsFocused
            && DeviceDisplay.MainDisplayInfo.Orientation == DisplayOrientation.Landscape
            && isKeyboardOpen;

        ViewModel.ShowFullForm = !hideForm;
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
