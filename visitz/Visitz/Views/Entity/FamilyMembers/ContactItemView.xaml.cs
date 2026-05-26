using Visitz.Views.BaseClasses;

namespace Visitz.Views.Entity.FamilyMembers;

#nullable enable

public partial class ContactItemView : BaseContentView
{
    public ContactItemView()
    {
        InitializeComponent();
    }

    async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        if (
            BindingContext is ContactItemViewModel vm
            && sender is ContactItemView item
            && item.Parent is CollectionView cv
        )
        {
            vm.ItemTapped();
            _ = ScrollTo(cv, vm);
        }
    }

    static async Task ScrollTo(CollectionView cv, ContactItemViewModel vm)
    {
        await Task.Delay(10); // not a fan but it's the easiest way to let the layout settle
        cv.ScrollTo(vm, position: ScrollToPosition.Start);
    }
}
