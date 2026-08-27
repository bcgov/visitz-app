using Microsoft.Extensions.Logging;
using Visitz.Views.BaseClasses;
using VisitzModel.Extensions;

namespace Visitz.Views.Entity.CallDetails;

public partial class ConcernListItemView : BaseContentView
{
    public ConcernListItemView()
    {
        InitializeComponent();
    }

    async void TapGestureRecognizer_Tapped(object? sender, TappedEventArgs e)
    {
        try
        {
            if (BindingContext is ConcernListItemViewModel vm && Parent.FindFirstParent<ScrollView>() is ScrollView sv)
            {
                vm.ToggleExpanded();

                if (vm.Expanded)
                {
                    await Task.Delay(100); // not a fan but it's the easiest way to let the layout settle
                    await sv.ScrollToAsync(this, ScrollToPosition.Start, true);
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex.Message, ex);
        }
    }
}
