using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Globalization;
using Visitz.FontIcons;
using Visitz.Resources.Localization;
using Visitz.Resources.Styles;
using Visitz.Views.BaseClasses;
using VisitzModel.Extensions;
using VisitzModel.Formats;
using VisitzModel.Models.People;

namespace Visitz.Views.Entity.FamilyMembers;

#nullable enable

public partial class ContactItemViewModel : VisitzViewModel
{
    [ObservableProperty]
    public IcmContact contact;

    [ObservableProperty]
    public bool expanded;

    [ObservableProperty]
    public string tagText;

    [ObservableProperty]
    public Color tagBgColor;

    [ObservableProperty]
    public Color tagTextColor;

    [ObservableProperty]
    public string expandedChevronGlyph = MaterialIcons.Keyboard_arrow_down;

    [ObservableProperty]
    public bool isDeceased;

    [ObservableProperty]
    public string deceasedText = LocalizedStrings.Deceased;

    [RelayCommand]
    public void ItemTapped()
    {
        Expanded = !Expanded;
    }

    partial void OnContactChanged(IcmContact value)
    {
        TagText = value.Relationship;

        TagBgColor = value.IsKeyPlayer
            ? VisitzColors.ContactRelationshipTagText
            : VisitzColors.ContactRelationshipTagBackground;

        TagTextColor = value.IsKeyPlayer
            ? Colors.White
            : VisitzColors.ContactRelationshipTagText;

        IsDeceased = value.Deceased.ParseWordTruthiness();

        if (IsDeceased && value.DeceasedDate is DateTimeOffset deceasedDate)
        {
            DeceasedText = LocalizedStrings.Deceased + " "
                + deceasedDate.ToString(IcmDateFormats.BasicTimestampShort,
                    CultureInfo.InvariantCulture);
        }
    }

    partial void OnExpandedChanged(bool value)
    {
        ExpandedChevronGlyph = value
            ? MaterialIcons.Keyboard_arrow_up
            : MaterialIcons.Keyboard_arrow_down;
    }
}
