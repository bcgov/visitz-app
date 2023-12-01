using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using static Visitz.Views.Caseload.FilterPopupView;

namespace Visitz.Views.Caseload;

public partial class FilterPopup : Popup
{
	public static readonly double Width = 250;
    public static readonly double Height = 180;

    public FilterPopup(View anchor = null)
	{
		InitializeComponent();
		Anchor = anchor;
        Size = new Size(Width, Height);
        ResultWhenUserTapsOutsideOfPopup = null;

        FilterPopupView.SubtypeSelected += FilterPopupView_SubtypeSelected;
    }

    private void FilterPopupView_SubtypeSelected(object sender, SubtypeSelectedEventArgs e)
    {
        Close(e.Subtype);
    }
}