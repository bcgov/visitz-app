using Visitz.Views.SplitView;

namespace Visitz.Views.Todo;

public partial class TodoContainerView : SplitLayoutView
{
    public TodoContainerView()
    {
        InitializeComponent();

        StartPaneColumnWidth = GridLength.Auto;
        StartPane.MinimumWidthRequest = SplitLayoutDimensions.MinimumStartPaneWidth;

        var StartView = ServiceProvider.GetService<TodoMasterList>();
        SetStartPane(StartView);

        var EndView = ServiceProvider.GetService<TodoVisitsView>();
        SetEndPane(EndView);
    }
}
