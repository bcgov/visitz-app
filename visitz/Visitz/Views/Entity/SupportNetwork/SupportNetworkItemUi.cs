using CommunityToolkit.Mvvm.ComponentModel;
using VisitzModel.Models.People;

namespace Visitz.Views.Entity.SupportNetwork;

public partial class SupportNetworkItemUi : ObservableObject
{
    [ObservableProperty]
    bool isExpanded;

    [ObservableProperty]
    SupportNetworkItem supportNetwork;

    public SupportNetworkItemUi(SupportNetworkItem item)
    {
        supportNetwork = item;
    }
}