/*
	Workaround for https://github.com/dotnet/maui/issues/18933.
 */

using System.Collections;
using System.Windows.Input;
using Visitz.VisualStates;

namespace Visitz.Views.SelectionView;

public partial class SelectionList : BaseContentView
{
    private static readonly BindableProperty SelectedItemViewProperty =
        BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(SelectionList),
            defaultBindingMode: BindingMode.TwoWay, propertyChanged: SelectedItemViewChanged);

    public static readonly BindableProperty ItemsSourceProperty =
		BindableProperty.Create(nameof(ItemsSource), typeof(IList), typeof(SelectionList));

    public static readonly BindableProperty ItemTemplateProperty =
        BindableProperty.Create(nameof(ItemTemplate), typeof(DataTemplate), typeof(SelectionList));

    public static readonly BindableProperty SelectedItemProperty =
        BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(SelectionList),
            defaultBindingMode: BindingMode.TwoWay, propertyChanged: SelectedItemChanged);

    public static readonly BindableProperty SelectionChangedCommandProperty =
        BindableProperty.Create(nameof(SelectionChangedCommand), typeof(ICommand), typeof(SelectionList));

    public static readonly BindableProperty OrientationProperty =
        BindableProperty.Create(nameof(Orientation), typeof(StackOrientation), typeof(SelectionList));

    private ISelectedState SelectedItemView
    {
        get => (ISelectedState)GetValue(SelectedItemViewProperty);
        set => SetValue(SelectedItemViewProperty, value);
    }

    public IList ItemsSource 
	{
		get => (IList)GetValue(ItemsSourceProperty);
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

    public ICommand SelectionChangedCommand
    {
        get => (ICommand)GetValue(SelectionChangedCommandProperty);
        set => SetValue(SelectionChangedCommandProperty, value);
    }

    public StackOrientation Orientation
    {
        get => (StackOrientation)GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    public SelectionList()
	{
		InitializeComponent();
	}

    private static void SelectedItemViewChanged(BindableObject boundObj, object oldValue, object newValue)
    {
        if (oldValue is ISelectedState oldSelected)
            oldSelected.IsSelected = false;

        if (newValue is ISelectedState newSelected)
            newSelected.IsSelected = true;
    }

    private static void SelectedItemChanged(BindableObject boundObj, object oldValue, object newValue)
    {
        var thiz = (SelectionList)boundObj;

        if (newValue is not null)
        {
            thiz.SelectionChangedCommand?.Execute(newValue);
            thiz.SelectedItemView = thiz.GetSelectableViewByItem(newValue);
        }
    }

	private void Point_PointerReleased(object sender, PointerEventArgs e)
	{
		var selectableItem = (ISelectedState)sender;

		if (SelectedItemView?.Equals(selectableItem) ?? false && selectableItem.IsSelected)
			return;

		selectableItem.IsSelected = !selectableItem.IsSelected;

		if (selectableItem.IsSelected && selectableItem is BindableObject bindable)
			SelectedItem = bindable.BindingContext;
	}

	private void MainStack_ChildAdded(object sender, ElementEventArgs e)
    {
        if (e.Element is View view)
        {
			PointerGestureRecognizer point = new();
			point.PointerReleased += Point_PointerReleased;
			view.GestureRecognizers.Add(point);
		}
    }

	private void MainStack_ChildRemoved(object sender, ElementEventArgs e)
    {
        if (e.Element is View view)
            foreach (var g in view.GestureRecognizers)
                if (g is PointerGestureRecognizer tap)
                    tap.PointerReleased -= Point_PointerReleased;
    }

    private ISelectedState GetSelectableViewByItem(object item)
    {
        foreach (var child in MainStack.Children)
            if (child is ISelectedState selItem && child is View view && view.BindingContext == item)
                return selItem;

        return null;
    }
}
