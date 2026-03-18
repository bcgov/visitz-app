using System.Collections;
using System.Collections.Specialized;
using System.Windows.Input;
using Visitz.VisualStates;

namespace Visitz.Behaviors;

#nullable enable

internal partial class SelectionLayoutBehavior : Behavior<Layout>
{
    protected static readonly BindableProperty SelectedItemViewProperty = BindableProperty.Create(
        nameof(SelectedItemView),
        typeof(object),
        typeof(SelectionLayoutBehavior),
        defaultBindingMode: BindingMode.TwoWay,
        propertyChanged: (bound, old, @new) => SelectedItemView_Changed(old, @new)
    );

    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
        nameof(ItemsSource),
        typeof(IList),
        typeof(SelectionLayoutBehavior),
        propertyChanged: (bound, old, @new) =>
            (bound as SelectionLayoutBehavior)?.ItemsSource_Changed(old as IList, @new as IList)
    );

    public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(
        nameof(SelectedItem),
        typeof(object),
        typeof(SelectionLayoutBehavior),
        defaultBindingMode: BindingMode.TwoWay,
        propertyChanged: (bound, old, @new) => (bound as SelectionLayoutBehavior)?.SelectedItem_Changed(@new)
    );

    public static readonly BindableProperty SelectionChangedCommandProperty = BindableProperty.Create(
        nameof(SelectionChangedCommand),
        typeof(ICommand),
        typeof(SelectionLayoutBehavior)
    );

    public static readonly BindableProperty AutoSelectDefaultProperty = BindableProperty.Create(
        nameof(AutoSelectDefault),
        typeof(bool),
        typeof(SelectionLayoutBehavior)
    );

    public static readonly BindableProperty StickySelectionProperty = BindableProperty.Create(
        nameof(StickySelection),
        typeof(bool),
        typeof(SelectionLayoutBehavior)
    );

    protected ISelectedState? SelectedItemView
    {
        get => (ISelectedState)GetValue(SelectedItemViewProperty);
        set => SetValue(SelectedItemViewProperty, value);
    }

    public IList? ItemsSource
    {
        get => (IList?)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public object? SelectedItem
    {
        get => GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    public ICommand SelectionChangedCommand
    {
        get => (ICommand)GetValue(SelectionChangedCommandProperty);
        set => SetValue(SelectionChangedCommandProperty, value);
    }

    public bool AutoSelectDefault
    {
        get => (bool)GetValue(AutoSelectDefaultProperty);
        set => SetValue(AutoSelectDefaultProperty, value);
    }

    public bool StickySelection
    {
        get => (bool)GetValue(StickySelectionProperty);
        set => SetValue(StickySelectionProperty, value);
    }

    public Layout? Layout { get; private set; }

    protected override void OnAttachedTo(Layout layout)
    {
        base.OnAttachedTo(layout);

        Layout = layout;

        layout.SetBinding(
            BindableLayout.ItemsSourceProperty,
            static (SelectionLayoutBehavior slb) => slb.ItemsSource,
            source: this
        );

        layout.ChildAdded += Bindable_ChildAdded;
        layout.ChildRemoved += Bindable_ChildRemoved;
        layout.BindingContextChanged += Layout_BindingContextChanged;
    }

    protected override void OnDetachingFrom(Layout layout)
    {
        layout.RemoveBinding(BindableLayout.ItemsSourceProperty);

        layout.BindingContextChanged -= Layout_BindingContextChanged;
        layout.ChildRemoved -= Bindable_ChildRemoved;
        layout.ChildAdded -= Bindable_ChildAdded;

        base.OnDetachingFrom(layout);
    }

    void Layout_BindingContextChanged(object? sender, EventArgs e)
    {
        if (sender is Layout layout)
            BindingContext = layout.BindingContext;
    }

    void ItemsSource_Changed(IList? oldSource, IList? newSource)
    {
        if (newSource != null)
        {
            if (newSource is INotifyCollectionChanged newCollection)
                newCollection.CollectionChanged += ItemsSource_CollectionChanged;

            if (AutoSelectDefault && newSource.Count > 0 && SelectedItem == null)
                SelectedItem = newSource[0];
        }

        if (oldSource is INotifyCollectionChanged oldCollection)
            oldCollection.CollectionChanged -= ItemsSource_CollectionChanged;
    }

    void ItemsSource_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add && AutoSelectDefault)
            SelectedItem ??= e.NewItems?[0];
        else if (e.Action == NotifyCollectionChangedAction.Remove)
        {
            if (ItemsSource == null || ItemsSource.Count <= 0)
                SelectedItem = null;
            else if (ItemsSource.IndexOf(SelectedItem) == -1)
                SelectedItem = ItemsSource[0];
        }
        else if (e.Action == NotifyCollectionChangedAction.Reset)
            SelectedItem = null;
    }

    static void SelectedItemView_Changed(object oldValue, object newValue)
    {
        if (oldValue is ISelectedState oldSelected)
            oldSelected.IsSelected = false;

        if (newValue is ISelectedState newSelected)
            newSelected.IsSelected = true;
    }

    ISelectedState? GetSelectableViewByItem(object? item)
    {
        if (Layout == null || item == null)
            return null;

        foreach (var child in Layout.Children)
            if (child is ISelectedState selItem && child is View view && view.BindingContext == item)
                return selItem;

        return null;
    }

    void SelectedItem_Changed(object? newValue)
    {
        SelectedItemView = GetSelectableViewByItem(newValue);
        SelectionChangedCommand?.Execute(newValue);
    }

    void Bindable_ChildAdded(object? sender, ElementEventArgs e)
    {
        if (e.Element is View view)
        {
            PointerGestureRecognizer point = new();
            point.PointerReleased += Point_PointerReleased;
            view.GestureRecognizers.Add(point);

            if (view is ISelectedState selectable && view.BindingContext == SelectedItem)
                SelectedItemView = selectable;
        }
    }

    void Bindable_ChildRemoved(object? sender, ElementEventArgs e)
    {
        if (e.Element is View view)
            foreach (var g in view.GestureRecognizers)
                if (g is PointerGestureRecognizer tap)
                    tap.PointerReleased -= Point_PointerReleased;
    }

    void Point_PointerReleased(object? sender, PointerEventArgs e)
    {
        if (sender is not ISelectedState selectableItem || PreventDeselection(selectableItem))
            return;

        selectableItem.IsSelected = !selectableItem.IsSelected;

        if (selectableItem.IsSelected && selectableItem is BindableObject bindable)
            SelectedItem = bindable.BindingContext;
        else if (!selectableItem.IsSelected)
            SelectedItem = null;
    }

    bool PreventDeselection(ISelectedState selectableItem)
    {
        return StickySelection && (SelectedItemView?.Equals(selectableItem) ?? false) && selectableItem.IsSelected;
    }
}
