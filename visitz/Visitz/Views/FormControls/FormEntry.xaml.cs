namespace Visitz.Views.FormControls;

using CommunityToolkit.Maui;
using Visitz.Animations;
using Visitz.Animations.Haptic;
using Visitz.Resources.Localization;
using VisitzModel.Extensions;

public partial class FormEntry : ContentView
{
    [BindableProperty]
    public partial string FieldName { get; set; }

    [BindableProperty(
        DefaultBindingMode = BindingMode.TwoWay,
        PropertyChangedMethodName = nameof(TextProperty_Changed)
    )]
    public partial string Text { get; set; }

    [BindableProperty]
    public partial string LeadingSupportingText { get; set; }

    [BindableProperty]
    public partial string TrailingSupportingText { get; set; }

    [BindableProperty(PropertyChangedMethodName = nameof(FieldNameIsVisibleProperty_Changed))]
    public partial bool FieldNameIsVisible { get; set; } = true;

    [BindableProperty]
    public partial string Placeholder { get; set; }

    [BindableProperty(PropertyChangedMethodName = nameof(MaxLengthProperty_Changed))]
    public partial int MaxLength { get; set; } = int.MaxValue;

    [BindableProperty]
    public partial bool IsReadOnly { get; set; }

    public Editor EditorView => Editor;

    public FormEntry()
    {
        InitializeComponent();
    }

    static void TextProperty_Changed(BindableObject obj, object _, object __) =>
        ((FormEntry)obj).UpdateCharacterCount();

    static void FieldNameIsVisibleProperty_Changed(BindableObject obj, object _, object newValue)
    {
        var formEntry = (FormEntry)obj;
        var isVisible = (bool)newValue;

        formEntry.FieldNameRow.Height = isVisible ? GridLength.Star : 0.0;
    }

    static void MaxLengthProperty_Changed(BindableObject obj, object _, object newValue)
    {
        var formEntry = (FormEntry)obj;
        int newLength = (int)newValue;

        formEntry.UpdateBottomRowVisibility(newLength);
        formEntry.UpdateCharacterCount();
    }

    private void UpdateBottomRowVisibility(int maxLength)
    {
        SubRow.Height = maxLength == int.MaxValue ? 0.0d : GridLength.Auto;
    }

    private void UpdateCharacterCount()
    {
        TrailingSupportingText = $"{Text?.Length ?? 0}/{MaxLength}";
    }

    private void Editor_TextChanged(object? sender, TextChangedEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(sender, nameof(sender));

        if (ContainEmojis(e))
        {
            CancelTextChangedEvent((Editor)sender, e);
            var ErrorMessage = LocalizedStrings.InvalidEntry;
            _ = ShowEditorError(ErrorMessage);
            return;
        }
    }

    private static bool ContainEmojis(TextChangedEventArgs e)
    {
        return e.NewTextValue?.ContainsUnicodeSurrogatesAndOtherSymbols() ?? false;
    }

    private static void CancelTextChangedEvent(Editor editor, TextChangedEventArgs e)
    {
        editor.Text = e.OldTextValue;
    }

    public async Task ShowEditorError(string text)
    {
        await Task.WhenAll(ShowErrorText(text), AnimateEditorError());
    }

    private async Task ShowErrorText(string text)
    {
        if (EditorError.IsVisible)
            return;

        var showAnimation = new VisibilityAnimation(true, 300);
        LeadingSupportingText = text;
        await Task.WhenAll(showAnimation.Animate(EditorError), showAnimation.Animate(LeadingSupportingLabel));

        await Task.Delay(2500);

        var hideAnimation = new VisibilityAnimation(false, 300);
        await Task.WhenAll(hideAnimation.Animate(EditorError), hideAnimation.Animate(LeadingSupportingLabel));
    }

    private async Task AnimateEditorError()
    {
        var vibrateErrorAnim = new ErrorVibrateAnimation();
        await vibrateErrorAnim.Animate(Editor);
    }
}
