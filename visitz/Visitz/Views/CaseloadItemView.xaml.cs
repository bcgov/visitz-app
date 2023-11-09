using Visitz.Extensions;
using Visitz.Models;

namespace Visitz.Views;

public partial class CaseloadItemView : ContentView
{
	public CaseloadItemView()
	{
		InitializeComponent();
	}

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();

        var item = (CaseloadItem)BindingContext;

        OpenDateLabel.IsVisible = item.DisplayDate?.Length > 0;

        if (item.CaseIncidentType?.Length > 2)
        {
            var initials = item.CaseIncidentType.GetInitials();

            if (initials.Length > 2)
                initials = initials[..2];

            SubtypeLabel.Text = initials;
        }
    }
}