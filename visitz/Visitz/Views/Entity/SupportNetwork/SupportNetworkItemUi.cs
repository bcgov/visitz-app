using CommunityToolkit.Mvvm.ComponentModel;
using Visitz.FontIcons;
using Visitz.Resources.Localization;
using Visitz.Resources.Styles;
using VisitzModel.Models.People;

namespace Visitz.Views.Entity.SupportNetwork;

#nullable enable

public partial class SupportNetworkItemUi : ObservableObject
{
    [ObservableProperty]
    public partial bool IsExpanded { get; set; }

    [ObservableProperty]
    public partial SupportNetworkItem SupportNetwork { get; set; }

    [ObservableProperty]
    public partial bool ShowRelationshipTag { get; set; }

    [ObservableProperty]
    public partial string ExpandedChevronGlyph { get; set; } = MaterialIcons.Keyboard_arrow_down;

    public string ActiveText => SupportNetwork.IsActive ? LocalizedStrings.Active : LocalizedStrings.Inactive;

    public Color ActiveColor => SupportNetwork.IsActive ? VisitzColors.IsActiveTagBackground : VisitzColors.Gray100;

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
        SupportNetwork = item;
    }

    public void ToggleExpanded()
    {
        IsExpanded = !IsExpanded;
    }

    partial void OnSupportNetworkChanged(SupportNetworkItem value)
    {
        ShowRelationshipTag = !string.IsNullOrWhiteSpace(value?.Relationship);
    }

    partial void OnIsExpandedChanged(bool value)
    {
        ExpandedChevronGlyph = value ? MaterialIcons.Keyboard_arrow_up : MaterialIcons.Keyboard_arrow_down;
    }
}
