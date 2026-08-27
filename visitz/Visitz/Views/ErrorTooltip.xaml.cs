using Visitz.Animations;

namespace Visitz.Views;

public partial class ErrorTooltip : ContentView
{
    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text),
        typeof(string),
        typeof(ErrorTooltip),
        propertyChanged: (boundObj, _, _) => (boundObj as ErrorTooltip)?.UpdateUI()
    );

    public static readonly BindableProperty ShowProperty = BindableProperty.Create(
        nameof(Show),
        typeof(bool),
        typeof(ErrorTooltip),
        propertyChanged: (boundObj, _, newValue) =>
        {
            var fade = new VisibilityAnimation((bool)newValue, 100);
            _ = fade.Animate((VisualElement)boundObj);
        }
    );

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public bool Show
    {
        get => (bool)GetValue(ShowProperty);
        set => SetValue(ShowProperty, value);
    }

    public ErrorTooltip()
    {
        InitializeComponent();
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();

        UpdateUI();
    }

    private void UpdateUI()
    {
        ErrorLabel.IsVisible = Text?.Length > 0;
    }
}
