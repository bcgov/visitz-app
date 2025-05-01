using Visitz.Views.SplitView;

namespace Visitz.Views.Todo;

public partial class TodoContainerView : SplitLayoutView
{
    static IView StartView;

    public TodoContainerView()
    {
        InitializeComponent();
    }

    protected override void Creating()
    {
        base.Creating();

        StartPaneColumnWidth = GridLength.Auto;
        StartPane.MinimumWidthRequest = SplitLayoutDimensions.MinimumStartPaneWidth;

        StartView ??= ServiceProvider.GetService<TodoMasterList>();
        SetStartPane(StartView);
    }
}
