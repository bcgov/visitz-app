using Microsoft.Maui.Layouts;
using Visitz.Resources.Styles;
using Visitz.Views.BaseClasses;

namespace Visitz.Views.SplitView;

public abstract class SplitLayoutView : BaseContentView
{
    private static readonly double Unset = -1.0d;

    public static readonly BindableProperty StartPaneColumnWidthProperty = BindableProperty.Create(
        nameof(StartPaneColumnWidth),
        typeof(GridLength),
        typeof(SplitLayoutView),
        propertyChanged: StartPaneColumnWidthChanged
    );

    public static readonly BindableProperty EndPaneColumnWidthProperty = BindableProperty.Create(
        nameof(EndPaneColumnWidth),
        typeof(GridLength),
        typeof(SplitLayoutView),
        propertyChanged: EndPaneColumnWidthChanged
    );

    private static void MatchWidths(VisualElement ve, ColumnDefinition column, GridLength gridLength)
    {
        ve.WidthRequest = gridLength.IsStar ? Unset : column.Width.Value;
    }

    private static void StartPaneColumnWidthChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is SplitLayoutView splitView && newValue is GridLength newLength)
        {
            splitView.StartColumn.Width = newLength;
            MatchWidths(splitView.StartPane, splitView.StartColumn, newLength);
        }
    }

    private static void EndPaneColumnWidthChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is SplitLayoutView splitView && newValue is GridLength newLength)
        {
            splitView.EndColumn.Width = newLength;
            MatchWidths(splitView.EndPane, splitView.EndColumn, newLength);
        }
    }

    public GridLength StartPaneColumnWidth
    {
        get { return (GridLength)GetValue(StartPaneColumnWidthProperty); }
        set { SetValue(StartPaneColumnWidthProperty, value); }
    }

    public GridLength EndPaneColumnWidth
    {
        get { return (GridLength)GetValue(EndPaneColumnWidthProperty); }
        set { SetValue(EndPaneColumnWidthProperty, value); }
    }

    private Grid SplitLayout { get; set; }

    private ColumnDefinition StartColumn { get; set; } = new ColumnDefinition();

    private ColumnDefinition SeparatorColumn { get; set; } = new ColumnDefinition(0.5);

    private ColumnDefinition EndColumn { get; set; } = new ColumnDefinition();

    protected AbsoluteLayout StartPane { get; set; } = [];

    protected AbsoluteLayout EndPane { get; set; } = [];

    public SplitLayoutView()
    {
        Content = SplitLayout = new Grid
        {
            RowDefinitions = [new RowDefinition()],
            ColumnDefinitions = [StartColumn, SeparatorColumn, EndColumn],
        };

        var separator = new BoxView() { Color = VisitzColors.SeparatorColor };

        SplitLayout.Add(StartPane, 0, 0);
        SplitLayout.Add(separator, 1, 0);
        SplitLayout.Add(EndPane, 2, 0);

        StartPane.IsClippedToBounds = true;
        EndPane.IsClippedToBounds = true;
    }

    public void SetStartPane(IView view)
    {
        foreach (var startView in StartPane.Children)
            if (startView is BaseContentView baseView)
                baseView.Dispose();

        StartPane.Clear();

        if (view != null)
            AddToPane(StartPane, view);
    }

    public void SetEndPane(IView view)
    {
        foreach (var endView in EndPane.Children)
            if (endView is BaseContentView baseView)
                baseView.Dispose();

        EndPane.Clear();

        if (view != null)
            AddToPane(EndPane, view);
    }

    private static void AddToPane(AbsoluteLayout layout, IView view)
    {
        layout.Add(view);
        layout.SetLayoutBounds(view, new Rect(0, 0, 1, 1));
        layout.SetLayoutFlags(view, AbsoluteLayoutFlags.All);
    }
}
