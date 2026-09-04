using Microsoft.Extensions.Logging;
using Visitz.Views.BaseClasses;
using VisitzModel.Extensions;

namespace Visitz.Views.Entity.SupportNetwork;

public partial class SupportNetworkListItem : BaseContentView
{
    public SupportNetworkListItem()
    {
        InitializeComponent();
    }

    private async void TapGestureRecognizer_Tapped(object? sender, TappedEventArgs e)
    {
        try
        {
            if (
                BindingContext is SupportNetworkItemUi itemVm
                && sender is SupportNetworkListItem item
                && item.Parent is CollectionView cv
            )
            {
                itemVm.ToggleExpanded();
                await ScrollTo(cv, itemVm);
            }
        }
        catch (Exception ex)
        {
            Logger.LogException(ex);
        }
    }

    static async Task ScrollTo(CollectionView cv, SupportNetworkItemUi vm)
    {
        await Task.Delay(10); // not a fan but it's the easiest way to let the layout settle
        cv.ScrollTo(vm, position: ScrollToPosition.Start);
    }
}
