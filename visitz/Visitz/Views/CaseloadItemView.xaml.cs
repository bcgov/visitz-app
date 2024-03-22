using Visitz.Resources.Styles;
using VisitzModel.Models;

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

        if (BindingContext is CaseloadItem item)
		{
			OpenDateLabel.IsVisible = item.DisplayDate?.Length > 0;

			UpdateTagViewStyles(item);
		}
    }

    private void UpdateTagViewStyles(CaseloadItem item)
    {
        TagView.StrokeThickness = 0;

        if (item.EntityType == IcmEntity.Case)
        {
            TagView.BackgroundColor = VisitzColors.EntityCaseTagBackground;
            TagView.TextColor = VisitzColors.EntityCaseTagText;
        }
        else if (item.EntityType == IcmEntity.Incident)
        {
            TagView.BackgroundColor = VisitzColors.EntityIncidentTagBackground;
            TagView.TextColor = VisitzColors.EntityIncidentTagText;
        }
        else if (item.EntityType == IcmEntity.Memo)
        {
            TagView.BackgroundColor = VisitzColors.EntityMemoTagBackground;
            TagView.TextColor = VisitzColors.EntityMemoTagText;
        }
        else if (item.EntityType == IcmEntity.ServiceRequest)
        {
            TagView.BackgroundColor = VisitzColors.EntityServiceRequestTagBackground;
            TagView.TextColor = VisitzColors.EntityServiceRequestTagText;
        }
        else
        {
            TagView.BackgroundColor = Colors.Transparent;
            TagView.TextColor = VisitzColors.BC_TextColor;
        }
    }
}
