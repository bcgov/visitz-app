using System.Web;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Visitz.Device;
using Visitz.Extensions;
using Visitz.FontIcons;
using Visitz.Resources.Localization;
using Visitz.Resources.Styles;
using VisitzModel.Models.People;

namespace Visitz.Views.Entity.SupportNetwork;

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

    public string ActiveTagText => SupportNetwork.IsActive ? LocalizedStrings.Active : LocalizedStrings.Inactive;

    public Color ActiveTagBackgroundColor =>
        SupportNetwork.IsActive ? VisitzColors.TagGreenBackground : VisitzColors.Gray100;

    public Color ActiveTagTextColor => SupportNetwork.IsActive ? VisitzColors.TagGreenText : VisitzColors.BC_TextColor;

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

    [RelayCommand]
    public async Task OpenInMaps()
    {
        try
        {
            if (SupportNetwork.AddressBinding.Trim().Length > 0)
                await MapsHelper.OpenAddress(HttpUtility.UrlEncode(SupportNetwork.AddressBinding));
        }
        catch (Exception ex)
        {
            await Navigator.CurrentOpenPage.DisplayErrorAlert(ex);
        }
    }

    [RelayCommand]
    public void OpenInDialer(string phoneNumber)
    {
        try
        {
            if (phoneNumber.Trim().Length > 0)
                PhoneDialer.Default.Open(phoneNumber);
        }
        catch (Exception ex)
        {
            _ = Navigator.CurrentOpenPage.DisplayErrorAlert(ex);
        }
    }
}
