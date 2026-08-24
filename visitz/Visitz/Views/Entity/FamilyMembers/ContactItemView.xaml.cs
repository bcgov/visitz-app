using Microsoft.Extensions.Logging;
using Visitz.Extensions;
using Visitz.Storage;
using Visitz.Views.BaseClasses;
using Visitz.Views.Debugging;
using VisitzModel.Extensions;

namespace Visitz.Views.Entity.FamilyMembers;

public partial class ContactItemView : BaseContentView
{
    ContactItemViewModel? ViewModel => BindingContext as ContactItemViewModel;

    public ContactItemView()
    {
        InitializeComponent();

        if (DebugOptions.Default.Enabled)
            AddDebugContextMenu();
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
            Logger.LogException(ex);
        }
    }

    static async Task ScrollTo(CollectionView cv, ContactItemViewModel vm)
    {
        await Task.Delay(10); // not a fan but it's the easiest way to let the layout settle
        cv.ScrollTo(vm, position: ScrollToPosition.Start);
    }

    void AddDebugContextMenu()
    {
        MenuFlyoutItem item = new() { Text = "Delete contact locally" };
        item.Clicked += async (s, e) =>
        {
            var dataRealm = await VisitzRealms.GetIcmDataRealmAsync();
            if (ViewModel != null)
                await dataRealm.CommitAsync(() => dataRealm.Remove(ViewModel.Contact));
        };

        MenuFlyout menu = [item];
        FlyoutBase.SetContextFlyout(this, menu);
    }
}
