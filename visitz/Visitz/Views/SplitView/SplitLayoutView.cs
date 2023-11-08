using Microsoft.Maui.Layouts;

namespace Visitz.Views.SplitView;

public abstract class SplitLayoutView : BaseContentView
{
    public static readonly BindableProperty StartPaneColumnWidthProperty = BindableProperty.Create(nameof(StartPaneColumnWidth),
        typeof(GridLength), typeof(SplitLayoutView), propertyChanged: StartPaneColumnWidthChanged);

    public static readonly BindableProperty EndPaneColumnWidthProperty = BindableProperty.Create(nameof(EndPaneColumnWidth),
        typeof(GridLength), typeof(SplitLayoutView), propertyChanged: EndPaneColumnWidthChanged);

    private static void StartPaneColumnWidthChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is SplitLayoutView splitView)
            splitView.StartColumn.Width = (GridLength)newValue;
    }

    private static void EndPaneColumnWidthChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is SplitLayoutView splitView)
            splitView.EndColumn.Width = (GridLength)newValue;
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

    private ColumnDefinition EndColumn { get; set; } = new ColumnDefinition();

    protected AbsoluteLayout StartPane { get; set; } = [];

    protected AbsoluteLayout EndPane { get; set; } = [];

    public SplitLayoutView()
	{
        Content = SplitLayout = new Grid
        {
            RowDefinitions = [new RowDefinition()],
            ColumnDefinitions = [StartColumn, EndColumn,],
        };

        SplitLayout.Add(StartPane, 0, 0);
        SplitLayout.Add(EndPane, 1, 0);
    }

    public void SetStartPane(IView view)
    {
        StartPane.Clear();
        AddToPane(StartPane, view);
    }

    public void SetEndPane(IView view)
    {
        EndPane.Clear();
        AddToPane(EndPane, view);
    }

    private static void AddToPane(AbsoluteLayout layout, IView view)
    {
        layout.Add(view);
        layout.SetLayoutBounds(view, new Rect(0, 0, 1, 1));
        layout.SetLayoutFlags(view, AbsoluteLayoutFlags.All);
    }
}