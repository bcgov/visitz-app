using CommunityToolkit.Maui;
using Visitz.Resources.Styles;

namespace Visitz.Views.FormControls;

public partial class InfoItemView : ContentView
{
    [BindableProperty(PropertyChangedMethodName = nameof(IconGlyph_PropertyChanged))]
    public partial string IconGlyph { get; set; } = string.Empty;

    [BindableProperty]
    public partial string IconFontFamily { get; set; } = string.Empty;

    [BindableProperty]
    public partial string Label { get; set; } = string.Empty;

    [BindableProperty]
    public partial string Value { get; set; } = string.Empty;

    [BindableProperty]
    public partial Color? ValueColor { get; set; }

    [BindableProperty]
    public partial TextDecorations ValueTextDecorations { get; set; } = TextDecorations.None;

    [BindableProperty]
    public partial Action? TapAction { get; set; }

    [BindableProperty]
    public partial GridLength IconColumnWidth { get; set; } = new GridLength(0.0d);

    [BindableProperty]
    public partial double ColumnSpacing { get; set; }

    public InfoItemView()
    {
        InitializeComponent();
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();

        if (BindingContext is InfoItem item)
        {
            // We've decoupled InfoItem from this view but for backward compatibility/convenience
            // we can set up bindings for it in code here instead of each referenced place
            this.SetBinding(IconGlyphProperty, static (InfoItem source) => source.IconGlyph, source: item);
            this.SetBinding(IconFontFamilyProperty, static (InfoItem source) => source.IconFontFamily, source: item);
            this.SetBinding(LabelProperty, static (InfoItem source) => source.Label, source: item);
            this.SetBinding(ValueProperty, static (InfoItem source) => source.Value, source: item);
            this.SetBinding(ValueColorProperty, static (InfoItem source) => source.ValueColor, source: item);
            this.SetBinding(
                ValueTextDecorationsProperty,
                static (InfoItem source) => source.ValueTextDecorations,
                source: item
            );
            this.SetBinding(TapActionProperty, static (InfoItem source) => source.TapAction, source: item);
        }
    }

    void TapGestureRecognizer_Tapped(object? sender, TappedEventArgs e)
    {
        TapAction?.Invoke();
    }

    static void IconGlyph_PropertyChanged(BindableObject bindable, object _, object newValue)
    {
        if (bindable is not InfoItemView itemView)
            return;

        if (newValue is string { Length: > 0 })
        {
            itemView.IconColumnWidth = GridLength.Auto;
            itemView.ColumnSpacing = VisitzDimensions.DefaultSpacing;
        }
        else
        {
            itemView.IconColumnWidth = new GridLength(0.0d);
            itemView.ColumnSpacing = 0.0d;
        }
    }
}
