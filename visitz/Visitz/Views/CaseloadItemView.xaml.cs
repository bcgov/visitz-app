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
    }
}