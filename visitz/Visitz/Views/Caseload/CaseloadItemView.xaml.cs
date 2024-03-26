using VisitzModel.Models;

namespace Visitz.Views.Caseload;

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
		}
    }
}
