/*
	Workaround for https://github.com/dotnet/maui/issues/18933.
 */

using System.Collections;

namespace Visitz.Views.SelectionView;

public partial class VerticalSelectionView : BaseContentView
{
	public static readonly BindableProperty ItemsSourceProperty =
		BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(VerticalSelectionView));

    public IEnumerable ItemsSource 
	{
		get => (IEnumerable)GetValue(ItemsSourceProperty);
		set => SetValue(ItemsSourceProperty, value);
	}

	public VerticalSelectionView()
	{
		InitializeComponent();
	}
}