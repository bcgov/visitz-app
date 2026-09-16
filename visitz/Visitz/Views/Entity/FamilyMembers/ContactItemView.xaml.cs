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
