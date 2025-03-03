using CommunityToolkit.Mvvm.ComponentModel;
using VisitzModel.Models.People;

namespace Visitz.Views.Entity.SupportNetwork;

public partial class SupportNetworkItemUi : ObservableObject
{
    [ObservableProperty]
    bool isExpanded;

    [ObservableProperty]
    SupportNetworkItem supportNetworkObj;

    public SupportNetworkItemUi(SupportNetworkItem item){
        supportNetworkObj = item;
    }
}