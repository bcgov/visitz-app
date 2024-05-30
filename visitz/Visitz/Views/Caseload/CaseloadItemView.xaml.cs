using VisitzModel.Extensions.EntityTypes;
using VisitzModel.Models;
using VisitzModel.Models.EntityTypes;

namespace Visitz.Views.Caseload;

public partial class CaseloadItemView : ContentView
{
	public static readonly BindableProperty DraftedItemsProperty = BindableProperty.Create(
		nameof(DraftedItems), typeof(HashSet<(string, EntityType)>), typeof(CaseloadItemView), propertyChanged: (boundObj, oldVal, newVal) =>
		{
			(boundObj as CaseloadItemView).OnBindingContextChanged();
		});

	public HashSet<(string, EntityType)> DraftedItems
	{
		get => (HashSet<(string, EntityType)>)GetValue(DraftedItemsProperty);
		set => SetValue(DraftedItemsProperty, value);
	}

	public CaseloadItemView()
	{
		InitializeComponent();
	}

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();

        if (BindingContext is CaseloadItem item)
			ApplyCaseloadItem(item);
    }

	private void ApplyCaseloadItem(CaseloadItem item)
	{
		OpenDateLabel.IsVisible = item.DisplayDate?.Length > 0;

		if (DraftedItems != null)
		{
			var tuple = (item.CaseIncidentNumber, item.EntityType.ParseEntityType());
			DraftIndicator.IsVisible = DraftedItems.Contains(tuple);
		}
	}
}
