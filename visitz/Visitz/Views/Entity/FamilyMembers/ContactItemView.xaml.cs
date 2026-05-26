using Microsoft.Extensions.Logging;
using Visitz.Views.BaseClasses;

namespace Visitz.Views.Entity.FamilyMembers;

#nullable enable

public partial class ContactItemView : BaseContentView
{
    public ContactItemView()
    {
        InitializeComponent();
    }

    async void TapGestureRecognizer_Tapped(object? sender, TappedEventArgs e)
    {
        try
        {
            if (
                BindingContext is ContactItemViewModel vm
                && sender is ContactItemView item
                && item.Parent is CollectionView cv
            )
            {
                vm.ItemTapped();
                await ScrollTo(cv, vm);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex.Message, ex);
        }
    }

    static async Task ScrollTo(CollectionView cv, ContactItemViewModel vm)
    {
        await Task.Delay(10); // not a fan but it's the easiest way to let the layout settle
        cv.ScrollTo(vm, position: ScrollToPosition.Start);
    }
}
