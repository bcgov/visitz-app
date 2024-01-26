/*
	Workaround for https://github.com/dotnet/maui/issues/18933.
 */

using System.Collections;

namespace Visitz.Views.SelectionView;

public partial class VerticalSelectionView : BaseContentView
{
	public static readonly BindableProperty ItemsSourceProperty =
		BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(VerticalSelectionView));

    public static readonly BindableProperty ItemTemplateProperty =
        BindableProperty.Create(nameof(ItemTemplate), typeof(DataTemplate), typeof(VerticalSelectionView));

    public static readonly BindableProperty SelectedItemProperty =
        BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(VerticalSelectionView),
            defaultBindingMode: BindingMode.TwoWay);

    public IEnumerable ItemsSource 
	{
		get => (IEnumerable)GetValue(ItemsSourceProperty);
		set => SetValue(ItemsSourceProperty, value);
	}

	public DataTemplate ItemTemplate
    {
        get => (DataTemplate)GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }

	public object SelectedItem
    {
        get => GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    public VerticalSelectionView()
	{
		InitializeComponent();
	}
}