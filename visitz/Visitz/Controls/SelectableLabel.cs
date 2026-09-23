using CommunityToolkit.Maui;
#if IOS
using UIKit;
#elif WINDOWS
using Microsoft.UI.Xaml.Controls;
#endif

namespace Visitz.Controls;

public partial class SelectableLabel : Label
{
    [BindableProperty(PropertyChangedMethodName = nameof(OnIsSelectionEnabled_PropertyChanged))]
    public partial bool IsSelectionEnabled { get; set; } = true;

    static void OnIsSelectionEnabled_PropertyChanged(BindableObject bindable, object _, object newValue)
    {
        if (bindable is SelectableLabel label)
            label.UpdateTextSelection(isEnabled: newValue is bool enabled && enabled);
    }

    void UpdateTextSelection(bool isEnabled)
    {
#if IOS
        if (Handler?.PlatformView is UITextView uiTextView)
            uiTextView.Selectable = isEnabled;
#elif WINDOWS
        if (Handler?.PlatformView is TextBlock textBlock)
            textBlock.IsTextSelectionEnabled = isEnabled;
#else
        throw new NotImplementedException("Not implemented for current platform");
#endif
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();

        // Initializing the IsSelectionEnabled property is not enough to trigger its
        // PropertyChanged method, so we'll make sure to set the text selection property
        // here once a handler might be available.
        UpdateTextSelection(IsSelectionEnabled);
    }
}
