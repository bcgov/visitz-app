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

    [BindableProperty]
    public partial BindingBase? ItemDisplayBinding { get; set; }

    public FormPicker()
    {
        InitializeComponent();
    }
}
