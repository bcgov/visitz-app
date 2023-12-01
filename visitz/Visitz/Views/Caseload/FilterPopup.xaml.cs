using CommunityToolkit.Maui.Views;

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
    }
}