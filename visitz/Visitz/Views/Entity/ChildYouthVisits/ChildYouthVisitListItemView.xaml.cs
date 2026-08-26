using Microsoft.Extensions.Logging;
using Visitz.Views.BaseClasses;
using VisitzModel.Extensions;

namespace Visitz.Views.Entity.ChildYouthVisits;

public partial class ChildYouthVisitListItemView : BaseContentView
{
    public ChildYouthVisitListItemView()
    {
        InitializeComponent();
    }

    async void TapGestureRecognizer_Tapped(object? sender, TappedEventArgs e)
    {
        try
        {
            if (BindingContext is ChildYouthVisitListItemViewModel vm && Parent is CollectionView cv)
            {
                vm.ItemTapped();
                await ScrollTo(cv, vm);
            }
        }
        catch (Exception ex)
        {
            Logger.LogException(ex);
        }
    }

    static async Task ScrollTo(CollectionView cv, ChildYouthVisitListItemViewModel vm)
    {
        await Task.Delay(10); // not a fan but it's the easiest way to let the layout settle
        cv.ScrollTo(vm, position: ScrollToPosition.Start);
    }
}
