using CommunityToolkit.Maui;

namespace Visitz.Views.FormControls;

public partial class FormDatePicker : ContentView
{
    [BindableProperty]
    public partial DateTimeOffset? Date { get; set; }

    [BindableProperty]
    public partial string? FieldName { get; set; }

    public DatePicker DatePicker => DatePickerView;

    public FormDatePicker()
    {
        InitializeComponent();
    }
}
