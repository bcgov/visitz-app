using Visitz.Storage;
using Visitz.Views.BaseClasses;
using Visitz.Views.Debugging;
using VisitzModel.Extensions;

namespace Visitz.Views.Entity.ChildYouthVisits;

public partial class ChildYouthVisitListItemView : BaseContentView
{
    public ChildYouthVisitListItemView()
    {
        InitializeComponent();

        if (DebugOptions.Default.Enabled)
            AddDebugContextMenu();
    }

    void AddDebugContextMenu()
    {
        MenuFlyoutItem item = new() { Text = "Delete locally" };
        item.Clicked += async (s, e) =>
        {
            using var dataRealm = await VisitzRealms.GetIcmDataRealmAsync();
            if (BindingContext is ChildYouthVisitListItemViewModel viewModel)
                await dataRealm.CommitAsync(() => dataRealm.Remove(viewModel.Visit));
        };

        MenuFlyout menu = [item];
        FlyoutBase.SetContextFlyout(this, menu);
    }
}
