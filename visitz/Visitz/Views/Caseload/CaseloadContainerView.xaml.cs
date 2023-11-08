using Visitz.Resources.Styles;
using Visitz.Views.SplitView;

namespace Visitz.Views.Caseload;

public partial class CaseloadContainerView : SplitLayoutView
{
	public CaseloadContainerView()
    {
		InitializeComponent();
    }

    protected override void Creating()
    {
        base.Creating();

        StartPaneColumnWidth = new GridLength(0.5, GridUnitType.Star);

        SetStartPane(ServiceProvider.GetService<CaseloadView>());

        SetEndPane(new BoxView()
        {
            Color = VisitzColors.DarkSkyBlueBackground,
        });
    }
}