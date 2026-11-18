using System.Collections;
using CommunityToolkit.Maui;

namespace Visitz.Views.FormControls;

public partial class FormPicker : ContentView
{
    [BindableProperty]
    public partial string? FieldName { get; set; }

    [BindableProperty]
    public partial IList? ItemsSource { get; set; }

    [BindableProperty]
    public partial object? SelectedItem { get; set; }

    public BindingBase? ItemDisplayBinding
    {
        get => PickerView.ItemDisplayBinding;
        set => PickerView.ItemDisplayBinding = value;
    }

    public FormPicker()
    {
        InitializeComponent();

        FieldLabel.SetBinding(Label.TextProperty, static (FormPicker fp) => fp.FieldName, source: this);
        PickerView.SetBinding(Picker.ItemsSourceProperty, static (FormPicker fp) => fp.ItemsSource, source: this);
        PickerView.SetBinding(Picker.SelectedItemProperty, static (FormPicker fp) => fp.SelectedItem, source: this);
    }
}
