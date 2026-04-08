using Visitz.Views.SplitView;

namespace Visitz.Views.Drafts;

#nullable enable

public partial class DraftsContainerView : SplitLayoutView
{
    readonly IView StartView;
    readonly IView EndView;

    public DraftsContainerView()
    {
        InitializeComponent();

        StartPaneColumnWidth = GridLength.Auto;
        StartPane.MinimumWidthRequest = SplitLayoutDimensions.MinimumStartPaneWidth;

        StartView ??= ServiceProvider.GetService<DraftsMasterList>();
        SetStartPane(StartView);

        EndView ??= ServiceProvider.GetService<DraftsList>();
        SetEndPane(EndView);
    }
}
