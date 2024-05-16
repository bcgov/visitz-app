/*
	Workaround for https://github.com/dotnet/maui/issues/18933.
 */

using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;
using Visitz.VisualStates;

namespace Visitz.Views.SelectionView;

public partial class SelectionList : BaseContentView
{
    private static readonly BindableProperty SelectedItemViewProperty =
        BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(SelectionList),
            defaultBindingMode: BindingMode.TwoWay, propertyChanged: SelectedItemViewChanged);

    public static readonly BindableProperty ItemsSourceProperty =
		BindableProperty.Create(nameof(ItemsSource), typeof(IList), typeof(SelectionList),
			propertyChanged: ItemsSourceChanged);

    public static readonly BindableProperty ItemTemplateProperty =
        BindableProperty.Create(nameof(ItemTemplate), typeof(DataTemplate), typeof(SelectionList));

    public static readonly BindableProperty SelectedItemProperty =
        BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(SelectionList),
            defaultBindingMode: BindingMode.TwoWay, propertyChanged: SelectedItemChanged);

    public static readonly BindableProperty SelectionChangedCommandProperty =
        BindableProperty.Create(nameof(SelectionChangedCommand), typeof(ICommand), typeof(SelectionList));

    public static readonly BindableProperty OrientationProperty =
        BindableProperty.Create(nameof(Orientation), typeof(StackOrientation), typeof(SelectionList));

	public static readonly BindableProperty AutoSelectDefaultProperty =
		BindableProperty.Create(nameof(AutoSelectDefault), typeof(bool), typeof(SelectionList));

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

	public bool AutoSelectDefault
	{
		get => (bool)GetValue(AutoSelectDefaultProperty);
		set => SetValue(AutoSelectDefaultProperty, value);
	}

    public SelectionList()
	{
		InitializeComponent();
	}

	private static void ItemsSourceChanged(BindableObject boundObj, object oldValue, object newValue)
	{
		var thiz = (SelectionList)boundObj;
		var oldSource = (IList)oldValue;
		var newSource = (IList)newValue;

		if (thiz.AutoSelectDefault)
			HandleItemsSourceAutoUpdate(thiz, oldSource, newSource);
	}

	private static void HandleItemsSourceAutoUpdate(SelectionList selectionList, IList oldSource, IList newSource)
	{
		if (newSource != null)
		{
			if (newSource.Count > 0)
				selectionList.SelectedItem = newSource[0];

			if (newSource is INotifyCollectionChanged newCollection)
				newCollection.CollectionChanged += selectionList.ItemsSource_CollectionChanged;
		}

		if (oldSource is INotifyCollectionChanged oldCollection)
			oldCollection.CollectionChanged -= selectionList.ItemsSource_CollectionChanged;
	}

	private void ItemsSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
	{
		if (e.Action == NotifyCollectionChangedAction.Add)
			SelectedItem ??= e.NewItems[0];
		else if (e.Action == NotifyCollectionChangedAction.Remove)
		{
			if (ItemsSource.Count > 0 && ItemsSource.IndexOf(SelectedItem) == -1)
				SelectedItem = ItemsSource[0];
		}
		else if (e.Action == NotifyCollectionChangedAction.Reset)
			SelectedItem = null;
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
