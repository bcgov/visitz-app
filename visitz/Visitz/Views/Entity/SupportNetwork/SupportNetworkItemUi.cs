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

    public string CapitalizedRelationship
    {
        get
        {
            if (string.IsNullOrEmpty(SupportNetwork?.Relationship))
                return string.Empty;
            var words = SupportNetwork.Relationship.Split([' '], StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < words.Length; i++)
            {
                words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1).ToLower();
            }
            return string.Join(" ", words);
        }
    }

    public SupportNetworkItemUi(SupportNetworkItem item)
    {
        supportNetwork = item;
        ShowRelationshipTag = !string.IsNullOrWhiteSpace(item?.Relationship);
    }
}
