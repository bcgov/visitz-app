using CommunityToolkit.Maui;
using Visitz.Converters;

namespace Visitz.Views.FormControls;

public partial class FormDatePicker : ContentView
{
    [BindableProperty]
    public partial DateTimeOffset? Date { get; set; }

    [BindableProperty]
    public partial string? FieldName { get; set; }

    public FormDatePicker()
    {
        InitializeComponent();

        FieldNameLabel.SetBinding(Label.TextProperty, static (FormDatePicker fdp) => fdp.FieldName, source: this);

        DatePickerView.SetBinding(
            DatePicker.DateProperty,
            static (FormDatePicker fdp) => fdp.Date,
            mode: BindingMode.TwoWay,
            converter: new DateTimeOffsetNullableConverter(),
            source: this
        );
    }
}
