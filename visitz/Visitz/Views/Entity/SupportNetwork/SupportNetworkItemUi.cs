using CommunityToolkit.Mvvm.ComponentModel;
using VisitzModel.Models.People;

namespace Visitz.Views.Entity.SupportNetwork;

public partial class SupportNetworkItemUi : ObservableObject
{
    [ObservableProperty]
    bool isExpanded;

    [ObservableProperty]
    SupportNetworkItem supportNetwork;

    [ObservableProperty]
    public bool showRelationshipTag;

    public SupportNetworkItemUi(SupportNetworkItem item)
    {
        supportNetwork = item;
        ShowRelationshipTag = !string.IsNullOrWhiteSpace(item?.Relationship);
    }
}
