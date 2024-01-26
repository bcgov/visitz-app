/*
	Workaround for https://github.com/dotnet/maui/issues/18933.
 */

using System.Collections;
using System.Windows.Input;
using Visitz.Models;

namespace Visitz.Views.SelectionView;

public partial class VerticalSelectionView : BaseContentView
{
    private static readonly BindableProperty SelectedItemViewProperty =
        BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(VerticalSelectionView),
            defaultBindingMode: BindingMode.TwoWay, propertyChanged: SelectedItemViewChanged);

    public static readonly BindableProperty ItemsSourceProperty =
		BindableProperty.Create(nameof(ItemsSource), typeof(IList), typeof(VerticalSelectionView));

    public static readonly BindableProperty ItemTemplateProperty =
        BindableProperty.Create(nameof(ItemTemplate), typeof(DataTemplate), typeof(VerticalSelectionView));

    public static readonly BindableProperty SelectedItemProperty =
        BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(VerticalSelectionView),
            defaultBindingMode: BindingMode.TwoWay, propertyChanged: SelectedItemChanged);

    public static readonly BindableProperty SelectionChangedCommandProperty =
        BindableProperty.Create(nameof(SelectionChangedCommand), typeof(ICommand), typeof(VerticalSelectionView));

    private readonly TapGestureRecognizer ItemTapRecognizer = new() { Buttons = ButtonsMask.Primary, };

    private SelectableItem SelectedItemView
    {
        get => (SelectableItem)GetValue(SelectedItemViewProperty);
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

    public VerticalSelectionView()
	{
		InitializeComponent();
        ItemTapRecognizer.Tapped += ItemTapRecognizer_Tapped;
	}

    private static void SelectedItemViewChanged(BindableObject boundObj, object oldValue, object newValue)
    {
        if (oldValue is SelectableItem oldSelected)
            oldSelected.IsSelected = false;

        if (newValue is SelectableItem newSelected)
            newSelected.IsSelected = true;
    }

    private static void SelectedItemChanged(BindableObject boundObj, object oldValue, object newValue)
    {
        var thiz = (VerticalSelectionView)boundObj;

        if (newValue is NavItem)
        {
            thiz.SelectionChangedCommand?.Execute(newValue);
            thiz.SelectedItemView = thiz.GetSelectableViewByItem(newValue);
        }
    }

    private void ItemTapRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        var selectableItem = (SelectableItem)sender;
        selectableItem.IsSelected = !selectableItem.IsSelected;

        if (selectableItem.IsSelected)
            SelectedItem = selectableItem.BindingContext;
    }

    private void MainStack_ChildAdded(object sender, ElementEventArgs e)
    {
        if (e.Element is SelectableItem item)
            item.GestureRecognizers.Add(ItemTapRecognizer);
    }

    private void MainStack_ChildRemoved(object sender, ElementEventArgs e)
    {
        if (e.Element is SelectableItem item)
            item.GestureRecognizers.Remove(ItemTapRecognizer);
    }

    private SelectableItem GetSelectableViewByItem(object item)
    {
        foreach (var child in MainStack.Children)
            if (child is SelectableItem selItem && selItem.BindingContext == item)
                return selItem;

        return null;
    }
}