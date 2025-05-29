using VisitzModel.Models.Caseload;
using VisitzModel.Models.EntityTypes;

namespace Visitz.Views.Caseload;

public partial class CaseloadItemView : ContentView
{
    public static readonly BindableProperty DraftedItemsProperty = BindableProperty.Create(
        nameof(DraftedItems),
        typeof(HashSet<(string, EntityType)>),
        typeof(CaseloadItemView),
        propertyChanged: (boundObj, oldVal, newVal) =>
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

        if (BindingContext is IBusinessObject bobj && bobj.IsValid)
            ApplyBusinessObject(bobj);
    }

    private void ApplyBusinessObject(IBusinessObject businessObject)
    {
        OpenDateLabel.IsVisible = businessObject.DisplayDate?.Length > 0;

        if (DraftedItems != null)
        {
            // TODO Remove v1Tuple when fully using Row IDs
            var v1Tuple = (businessObject.FileNumber, businessObject.EntityType);
            var v2Tuple = (businessObject.Id, businessObject.EntityType);

            DraftIndicator.IsVisible = DraftedItems.Contains(v1Tuple) || DraftedItems.Contains(v2Tuple);
        }
    }
}
