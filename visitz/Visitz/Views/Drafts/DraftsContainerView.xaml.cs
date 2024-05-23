using Visitz.Views.SplitView;

namespace Visitz.Views.Drafts;

public partial class DraftsContainerView : SplitLayoutView
{
	static IView StartView;
	static IView EndView;

	public DraftsContainerView()
	{
		InitializeComponent();
	}

	protected override void Creating()
	{
		base.Creating();

		StartPaneColumnWidth = GridLength.Auto;
		StartPane.MinimumWidthRequest = SplitLayoutDimensions.MinimumStartPaneWidth;

		StartView ??= ServiceProvider.GetService<DraftsMasterList>();
		SetStartPane(StartView);

		EndView ??= ServiceProvider.GetService<DraftsList>();
		SetEndPane(EndView);
	}
}
